using System.Net;
using HtmlAgilityPack;
using Spectre.Console;
using WebScraper_RyanW84.Models;

namespace WebScraper_RyanW84.Service;

public class BasketballScraper
{
    private const string Url = "https://www.basketball-reference.com/boxscores/";
    private const string TableId = "confs_standings_E";

    public async Task<Results> Run()
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
        var results = new Results
        {
            EmailTitle = title,
            EmailTableHeadings = tableDetails,
            EmailTableRows = allRows
        };

        Console.WriteLine("Press any key to continue");

        Console.ReadKey();

        return results;
    }

    private HtmlDocument LoadDocument(string url)
    {
        var web = new HtmlWeb();
        return web.Load(url);
    }

    private string GetTitle(HtmlDocument document)
    {
        return document.DocumentNode.SelectNodes("//div/h1")?.FirstOrDefault()?.InnerText ?? string.Empty;
    }

    internal string[] GetTableDetails(HtmlDocument document)
    {
        return document
            .DocumentNode.SelectNodes($"//*[@id=\"{TableId}\"]/thead/tr/th")
            ?.Select(node => node.InnerText)
            .ToArray() ?? Array.Empty<string>();
    }

    // New method to collect all rows as a multidimensional array
    private string[][] GetAllTableRows(HtmlDocument document)
    {
        var rows = new List<string[]>();
        var row = 1;
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
    private Table BuildTable(string[] headings, string[][] allRows)
    {
        var table = new Table();
        foreach (var heading in headings)
            table.AddColumn(heading);

        foreach (var rowData in allRows) table.AddRow(rowData);
        return table;
    }
}