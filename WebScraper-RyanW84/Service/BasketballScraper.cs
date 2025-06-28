using System.Net;
using HtmlAgilityPack;
using Spectre.Console;

namespace WebScraper_RyanW84.Service;

internal class BasketballScraper
{
    private const string Url = "https://www.basketball-reference.com/boxscores/";
    private const string TableId = "confs_standings_E";

    BasketballScraper()
    {
        var document = LoadDocument(Url);
        var title = GetTitle(document);
        var tableHeadings = GetTableHeadings(document);

        AnsiConsole.Write(
            new Rule("[bold italic blue]Basketball Results Scraper[/]").RuleStyle("blue").Centered()
        );

        Console.WriteLine(title);
        Console.WriteLine();

        var table = BuildTable(tableHeadings, document);
        AnsiConsole.Write(table);

        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }

    HtmlDocument LoadDocument(string url)
    {
        var web = new HtmlWeb();
        return web.Load(url);
    }

    string? GetTitle(HtmlDocument document) =>
        document.DocumentNode.SelectNodes("//div/h1")?.FirstOrDefault()?.InnerText;

    string[] GetTableHeadings(HtmlDocument document) =>
        document
            .DocumentNode.SelectNodes($"//*[@id=\"{TableId}\"]/thead/tr/th")
            ?.Select(node => node.InnerText)
            .ToArray() ?? Array.Empty<string>();

    Table BuildTable(string[] headings, HtmlDocument document)
    {
        var table = new Table();
        foreach (var heading in headings)
            table.AddColumn(heading);

        int row = 1;
        while (true)
        {
            var dataNodes = document.DocumentNode.SelectNodes(
                $"//*[@id=\"{TableId}\"]/tbody/tr[{row}]/th|//*[@id=\"{TableId}\"]/tbody/tr[{row}]/td"
            );
            if (dataNodes == null || dataNodes.Count == 0)
                break;

            var rowData = dataNodes.Select(node => WebUtility.HtmlDecode(node.InnerText)).ToArray();

            table.AddRow(rowData);
            row++;
        }
        return table;
    }
}
