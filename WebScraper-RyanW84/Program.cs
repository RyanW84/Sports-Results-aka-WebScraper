using Microsoft.Extensions.Configuration;
using Spectre.Console;
using WebScraper_RyanW84.Service;

namespace WebScraper_RyanW84;

public class WebScraper
{
    public static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddUserSecrets<WebScraper>()
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var basketballScraper = new BasketballScraper();
        var emailService = new Email(configuration); // Create an instance of Email service
        var timerChecker = new TimerChecker(configuration, basketballScraper, emailService);

        if (args.Length > 0 && args[0] is "-B" or "-b")
        {
            AnsiConsole.MarkupLine("[Blue]Basketball Scraper selected by Args[/]");
            await timerChecker.SetTimer();
            await Task.Delay(Timeout.Infinite);
        }
        else if (args.Length > 0 && args[0] is "-H" or "-h")
        {
            AnsiConsole.MarkupLine("[Blue]Halestorm Scraper selected by Args[/]");
            await timerChecker.SetTimer();
            await Task.Delay(Timeout.Infinite);
        }
        else
        {
            AnsiConsole.MarkupLine("[Red]No Scraper selected by Args/]");
        }
    }
}