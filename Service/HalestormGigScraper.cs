using HtmlAgilityPack;
using Spectre.Console;
using WebScraper_RyanW84.Models;

namespace WebScraper_RyanW84.Service;

public class HalestormScraper() : IScraper
{
    private const string Url = "https://www.halestormrocks.com/tour";
    private const string TourDateClass = "tour-date";
    private readonly Helpers _helpers;

    public HalestormScraper(Helpers helpers) :this()
    {
        _helpers = helpers;
    }

    public async Task<Results> Run()
    {
        var document = await LoadDocument(Url);
        var title = GetTitle(document);
        var tableDetails = GetTableDetails(document);
        var allRows = GetAllTableRows(document);

        DisplayScraperInfo(title);

        var results = new Results
        {
            EmailTitle = title,
            EmailTableHeadings = tableDetails,
            EmailTableRows = allRows
        };

        _helpers.DisplayTable(_helpers.BuildTable(tableDetails, allRows));
        return results;
    }

    private async Task<HtmlDocument> LoadDocument(string url)
    {
        using var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc;
    }

    private string GetTitle(HtmlDocument document)
    {
        return document.DocumentNode.SelectSingleNode("//title")?.InnerText.Trim() 
            ?? "Halestorm Upcoming Gigs";
    }

    private string[] GetTableDetails(HtmlDocument document)
    {
       string[] tableHeadings =document.DocumentNode.SelectSingleNode("//div[contains(@class, 'tour-date-container')]")?.InnerText.Split("\n");
       return tableHeadings;
    }

    private string[][] GetAllTableRows(HtmlDocument document)
    {
        try
        {
            var gigNodes = document.DocumentNode.SelectNodes($"//div[contains(@class, '{TourDateClass}')]");
        var rows = new List<string[]>();

        if (gigNodes != null)
        {
            foreach (var node in gigNodes)
            {
                var date = ExtractNodeText(node, "date");
                var venue = ExtractNodeText(node, "venue");
                var location = ExtractNodeText(node, "location");
                var moreInfo = ExtractLinkHref(node);

                rows.Add([
                    date,
                    venue,
                    location,
                    moreInfo
                ]);
            }
        }

        return rows.ToArray();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error extracting table rows: {ex.Message}");
            return Array.Empty<string[]>();
        }
        
    }

    private string ExtractNodeText(HtmlNode parentNode, string className)
    {
        return parentNode.SelectSingleNode($".//div[contains(@class, '{className}')]")?.InnerText.Trim() ?? "N/A";
    }

    private string ExtractLinkHref(HtmlNode parentNode)
    {
        return parentNode.SelectSingleNode(".//a[contains(@class, 'ticket-link')]")?
            .GetAttributeValue("href", "") ?? "";
    }

    private void DisplayScraperInfo(string title)
    {
        AnsiConsole.Write(
            new Rule("[bold italic yellow]Halestorm Gigs Scraper[/]")
                .RuleStyle("yellow")
                .Centered()
        );

        Console.WriteLine(title);
        Console.WriteLine();
        AnsiConsole.MarkupLine("[Yellow]Passing data for SendEmail[/]");
    }
}