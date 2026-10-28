"""
Reference AI planner for the Quran memorization app.
Mirrors the C# PlanGeneratorService so you can test the algorithm standalone.
Run:  python planner_ai.py
"""

from dataclasses import dataclass, field, asdict
from datetime import date, timedelta
from typing import List, Dict, Any
import json

SURAHS = [
    (1,"Al-Fatiha","الفاتحة",7),(2,"Al-Baqarah","البقرة",286),
    (3,"Aal-Imran","آل عمران",200),(4,"An-Nisa","النساء",176),
    (5,"Al-Ma'idah","المائدة",120),(6,"Al-An'am","الأنعام",165),
    (7,"Al-A'raf","الأعراف",206),(8,"Al-Anfal","الأنفال",75),
    (9,"At-Tawbah","التوبة",129),(10,"Yunus","يونس",109),
    (11,"Hud","هود",123),(12,"Yusuf","يوسف",111),
    (13,"Ar-Ra'd","الرعد",43),(14,"Ibrahim","إبراهيم",52),
    (15,"Al-Hijr","الحجر",99),(16,"An-Nahl","النحل",128),
    (17,"Al-Isra","الإسراء",111),(18,"Al-Kahf","الكهف",110),
    (19,"Maryam","مريم",98),(20,"Ta-Ha","طه",135),
    (21,"Al-Anbiya","الأنبياء",112),(22,"Al-Hajj","الحج",78),
    (23,"Al-Mu'minun","المؤمنون",118),(24,"An-Nur","النور",64),
    (25,"Al-Furqan","الفرقان",77),(26,"Ash-Shu'ara","الشعراء",227),
    (27,"An-Naml","النمل",93),(28,"Al-Qasas","القصص",88),
    (29,"Al-Ankabut","العنكبوت",69),(30,"Ar-Rum","الروم",60),
    (31,"Luqman","لقمان",34),(32,"As-Sajdah","السجدة",30),
    (33,"Al-Ahzab","الأحزاب",73),(34,"Saba","سبأ",54),
    (35,"Fatir","فاطر",45),(36,"Ya-Sin","يس",83),
    (37,"As-Saffat","الصافات",182),(38,"Sad","ص",88),
    (39,"Az-Zumar","الزمر",75),(40,"Ghafir","غافر",85),
    (41,"Fussilat","فصلت",54),(42,"Ash-Shura","الشورى",53),
    (43,"Az-Zukhruf","الزخرف",89),(44,"Ad-Dukhan","الدخان",59),
    (45,"Al-Jathiyah","الجاثية",37),(46,"Al-Ahqaf","الأحقاف",35),
    (47,"Muhammad","محمد",38),(48,"Al-Fath","الفتح",29),
    (49,"Al-Hujurat","الحجرات",18),(50,"Qaf","ق",45),
    (51,"Adh-Dhariyat","الذاريات",60),(52,"At-Tur","الطور",49),
    (53,"An-Najm","النجم",62),(54,"Al-Qamar","القمر",55),
    (55,"Ar-Rahman","الرحمن",78),(56,"Al-Waqi'ah","الواقعة",96),
    (57,"Al-Hadid","الحديد",29),(58,"Al-Mujadila","المجادلة",22),
    (59,"Al-Hashr","الحشر",24),(60,"Al-Mumtahanah","الممتحنة",13),
    (61,"As-Saff","الصف",14),(62,"Al-Jumu'ah","الجمعة",11),
    (63,"Al-Munafiqun","المنافقون",11),(64,"At-Taghabun","التغابن",18),
    (65,"At-Talaq","الطلاق",12),(66,"At-Tahrim","التحريم",12),
    (67,"Al-Mulk","الملك",30),(68,"Al-Qalam","القلم",52),
    (69,"Al-Haqqah","الحاقة",52),(70,"Al-Ma'arij","المعارج",44),
    (71,"Nuh","نوح",28),(72,"Al-Jinn","الجن",28),
    (73,"Al-Muzzammil","المزمل",20),(74,"Al-Muddaththir","المدثر",56),
    (75,"Al-Qiyamah","القيامة",40),(76,"Al-Insan","الإنسان",31),
    (77,"Al-Mursalat","المرسلات",50),(78,"An-Naba","النبأ",40),
    (79,"An-Nazi'at","النازعات",46),(80,"Abasa","عبس",42),
    (81,"At-Takwir","التكوير",29),(82,"Al-Infitar","الانفطار",19),
    (83,"Al-Mutaffifin","المطففين",36),(84,"Al-Inshiqaq","الانشقاق",25),
    (85,"Al-Buruj","البروج",22),(86,"At-Tariq","الطارق",17),
    (87,"Al-A'la","الأعلى",19),(88,"Al-Ghashiyah","الغاشية",26),
    (89,"Al-Fajr","الفجر",30),(90,"Al-Balad","البلد",20),
    (91,"Ash-Shams","الشمس",15),(92,"Al-Layl","الليل",21),
    (93,"Ad-Duha","الضحى",11),(94,"Ash-Sharh","الشرح",8),
    (95,"At-Tin","التين",8),(96,"Al-Alaq","العلق",19),
    (97,"Al-Qadr","القدر",5),(98,"Al-Bayyinah","البينة",8),
    (99,"Az-Zalzalah","الزلزلة",8),(100,"Al-Adiyat","العاديات",11),
    (101,"Al-Qari'ah","القارعة",11),(102,"At-Takathur","التكاثر",8),
    (103,"Al-Asr","العصر",3),(104,"Al-Humazah","الهمزة",9),
    (105,"Al-Fil","الفيل",5),(106,"Quraysh","قريش",4),
    (107,"Al-Ma'un","الماعون",7),(108,"Al-Kawthar","الكوثر",3),
    (109,"Al-Kafirun","الكافرون",6),(110,"An-Nasr","النصر",3),
    (111,"Al-Masad","المسد",5),(112,"Al-Ikhlas","الإخلاص",4),
    (113,"Al-Falaq","الفلق",5),(114,"An-Nas","الناس",6),
]

