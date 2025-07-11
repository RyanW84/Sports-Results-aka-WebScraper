using System.Net;
using HtmlAgilityPack;
using Spectre.Console;
using WebScraper_RyanW84.Models;
using WebScraper_RyanW84.Helpers;

namespace WebScraper_RyanW84.Service;

public class BasketballScraper : IScraper
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


        AnsiConsole.MarkupLine("[Blue] Passing data for SendEmail[/]");
        var results = new Results
        {
            EmailTitle = title,
            EmailTableHeadings = tableDetails,
            EmailTableRows = allRows
        };
        Helpers.DisplayTable();
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

    private string[] GetTableDetails(HtmlDocument document)
    {
        return document
            .DocumentNode.SelectNodes($"//*[@id=\"{TableId}\"]/thead/tr/th")
            ?.Select(node => node.InnerText)
            .ToArray() ?? [];
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
}