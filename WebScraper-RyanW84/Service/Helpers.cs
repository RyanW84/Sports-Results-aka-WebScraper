using Spectre.Console;

namespace WebScraper_RyanW84.Service;

public class Helpers
{
    internal Table BuildTable(string[] headings, string[][] allRows)
    {
        var table = new Table();
        foreach (var heading in headings)
            table.AddColumn(heading);

        foreach (var rowData in allRows) table.AddRow(rowData);
        return table;
    }

    public void DisplayTable(Table table)
    {
        AnsiConsole.Write(table);
    }
}