using Spectre.Console;

using WebScraper_RyanW84.Service;
using WebScraper_RyanW84.UI;

namespace WebScraper_RyanW84;

public class WebScraper
{
	public static async Task Main(string[] args)
	{
		// Create an instance of TimerChecker to call the non-static method SetTimer  
		var timerChecker = new TimerChecker();

		if (args.Length > 0 && (args[0] is "-B" or "-b"))
		{
			AnsiConsole.MarkupLine("[Blue] running Basketball scraper as requested by args -B[/]");
			await timerChecker.SetTimer();
			await Task.Delay(Timeout.Infinite);
		}

		//Menu.DisplayMenu();
	}
}
