using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

using HtmlAgilityPack;

namespace WebScraper_RyanW84
{
    public class WebScraper
    {
        public static async Task Main(string[] args)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://www.basketball-reference.com/boxscores/");

            var title = document.DocumentNode.SelectNodes("//div/h1").First().InnerText;
            var tableoutput = document.DocumentNode.SelectNodes("//*[@id=\"confs_standings_E\"]/tbody/tr");

			Console.WriteLine(title);
			Console.WriteLine();
			tableoutput.ToList().ForEach(x => Console.WriteLine(x.InnerText));

        }
    }
}