EMOJIS = ["*","-","o","+","#","@","~",">","^","v","<","."]
CHILD_TIPS = [
    "Take a deep breath and begin with Bismillah.",
    "Listen to a beautiful recitation first.",
    "Repeat each ayah 3 times slowly.",
    "Ask a parent to check your recitation.",
    "Write down new words you learned.",
    "Make dua before you start.",
    "Take a small break every 10 minutes.",
    "Recite loudly and clearly.",
]

@dataclass
class Session:
    session_number: int
    week_number: int
    day: str
    date: str
    from_ref: str
    to_ref: str
    title: str
    description: str
    amount: float
    unit: str
    emoji: str
    completed: bool = False

@dataclass
class Plan:
    student_name: str
    summary: str
    start_date: str
    end_date: str
    total_sessions: int
    estimated_finish: str
    sessions: List[Session] = field(default_factory=list)
    milestones: List[Dict[str, Any]] = field(default_factory=list)

def _surah_index(name_or_num):
    try:
        n = int(name_or_num)
        for i, s in enumerate(SURAHS):
            if s[0] == n:
                return i
    except (ValueError, TypeError):
        pass
    for i, s in enumerate(SURAHS):
        if s[1].lower() == str(name_or_num).lower():
            return i
    return 0

def generate_plan(student_name="Little Star", age=8, start_surah="Al-Fatiha",
                  start_ayah=1, unit="verses", amount_per_session=5,
                  duration_weeks=4, sessions_per_week=5, preferred_days=None):
    preferred_days = preferred_days or ["Sun","Mon","Tue","Wed","Thu"]
    duration_weeks = max(1, duration_weeks)
    sessions_per_week = max(1, min(7, sessions_per_week))

    ayahs_per_session = {
        "verses":    amount_per_session,
        "lines":     amount_per_session * 6.0,
        "pages":     amount_per_session * 15.0,
        "half-page": amount_per_session * 7.5,
        "percent":   6236.0 * amount_per_session / 100.0,
    }.get(unit, amount_per_session)
    ayahs_per_session = max(1.0, ayahs_per_session)

    cur_surah_idx = _surah_index(start_surah)
    cur_ayah = max(1, start_ayah)
    total = duration_weeks * sessions_per_week
    today = date.today() + timedelta(days=1)

    sessions = []
    d = today
    for i in range(total):
        if cur_surah_idx >= len(SURAHS):
            break
        from_s = SURAHS[cur_surah_idx]
        from_ayah = cur_ayah

        remaining = ayahs_per_session
        to_idx = cur_surah_idx
        to_ayah = cur_ayah
        while remaining > 0 and to_idx < len(SURAHS):
            s = SURAHS[to_idx]
            available = s[3] - to_ayah + 1
            if remaining < available:
                to_ayah = to_ayah + int(remaining) - 1
                remaining = 0
            else:
                remaining -= available
                to_idx += 1
                to_ayah = 1
        to_idx = min(to_idx, len(SURAHS) - 1)
        to_s = SURAHS[to_idx]
        if to_idx >= len(SURAHS) - 1 and remaining > 0:
            to_ayah = to_s[3]

        if to_idx >= len(SURAHS) - 1 and remaining > 0:
            cur_surah_idx = len(SURAHS)
        else:
            cur_surah_idx = to_idx
            cur_ayah = to_ayah + 1
            if cur_ayah > SURAHS[cur_surah_idx][3]:
                cur_surah_idx += 1
                cur_ayah = 1

        day = preferred_days[i % len(preferred_days)]
        from_ref = f"{from_s[1]} {from_ayah}"
        to_ref   = f"{to_s[1]} {to_ayah}"

        sessions.append(Session(
            session_number=i + 1,
            week_number=i // sessions_per_week + 1,
            day=day,
            date=d.isoformat(),
            from_ref=from_ref,
            to_ref=to_ref,
            title=f"Session {i+1}: {from_ref} -> {to_ref}",
            description=CHILD_TIPS[i % len(CHILD_TIPS)],
            amount=amount_per_session,
            unit=unit,
            emoji=EMOJIS[i % len(EMOJIS)],
        ))
        d += timedelta(days=1)

    milestones = []
    for w in range(1, duration_weeks + 1):
        wk = [s for s in sessions if s.week_number == w]
        if wk:
            milestones.append({
                "title": f"Week {w} Champion! Finished {wk[-1].to_ref}",
                "emoji": "*",
                "date": wk[-1].date,
            })

    last = sessions[-1] if sessions else None
    summary = (
        f"Assalamu Alaikum {student_name}! This plan has {len(sessions)} "
        f"sessions over {duration_weeks} weeks ({sessions_per_week}/week). "
        f"Each session: {amount_per_session} {unit}."
    )
    return Plan(
        student_name=student_name,
        summary=summary,
        start_date=sessions[0].date if sessions else today.isoformat(),
        end_date=sessions[-1].date if sessions else today.isoformat(),
        total_sessions=len(sessions),
        estimated_finish=(f"You'll reach {last.to_ref} by {last.date}" if last else ""),
        sessions=sessions,
        milestones=milestones,
    )

if __name__ == "__main__":
    print(json.dumps(asdict(generate_plan(
        student_name="Aisha", age=8, start_surah="Al-Fatiha",
        duration_weeks=4, unit="verses",
        amount_per_session=5, sessions_per_week=5,
    )), ensure_ascii=False, indent=2))