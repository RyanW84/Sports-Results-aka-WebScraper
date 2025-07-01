using WebScraper_RyanW84.Service;
using WebScraper_RyanW84.UI;

namespace WebScraper_RyanW84;

public class WebScraper
{
	public static async Task Main(string[] args)
	{

		if (args[0] is "B" or "b")
		{
			BasketballScraper basketballScraper = new ();
			basketballScraper.Run();
		}



		Menu.DisplayMenu();
	}
}
