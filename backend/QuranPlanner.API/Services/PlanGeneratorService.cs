using QuranPlanner.API.Models;

namespace QuranPlanner.API.Services;

public class PlanGeneratorService
{
    private readonly QuranDataService _quran;
    private readonly AyahLocationService _loc;

    public PlanGeneratorService(QuranDataService quran, AyahLocationService loc)
    {
        _quran = quran;
        _loc   = loc;
    }

    public PlanResponse Generate(PlanRequest req)
    {
        var response = new PlanResponse { StudentName = req.StudentName };
        var sessions = new List<SessionTask>();

        var startSurah = _quran.GetSurah(req.StartSurah) ?? _quran.Surahs[0];
        int startAyah  = Math.Max(1, Math.Min(req.StartAyah, startSurah.AyahCount));

        // مؤشرات الحفظ
        int curSurah = startSurah.Number;
        int curAyah  = startAyah;
        var startLoc0 = _loc.GetLocation(curSurah, curAyah);
        int curPage = startLoc0?.Page ?? startSurah.StartPage;
        int curLine = startLoc0?.Line ?? 1;

        // مؤشرات المراجعة (تبدأ من الناس - الصفحة 604)
        
        

        int totalSessions = req.DurationWeeks * req.SessionsPerWeek;
        var sessionDate = DateTime.Today.AddDays(1);
        
        var tajweedPlan = GenerateTajweedPlan(req.TajweedLevel ?? "صغار", totalSessions);
        int tajweedIndex = 0;
        
        string currentPhase = "Study";
        int phaseCounter = 0;
        int safetyCounter = 0;

        // 1) جدول المراجعة (كم سطر يراجع مقابل كل كمية حفظ)
        var reviewMap = new Dictionary<double, int> {
            {1, 5},         // 1 سطر -> 5 أسطر
            {2, 7},         // 2 سطر -> 7.5 سطر (نستخدم 7 أسطر تقريباً)
            {3.75, 15},     // ربع صفحة -> صفحة
            {5, 22},        // ثلث صفحة -> 1.5 صفحة
            {7.5, 30},      // نصف صفحة -> صفحتان
            {15, 37},       // صفحة -> 2.5 صفحة
            {22.5, 45},     // صفحة ونصف -> 3 صفحات
            {30, 50},       // صفحتان -> ربع (تقريباً 50 سطر)
            {50, 75},       // ربع جزء -> 2 ربع
            {100, 150},     // ربعان -> 3 ربع
            {150, 200},     // 3 أرباع -> حزب
            {200, 400}      // حزب -> جزء
        };
        int linesToReview = reviewMap.ContainsKey(req.AmountPerSession) ? reviewMap[req.AmountPerSession] : (int)req.AmountPerSession;

        while (sessions.Count < totalSessions && safetyCounter < 1000)
        {
            safetyCounter++;
            if (curSurah == 114 && curAyah == _quran.Surahs[113].AyahCount) break;

            // =========================================================
            // مرحلة التجهيز والتصعيد
            // =========================================================
            if (currentPhase != "Study")
            {
                string title = "", desc = "", emoji = "", phaseType = "";
                
                if (currentPhase == "Prep1") {
                    title = $"تجهيز المرحلة ({phaseCounter}/4)"; desc = "مراجعة شاملة للجزء"; emoji = "🛠️"; phaseType = "تجهيز المرحلة";
                    phaseCounter++; if (phaseCounter > 4) { currentPhase = "Promo1"; phaseCounter = 1; }
                }
                else if (currentPhase == "Promo1") {
                    title = "تصعيد المرحلة"; desc = "اختبار الجزء بالكامل"; emoji = "🎓"; phaseType = "تصعيد المرحلة";
                    currentPhase = "Prep2"; phaseCounter = 1;
                }
                else if (currentPhase == "Prep2") {
                    title = $"تجهيز الربط ({phaseCounter}/4)"; desc = "مراجعة ربط الجزء"; emoji = "🔗"; phaseType = "تجهيز الربط";
                    phaseCounter++; if (phaseCounter > 4) { currentPhase = "Promo2"; phaseCounter = 1; }
                }
                else if (currentPhase == "Promo2") {
                    title = "تصعيد الربط"; desc = "اختبار ربط الجزء"; emoji = "🏆"; phaseType = "تصعيد الربط";
                    currentPhase = "Study"; phaseCounter = 0;
                }

                sessions.Add(new SessionTask {
                    SessionNumber = sessions.Count + 1, WeekNumber = (sessions.Count / req.SessionsPerWeek) + 1,
                    Day = GetDayName(sessionDate.DayOfWeek), Date = sessionDate.ToString("yyyy-MM-dd"),
                    FromRef = title, ToRef = "", FromPage = 0, ToPage = 0, PageRef = "",
                    Title = title, Description = desc, Amount = 0, Unit = "", Emoji = emoji,
                    IsSurahPromotion = phaseType.Contains("تصعيد"), PromotionSurahName = phaseType, Completed = false
                });
                sessionDate = sessionDate.AddDays(1);
                continue; 
            }

            // =========================================================
            // مرحلة الحفظ العادي
            // =========================================================
            int fromSurahNum = curSurah;
            int fromAyahNum  = curAyah;
            int fromPage     = curPage;
            int fromLine     = curLine;

            int n = Math.Max(1, (int)Math.Round(req.AmountPerSession));
            
            // حساب نهاية الحفظ
            var (endPage, endLine) = _loc.AdvanceLines(curPage, curLine, n - 1);
            var lastAyah = _loc.GetLastAyahOnLine(endPage, endLine) ?? _loc.GetLastAyahOnPage(endPage);
            
            int toSurahNum = lastAyah?.Surah ?? 114;
            int toAyahNum  = lastAyah?.Ayah ?? 6;

            var fromSurahInfo = _quran.Surahs[fromSurahNum - 1];
            var toSurahInfo   = _quran.Surahs[toSurahNum - 1];

            string fromRef = $"{fromSurahInfo.ArabicName} {fromAyahNum}";
            string toRef   = $"{toSurahInfo.ArabicName} {toAyahNum}";
            string pageRef = fromPage == endPage ? $"ص.{fromPage}" : $"ص.{fromPage} -> ص.{endPage}";

            // =========================================================
            // حساب المراجعة (الكمية فقط بناءً على الجدول)
            // =========================================================
            string reviewRefStr = "مراجعة عامة";
            double reviewAmount = req.AmountPerSession; // القيمة الافتراضية
            
            if (req.AmountPerSession == 1) reviewAmount = 5;
            else if (req.AmountPerSession == 2) reviewAmount = 7;
            else if (req.AmountPerSession == 3.75) reviewAmount = 15;
            else if (req.AmountPerSession == 5) reviewAmount = 22;
            else if (req.AmountPerSession == 7.5) reviewAmount = 30;
            else if (req.AmountPerSession == 15) reviewAmount = 37;
            else if (req.AmountPerSession == 22.5) reviewAmount = 45;
            else if (req.AmountPerSession == 30) reviewAmount = 50;
            else if (req.AmountPerSession == 50) reviewAmount = 75;
            else if (req.AmountPerSession == 100) reviewAmount = 150;
            else if (req.AmountPerSession == 150) reviewAmount = 200;
            else if (req.AmountPerSession == 200) reviewAmount = 400;

            if (reviewAmount == 5.0) reviewRefStr = "5 أسطر";
            else if (reviewAmount == 7.5) reviewRefStr = "7.5 سطر (نصف صفحة)";
            else if (reviewAmount == 15.0) reviewRefStr = "صفحة كاملة";
            else if (reviewAmount == 22.0) reviewRefStr = "1.5 صفحة";
            else if (reviewAmount == 30.0) reviewRefStr = "صفحتان";
            else if (reviewAmount == 37.0) reviewRefStr = "2.5 صفحة";
            else if (reviewAmount == 45.0) reviewRefStr = "3 صفحات";
            else if (reviewAmount == 50.0) reviewRefStr = "ربع جزء (2 ربع)";
            else if (reviewAmount == 75.0) reviewRefStr = "3 أرباع";
            else if (reviewAmount == 150.0) reviewRefStr = "حزب";
            else if (reviewAmount == 200.0) reviewRefStr = "جزء كامل";
            else reviewRefStr = $"{reviewAmount} سطر";

            string tajweedLesson = "مراجعة عامة";
            if (tajweedIndex < tajweedPlan.Count) {
                tajweedLesson = tajweedPlan[tajweedIndex];
                tajweedIndex++;
            }

            bool surahEnded = (fromSurahNum != toSurahNum) || (toAyahNum == toSurahInfo.AyahCount);
            string promoName = surahEnded ? toSurahInfo.Name : "";

            sessions.Add(new SessionTask {
                SessionNumber = sessions.Count + 1, WeekNumber = (sessions.Count / req.SessionsPerWeek) + 1,
                Day = GetDayName(sessionDate.DayOfWeek), Date = sessionDate.ToString("yyyy-MM-dd"),
                FromRef = fromRef, ToRef = toRef, FromPage = fromPage, ToPage = endPage, PageRef = pageRef,
                Title = $"Session {sessions.Count + 1}: {fromRef} -> {toRef}",
                Description = "حفظ جديد + مراجعة", Amount = req.AmountPerSession, Unit = "سطر", Emoji = "📖",
                ReviewRef = reviewRefStr, TajweedLesson = tajweedLesson, IsSurahPromotion = false, Completed = false
            });
            sessionDate = sessionDate.AddDays(1);

            if (surahEnded) {
                sessions.Add(new SessionTask {
                    SessionNumber = sessions.Count + 1, WeekNumber = (sessions.Count / req.SessionsPerWeek) + 1,
                    Day = GetDayName(sessionDate.DayOfWeek), Date = sessionDate.ToString("yyyy-MM-dd"),
                    FromRef = $"تصعيد {toSurahInfo.ArabicName}", ToRef = toSurahInfo.ArabicName, FromPage = endPage, ToPage = endPage, PageRef = pageRef,
                    Title = $"🎉 تصعيد سورة {toSurahInfo.ArabicName}", Description = "تقييم المعلم", Amount = 0, Unit = "", Emoji = "⭐",
                    IsSurahPromotion = true, PromotionSurahName = toSurahInfo.ArabicName, Completed = false
                });
                sessionDate = sessionDate.AddDays(1);
            }

            var (nextPage, nextLine) = _loc.AdvanceLines(endPage, endLine, 1);
            curPage = nextPage; curLine = nextLine;
            var nextAyah = _loc.GetFirstAyahOnLine(curPage, curLine) ?? _loc.GetFirstAyahOnPage(curPage);
            if (nextAyah != null) { curSurah = nextAyah.Value.Surah; curAyah = nextAyah.Value.Ayah; }

            int pagesDone = curPage - (_loc.GetLocation(startSurah.Number, startAyah)?.Page ?? startSurah.StartPage);
            if (pagesDone >= 20) { currentPhase = "Prep1"; phaseCounter = 1; }
        }

        response.Sessions = sessions;
        response.TotalSessions = sessions.Count;
        response.StartDate = sessions.FirstOrDefault()?.Date ?? DateTime.Today.ToString("yyyy-MM-dd");
        response.EndDate = sessions.LastOrDefault()?.Date  ?? DateTime.Today.ToString("yyyy-MM-dd");
        response.EstimatedFinish = $"ستصل إلى {sessions.LastOrDefault()?.ToRef} بحلول {response.EndDate}";
        return response;
    }

