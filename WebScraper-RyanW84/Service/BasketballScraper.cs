using System.Net;
using HtmlAgilityPack;
using Spectre.Console;
using WebScraper_RyanW84.Models;

namespace WebScraper_RyanW84.Service;

internal class BasketballScraper
{
    private const string Url = "https://www.basketball-reference.com/boxscores/";
    private const string TableId = "confs_standings_E";

    public async Task<BasketballResults> Run()
    {
        var document = LoadDocument(Url);
        var title = GetTitle(document);
        var tableDetails = GetTableDetails(document);

        AnsiConsole.Write(
            new Rule("[bold italic blue]Basketball Results Scraper[/]").RuleStyle("blue").Centered()
        );

        Console.WriteLine(title);
        Console.WriteLine();

        // Collect all rows into a multidimensional array
        var allRows = GetAllTableRows(document);

        var table = BuildTable(tableDetails, allRows);
        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine("[Blue] Passing data for SendEmail[/]");
        BasketballResults results = new BasketballResults
        {
            EmailTitle = title,
            EmailTableHeadings = tableDetails,
            EmailTableRows = allRows
        };

        Console.WriteLine("Press any key to continue");

        Console.ReadKey();

        return results;
    }

    HtmlDocument LoadDocument(string url)
    {
        var web = new HtmlWeb();
        return web.Load(url);
    }

    string GetTitle(HtmlDocument document) =>
        document.DocumentNode.SelectNodes("//div/h1")?.FirstOrDefault()?.InnerText ?? string.Empty;

    internal string[] GetTableDetails(HtmlDocument document) =>
        document
            .DocumentNode.SelectNodes($"//*[@id=\"{TableId}\"]/thead/tr/th")
            ?.Select(node => node.InnerText)
            .ToArray() ?? Array.Empty<string>();

    // New method to collect all rows as a multidimensional array
    string[][] GetAllTableRows(HtmlDocument document)
    {
        var rows = new List<string[]>();
        int row = 1;
        while (true)
        {
            var dataNodes = document.DocumentNode.SelectNodes(
                $"//*[@id=\"{TableId}\"]/tbody/tr[{row}]/th|//*[@id=\"{TableId}\"]/tbody/tr[{row}]/td"
            );
            if (dataNodes == null || dataNodes.Count == 0)
                break;

            var rowData = dataNodes.Select(node => WebUtility.HtmlDecode(node.InnerText)).ToArray();
            rows.Add(rowData);
            row++;
        }
        return rows.ToArray();
    }

    // Modified to accept allRows
    Table BuildTable(string[] headings, string[][] allRows)
    {
        var table = new Table();
        foreach (var heading in headings)
            table.AddColumn(heading);

        foreach (var rowData in allRows)
        {
            table.AddRow(rowData);
        }
        return table;
    }
}
