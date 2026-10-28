using System.Text.Json;

namespace QuranPlanner.API.Services;

public record AyahLocation(int Page, int Line);

public record LineContent(int Page, int Line,
                          int FromSurah, int FromAyah,
                          int ToSurah, int ToAyah,
                          int Count);

public class AyahLocationService
{
    private readonly Dictionary<string, AyahLocation> _ayahs;
    private readonly Dictionary<string, LineContent> _lines;
    private readonly Dictionary<int, int> _pageLines;
    private readonly List<(int Surah, int Ayah, int Page, int Line)> _ordered;
    private readonly Dictionary<string, int> _orderIndex;

    public int TotalPages { get; }
    public int TotalLines { get; }
    public int TotalAyahs => _ordered.Count;

    public AyahLocationService(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "ayah-locations.json");
        if (!File.Exists(path))
            throw new FileNotFoundException($"Ayah locations file not found at {path}");

        using var fs = File.OpenRead(path);
        using var doc = JsonDocument.Parse(fs);
        var root = doc.RootElement;

        _ayahs = new Dictionary<string, AyahLocation>();
        _lines = new Dictionary<string, LineContent>();
        _ordered = new List<(int, int, int, int)>();
        _orderIndex = new Dictionary<string, int>();

        if (root.TryGetProperty("ayahs", out var ayahsEl))
        {
            foreach (var prop in ayahsEl.EnumerateObject())
            {
                var parts = prop.Name.Split(':');
                if (parts.Length != 2) continue;
                if (!int.TryParse(parts[0], out int s)) continue;
                if (!int.TryParse(parts[1], out int a)) continue;

                var p = prop.Value.GetProperty("p").GetInt32();
                var l = prop.Value.GetProperty("l").GetInt32();
                _ayahs[prop.Name] = new AyahLocation(p, l);
                _ordered.Add((s, a, p, l));
            }
        }

        _ordered = _ordered
            .OrderBy(x => x.Item3)
            .ThenBy(x => x.Item4)
            .ThenBy(x => x.Item1)
            .ThenBy(x => x.Item2)
            .ToList();

        for (int i = 0; i < _ordered.Count; i++)
        {
            var r = _ordered[i];
            _orderIndex[$"{r.Item1}:{r.Item2}"] = i;
        }

        if (root.TryGetProperty("lines", out var linesEl))
        {
            foreach (var prop in linesEl.EnumerateObject())
            {
                var parts = prop.Name.Split(':');
                if (parts.Length != 2) continue;
                if (!int.TryParse(parts[0], out int p)) continue;
                if (!int.TryParse(parts[1], out int l)) continue;

                var v = prop.Value;
                _lines[prop.Name] = new LineContent(
                    p, l,
                    v.GetProperty("fromS").GetInt32(),
                    v.GetProperty("fromA").GetInt32(),
                    v.GetProperty("toS").GetInt32(),
                    v.GetProperty("toA").GetInt32(),
                    v.GetProperty("count").GetInt32());
            }
        }

        _pageLines = new Dictionary<int, int>();
        if (root.TryGetProperty("pages", out var pagesEl))
        {
            foreach (var prop in pagesEl.EnumerateObject())
            {
                if (int.TryParse(prop.Name, out int pageNum))
                    _pageLines[pageNum] = prop.Value.GetInt32();
            }
        }

        TotalPages = root.TryGetProperty("totalPages", out var tp) ? tp.GetInt32() : 604;
        TotalLines = _pageLines.Values.Sum();
        if (TotalLines <= 0) TotalLines = 6236;

