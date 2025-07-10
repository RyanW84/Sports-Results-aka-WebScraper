using HtmlAgilityPack;
using WebScraper_RyanW84.Models;

namespace WebScraper_RyanW84.Service;

public class HalestormScraper : IScraper


{
    private const string GigsUrl = "https://www.halestormrocks.com/tour";

    public async Task<Results> Run()
    {
        using var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(GigsUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // 1. Get page <title> for EmailTitle
        var titleNode = doc.DocumentNode.SelectSingleNode("//title");
        var title = titleNode?.InnerText.Trim() ?? "Halestorm Upcoming Gigs";

        // 2. Try to get table headings (varies by website structure!)
        var headings = new List<string>();

        // Option 1: Standard <th> (in table or header row)
        var thNodes = doc.DocumentNode.SelectNodes("//th");
        if (thNodes is { Count: > 0 })
        {
            foreach (var th in thNodes)
                headings.Add(th.InnerText.Trim());
        }
        else
        {
            // Option 2: Headings as divs right above event row (common in modern designs)
            // Look for a parent of first event/gig with label-style children
            var firstEventNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'tour-date')]");
            if (firstEventNode != null)
            {
                var headingsNode = firstEventNode.ParentNode
                    .SelectSingleNode(
                        ".//div[contains(@class, 'row') or contains(@class, 'head') or contains(@class, 'header')]");
                if (headingsNode != null)
                {
                    // Get all immediate children that look like headings
                    var hDivs = headingsNode.SelectNodes("./div") ?? headingsNode.SelectNodes("./span");
                    if (hDivs != null)
                        foreach (var hd in hDivs)
                        {
                            var htxt = hd.InnerText.Trim();
                            if (!string.IsNullOrWhiteSpace(htxt))
                                headings.Add(htxt);
                        }
                }
            }
        }

        // Fallback if none found
        if (headings.Count == 0) headings.AddRange(["Date", "Venue", "Location", "More Info"]);

        // 3. Scrape data rows
        var gigNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'tour-date')]");
        var tableRows = new List<string[]>();

        if (gigNodes != null)
            foreach (var node in gigNodes)
            {
                var dateNode = node.SelectSingleNode(".//div[contains(@class, 'date')]");
                var venueNode = node.SelectSingleNode(".//div[contains(@class, 'venue')]");
                var cityNode = node.SelectSingleNode(".//div[contains(@class, 'location')]");
                var linkNode = node.SelectSingleNode(".//a[contains(@class, 'ticket-link')]");

                var date = dateNode?.InnerText.Trim() ?? "N/A";
                var venue = venueNode?.InnerText.Trim() ?? "N/A";
                var location = cityNode?.InnerText.Trim() ?? "N/A";
                var moreInfo = linkNode?.GetAttributeValue("href", "") ?? "";

                tableRows.Add([
                    date,
                    venue,
                    location,
                    moreInfo
                ]);
            }

        return new Results
        {
            EmailTitle = title,
            EmailTableHeadings = headings.ToArray(),
            EmailTableRows = tableRows.ToArray()
        };
    }
}