namespace QuranPlanner.API.Services;

public record SurahInfo(
    int Number,
    string Name,
    string ArabicName,
    int AyahCount,
    int StartPage,   // Madinah Mushaf (KFGQPC Hafs, Uthmani)
    int EndPage,     // last page of the surah
    int Lines        // exact count of text lines in the mushaf
);

public class QuranDataService
{
    // Madinah Mushaf (King Fahd Complex), Hafs 'an Asim, Uthmani script
    // 604 pages. Page and line data derived from real Mushaf layout.
    public List<SurahInfo> Surahs { get; } = new()
    {
        new(1, "Al-Fatiha", "الفاتحة", 7, 1, 1, 7),
        new(2, "Al-Baqarah", "البقرة", 286, 2, 49, 711),
        new(3, "Aal-Imran", "آل عمران", 200, 50, 76, 402),
        new(4, "An-Nisa", "النساء", 176, 77, 106, 439),
        new(5, "Al-Ma'idah", "المائدة", 120, 106, 127, 323),
        new(6, "Al-An'am", "الأنعام", 165, 128, 150, 343),
        new(7, "Al-A'raf", "الأعراف", 206, 151, 176, 388),
        new(8, "Al-Anfal", "الأنفال", 75, 177, 186, 148),
        new(9, "At-Tawbah", "التوبة", 129, 187, 207, 313),
        new(10, "Yunus", "يونس", 109, 208, 221, 200),
        new(11, "Hud", "هود", 123, 221, 235, 210),
        new(12, "Yusuf", "يوسف", 111, 235, 248, 200),
        new(13, "Ar-Ra'd", "الرعد", 43, 249, 255, 90),
        new(14, "Ibrahim", "إبراهيم", 52, 255, 261, 101),
        new(15, "Al-Hijr", "الحجر", 99, 262, 267, 79),
        new(16, "An-Nahl", "النحل", 128, 267, 281, 217),
        new(17, "Al-Isra", "الإسراء", 111, 282, 293, 172),
        new(18, "Al-Kahf", "الكهف", 110, 293, 304, 169),
        new(19, "Maryam", "مريم", 98, 305, 312, 107),
        new(20, "Ta-Ha", "طه", 135, 312, 321, 144),
        new(21, "Al-Anbiya", "الأنبياء", 112, 322, 331, 147),
        new(22, "Al-Hajj", "الحج", 78, 332, 341, 148),
        new(23, "Al-Mu'minun", "المؤمنون", 118, 342, 349, 118),
        new(24, "An-Nur", "النور", 64, 350, 359, 144),
        new(25, "Al-Furqan", "الفرقان", 77, 359, 366, 107),
        new(26, "Ash-Shu'ara", "الشعراء", 227, 367, 376, 148),
        new(27, "An-Naml", "النمل", 93, 377, 385, 126),
        new(28, "Al-Qasas", "القصص", 88, 385, 396, 163),
        new(29, "Al-Ankabut", "العنكبوت", 69, 396, 404, 120),
        new(30, "Ar-Rum", "الروم", 60, 404, 410, 94),
        new(31, "Luqman", "لقمان", 34, 411, 414, 57),
        new(32, "As-Sajdah", "السجدة", 30, 415, 417, 43),
        new(33, "Al-Ahzab", "الأحزاب", 73, 418, 427, 149),
        new(34, "Saba", "سبأ", 54, 428, 434, 95),
        new(35, "Fatir", "فاطر", 45, 434, 440, 84),
        new(36, "Ya-Sin", "يس", 83, 440, 445, 84),
        new(37, "As-Saffat", "الصافات", 182, 446, 452, 103),
        new(38, "Sad", "ص", 88, 453, 458, 77),
        new(39, "Az-Zumar", "الزمر", 75, 458, 467, 132),
        new(40, "Ghafir", "غافر", 85, 467, 476, 146),
        new(41, "Fussilat", "فصلت", 54, 477, 482, 88),
        new(42, "Ash-Shura", "الشورى", 53, 483, 489, 92),
        new(43, "Az-Zukhruf", "الزخرف", 89, 489, 495, 99),
        new(44, "Ad-Dukhan", "الدخان", 59, 496, 498, 42),
        new(45, "Al-Jathiyah", "الجاثية", 37, 499, 502, 50),
        new(46, "Al-Ahqaf", "الأحقاف", 35, 502, 506, 66),
        new(47, "Muhammad", "محمد", 38, 507, 510, 59),
        new(48, "Al-Fath", "الفتح", 29, 511, 515, 64),
        new(49, "Al-Hujurat", "الحجرات", 18, 515, 517, 37),
        new(50, "Qaf", "ق", 45, 518, 520, 39),
        new(51, "Adh-Dhariyat", "الذاريات", 60, 520, 523, 39),
        new(52, "At-Tur", "الطور", 49, 523, 525, 35),
        new(53, "An-Najm", "النجم", 62, 526, 528, 37),
        new(54, "Al-Qamar", "القمر", 55, 528, 531, 39),
        new(55, "Ar-Rahman", "الرحمن", 78, 531, 534, 45),
        new(56, "Al-Waqi'ah", "الواقعة", 96, 534, 537, 47),
        new(57, "Al-Hadid", "الحديد", 29, 537, 541, 63),
        new(58, "Al-Mujadila", "المجادلة", 22, 542, 545, 49),
        new(59, "Al-Hashr", "الحشر", 24, 545, 548, 51),
        new(60, "Al-Mumtahanah", "الممتحنة", 13, 549, 551, 35),
        new(61, "As-Saff", "الصف", 14, 551, 552, 22),
        new(62, "Al-Jumu'ah", "الجمعة", 11, 553, 554, 19),
        new(63, "Al-Munafiqun", "المنافقون", 11, 554, 555, 21),
        new(64, "At-Taghabun", "التغابن", 18, 556, 557, 28),
        new(65, "At-Talaq", "الطلاق", 12, 558, 559, 29),
        new(66, "At-Tahrim", "التحريم", 12, 560, 561, 28),
        new(67, "Al-Mulk", "الملك", 30, 562, 564, 33),
        new(68, "Al-Qalam", "القلم", 52, 564, 566, 31),
        new(69, "Al-Haqqah", "الحاقة", 52, 566, 568, 28),
        new(70, "Al-Ma'arij", "المعارج", 44, 568, 570, 24),
        new(71, "Nuh", "نوح", 28, 570, 571, 24),
        new(72, "Al-Jinn", "الجن", 28, 572, 573, 28),
        new(73, "Al-Muzzammil", "المزمل", 20, 574, 575, 20),
        new(74, "Al-Muddaththir", "المدثر", 56, 575, 577, 26),
        new(75, "Al-Qiyamah", "القيامة", 40, 577, 578, 16),
        new(76, "Al-Insan", "الإنسان", 31, 578, 580, 26),
        new(77, "Al-Mursalat", "المرسلات", 50, 580, 581, 22),
        new(78, "An-Naba", "النبأ", 40, 582, 583, 20),
        new(79, "An-Nazi'at", "النازعات", 46, 583, 584, 20),
        new(80, "Abasa", "عبس", 42, 585, 585, 14),
        new(81, "At-Takwir", "التكوير", 29, 586, 586, 12),
        new(82, "Al-Infitar", "الانفطار", 19, 587, 587, 9),
        new(83, "Al-Mutaffifin", "المطففين", 36, 587, 589, 19),
        new(84, "Al-Inshiqaq", "الانشقاق", 25, 589, 589, 12),
        new(85, "Al-Buruj", "البروج", 22, 590, 590, 12),
        new(86, "At-Tariq", "الطارق", 17, 591, 591, 6),
        new(87, "Al-A'la", "الأعلى", 19, 591, 592, 8),
        new(88, "Al-Ghashiyah", "الغاشية", 26, 592, 592, 11),
        new(89, "Al-Fajr", "الفجر", 30, 593, 594, 16),
        new(90, "Al-Balad", "البلد", 20, 594, 594, 9),
        new(91, "Ash-Shams", "الشمس", 15, 595, 595, 7),
        new(92, "Al-Layl", "الليل", 21, 595, 596, 8),
        new(93, "Ad-Duha", "الضحى", 11, 596, 596, 5),
        new(94, "Ash-Sharh", "الشرح", 8, 596, 596, 3),
        new(95, "At-Tin", "التين", 8, 597, 597, 4),
        new(96, "Al-Alaq", "العلق", 19, 597, 597, 7),
        new(97, "Al-Qadr", "القدر", 5, 598, 598, 3),
        new(98, "Al-Bayyinah", "البينة", 8, 598, 599, 10),
        new(99, "Az-Zalzalah", "الزلزلة", 8, 599, 599, 5),
        new(100, "Al-Adiyat", "العاديات", 11, 599, 600, 5),
        new(101, "Al-Qari'ah", "القارعة", 11, 600, 600, 6),
        new(102, "At-Takathur", "التكاثر", 8, 600, 600, 4),
        new(103, "Al-Asr", "العصر", 3, 601, 601, 2),
        new(104, "Al-Humazah", "الهمزة", 9, 601, 601, 4),
        new(105, "Al-Fil", "الفيل", 5, 601, 601, 3),
        new(106, "Quraysh", "قريش", 4, 602, 602, 3),
        new(107, "Al-Ma'un", "الماعون", 7, 602, 602, 4),
        new(108, "Al-Kawthar", "الكوثر", 3, 602, 602, 2),
        new(109, "Al-Kafirun", "الكافرون", 6, 603, 603, 3),
        new(110, "An-Nasr", "النصر", 3, 603, 603, 3),
        new(111, "Al-Masad", "المسد", 5, 603, 603, 3),
        new(112, "Al-Ikhlas", "الإخلاص", 4, 604, 604, 2),
        new(113, "Al-Falaq", "الفلق", 5, 604, 604, 3),
        new(114, "An-Nas", "الناس", 6, 604, 604, 4),
    };

    public SurahInfo? GetSurah(string nameOrNumber)
    {
        if (int.TryParse(nameOrNumber, out int num))
            return Surahs.FirstOrDefault(s => s.Number == num);
        return Surahs.FirstOrDefault(s =>
            s.Name.Equals(nameOrNumber, StringComparison.OrdinalIgnoreCase));
    }

    public int GetIndex(string nameOrNumber)
    {
        var s = GetSurah(nameOrNumber);
        return s == null ? 0 : Surahs.IndexOf(s);
    }
}