    private List<string> GenerateTajweedPlan(string level, int totalSessions)
    {
        var plan = new List<string>();
        if (level == "صغار") {
            plan.Add("تمهيد: معرفة أحكام التجويد");
            plan.Add("مراجعة"); plan.Add("الميم والنون المشددتين (1/2)"); plan.Add("الميم والنون المشددتين (2/2)");
            for(int i=1; i<=7; i++) plan.Add($"الميم الساكنة ({i}/7)");
            for(int i=1; i<=10; i++) plan.Add($"النون الساكنة ({i}/10)");
            plan.Add("مراجعة الغن"); plan.Add("اختبار الغن");
            plan.Add("مراجعة"); plan.Add("تحديد المد (1/2)"); plan.Add("تحديد المد (2/2)");
            plan.Add("مد حركة (1/2)"); plan.Add("مد حركة (2/2)"); plan.Add("مد سكون (1/2)"); plan.Add("مد سكون (2/2)");
            plan.Add("مد همزة كلمة (1/2)"); plan.Add("مد همزة كلمة (2/2)"); plan.Add("مراجعة المدود"); plan.Add("مراجعة"); plan.Add("اختبار المدود");
        }
        else if (level == "شباب") {
            plan.Add("تمهيد"); for(int i=1; i<=21; i++) plan.Add($"الغن ({i}/21)");
            for(int i=1; i<=12; i++) plan.Add($"المدود ({i}/12)"); for(int i=1; i<=18; i++) plan.Add($"التفخيم والترقيق ({i}/18)");
            for(int i=1; i<=14; i++) plan.Add($"السواكن ({i}/14)"); for(int i=1; i<=12; i++) plan.Add($"المخارج ({i}/12)");
            for(int i=1; i<=33; i++) plan.Add($"الصفات ({i}/33)"); for(int i=1; i<=8; i++) plan.Add($"تتمات ({i}/8)");
        }
        else {
            plan.Add("تمهيد"); for(int i=1; i<=21; i++) plan.Add($"الغن ({i}/21)");
            for(int i=1; i<=12; i++) plan.Add($"المدود ({i}/12)"); for(int i=1; i<=18; i++) plan.Add($"التفخيم والترقيق ({i}/18)");
            for(int i=1; i<=14; i++) plan.Add($"السواكن ({i}/14)"); for(int i=1; i<=12; i++) plan.Add($"المخارج ({i}/12)");
            for(int i=1; i<=33; i++) plan.Add($"الصفات ({i}/33)"); for(int i=1; i<=8; i++) plan.Add($"تتمات ({i}/8)");
        }
        while (plan.Count < totalSessions) plan.Add("مراجعة عامة");
        return plan;
    }

    private static string GetDayName(DayOfWeek d) => d switch {
        DayOfWeek.Sunday => "الأحد", DayOfWeek.Monday => "الاثنين", DayOfWeek.Tuesday => "الثلاثاء",
        DayOfWeek.Wednesday => "الأربعاء", DayOfWeek.Thursday => "الخميس", DayOfWeek.Friday => "الجمعة", _ => "السبت"
    };
}