        Console.WriteLine($"[AyahLocationService] Ayahs={_ordered.Count}, Lines={_lines.Count}, Pages={_pageLines.Count}");
    }

    public AyahLocation? GetLocation(int surah, int ayah)
        => _ayahs.TryGetValue($"{surah}:{ayah}", out var loc) ? loc : null;

    public LineContent? GetLine(int page, int line)
        => _lines.TryGetValue($"{page}:{line}", out var lc) ? lc : null;

    public int GetPageLineCount(int page)
        => _pageLines.TryGetValue(page, out var n) ? n : 15;

    /// <summary>
    /// يتحرك بمقدار N آية من (surah, ayah) الحالية.
    /// count = عدد الآيات التي ستُقرأ (من ضمنها الحالية).
    /// متى count=5، نتحرك 4 مواضع.
    /// </summary>
    public (int Surah, int Ayah) AdvanceAyahs(int surah, int ayah, int count)
    {
        string key = $"{surah}:{ayah}";
        int idx;
        if (!_orderIndex.TryGetValue(key, out idx))
        {
            idx = _ordered.FindIndex(x => x.Item1 > surah || (x.Item1 == surah && x.Item2 >= ayah));
            if (idx < 0) idx = _ordered.Count - 1;
        }
        int steps = Math.Max(0, count - 1);
        int target = Math.Min(idx + steps, _ordered.Count - 1);
        if (target < 0) target = 0;
        var t = _ordered[target];
        return (t.Item1, t.Item2);
    }

    /// <summary>
    /// يُرجع الآية التالية (تتقدم واحدة).
    /// </summary>
    public (int Surah, int Ayah)? GetNextAyah(int surah, int ayah)
    {
        string key = $"{surah}:{ayah}";
        if (!_orderIndex.TryGetValue(key, out int idx)) return null;
        if (idx + 1 >= _ordered.Count) return null;
        var t = _ordered[idx + 1];
        return (t.Item1, t.Item2);
    }

    /// <summary>
    /// يُرجع أول آية في صفحة معينة.
    /// </summary>
    public (int Surah, int Ayah)? GetFirstAyahOnPage(int page)
    {
        for (int i = 0; i < _ordered.Count; i++)
            if (_ordered[i].Item3 >= page) return (_ordered[i].Item1, _ordered[i].Item2);
        return null;
    }

    /// <summary>
    /// يُرجع آخر آية في صفحة معينة.
    /// </summary>
    public (int Surah, int Ayah)? GetLastAyahOnPage(int page)
    {
        for (int i = _ordered.Count - 1; i >= 0; i--)
            if (_ordered[i].Item3 <= page) return (_ordered[i].Item1, _ordered[i].Item2);
        return null;
    }

    /// <summary>
    /// يُرجع أول آية في السطر (صفحة، سطر).
    /// </summary>
    public (int Surah, int Ayah)? GetFirstAyahOnLine(int page, int line)
    {
        var lc = GetLine(page, line);
        return lc == null ? null : (lc.FromSurah, lc.FromAyah);
    }

    /// <summary>
    /// يُرجع آخر آية في السطر (صفحة، سطر).
    /// </summary>
    public (int Surah, int Ayah)? GetLastAyahOnLine(int page, int line)
    {
        var lc = GetLine(page, line);
        return lc == null ? null : (lc.ToSurah, lc.ToAyah);
    }

    public int? GetPageOfAyah(int surah, int ayah)
        => GetLocation(surah, ayah)?.Page;

    public int AdvancePages(int currentPage, int pagesToAdvance)
    {
        int target = currentPage + pagesToAdvance - 1;
        return Math.Min(Math.Max(1, target), TotalPages);
    }

    /// <summary>
    /// يتحرك بعدد من الأسطر من (صفحة، سطر).
    /// </summary>
    public (int Page, int Line) AdvanceLines(int startPage, int startLine, int linesToAdvance)
    {
        int page = startPage;
        int line = startLine;
        int remaining = linesToAdvance;

        while (remaining > 0 && page <= TotalPages)
        {
            int pageLines = GetPageLineCount(page);
            int available = pageLines - line + 1;

            if (remaining < available)
            {
                line += remaining;
                remaining = 0;
            }
            else
            {
                remaining -= available;
                page++;
                line = 1;
            }
        }

        if (page > TotalPages) return (TotalPages, GetPageLineCount(TotalPages));
        return (page, line);
    }
}
