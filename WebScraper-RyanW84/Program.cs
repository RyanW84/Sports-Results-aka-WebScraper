using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using HtmlAgilityPack;
using Spectre.Console;

namespace WebScraper_RyanW84
{
    public class WebScraper
    {
        public static async Task Main(string[] args)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://www.basketball-reference.com/boxscores/");

            var title = document.DocumentNode.SelectNodes("//div/h1").First().InnerText; // Website Title
            var subHeading = document
                .DocumentNode.SelectSingleNode( // Table heading
                    "//*[@id=\"confs_standings_E\"]/thead/tr/th[1]"
                )
                ?.InnerText; // Handle null case

            var tableHeadingsNodes = document.DocumentNode.SelectNodes( // Table headings
                "//*[@id=\"confs_standings_E\"]/thead/tr/th"
            );
            var tableDataNodes = document.DocumentNode.SelectNodes( // Data
                "//*[@id=\"confs_standings_E\"]/tbody/tr"
            );

            string[] tableHeadings =
                tableHeadingsNodes != null
                    ? [..tableHeadingsNodes.Select(node => node.InnerText)] // segment into array for table output
                    : []; // Handle null case
            string[] tableData =
                tableDataNodes != null
                    ? [.. tableDataNodes.Select(node => node.InnerText)] // segment into array for table output
					: []; // Handle null case

            AnsiConsole.Write(
                new Rule("[bold italic blue]Basketball Results Scrape[/]")
                    .RuleStyle("blue")
                    .Centered()
            );

            Console.WriteLine(title);
            Console.WriteLine();
            Console.WriteLine(subHeading);

            Table table = new Table();
            foreach (var column in tableHeadings)
            {
                table.AddColumn(column);
            }
            foreach (var row in tableData)
            {
                table.AddRow(row);
            }
            AnsiConsole.Write(table);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            
        }
    }
}
