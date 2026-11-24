namespace SkyFlow.Console.Application.Helpers;

public static class AsciiTableRenderer
{
    public static void Render(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> rows)
    {
        var widths = new int[headers.Count];
        for (var i = 0; i < headers.Count; i++)
        {
            widths[i] = headers[i].Length;
        }

        foreach (var row in rows)
        {
            for (var i = 0; i < headers.Count && i < row.Count; i++)
            {
                widths[i] = Math.Max(widths[i], row[i].Length);
            }
        }

        var border = "+" + string.Join("+", widths.Select(w => new string('-', w + 2))) + "+";

        global::System.Console.WriteLine(border);
        global::System.Console.WriteLine(Row(headers, widths));
        global::System.Console.WriteLine(border);

        foreach (var row in rows)
        {
            global::System.Console.WriteLine(Row(row, widths));
        }

        global::System.Console.WriteLine(border);
    }

    private static string Row(IReadOnlyList<string> values, IReadOnlyList<int> widths)
    {
        var cells = new List<string>(widths.Count);

        for (var i = 0; i < widths.Count; i++)
        {
            var value = i < values.Count ? values[i] : string.Empty;
            cells.Add(" " + value.PadRight(widths[i]) + " ");
        }

        return "|" + string.Join("|", cells) + "|";
    }
}
