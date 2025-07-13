using Microsoft.Extensions.Configuration;
using Spectre.Console;
using WebScraper_RyanW84.Models;

namespace WebScraper_RyanW84.Service;

public interface IScraper
{
    Task<Results> Run();
}

public interface IEmailService
{
    Task SendEmail(Results results);
}

public class TimerChecker(
    IConfiguration configuration,
    IScraper scraper,
    IEmailService emailService
)
{
    private readonly IConfiguration _configuration = configuration;
    internal Timer Checker;

    private async void TimerCallback(object state)
    {
        try
        {
            var results = await scraper.Run();
            if (results.EmailTableRows.Length is not 0)
            {
                await emailService.SendEmail(results);
                // Don't run again if successful
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Scraper returned null results, trying again[/]");
                Thread.Sleep(2000);  // Sleep for 2 seconds
                await scraper.Run(); // Only run again if the first attempt failed
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
        }
    }

    public Task SetTimer()
    {
        try
        {
            // Dispose existing timer if any
            Checker?.Dispose();

            // Set initial run to 10 seconds from now
            var due = DateTime.Now.AddSeconds(10);
            var span = due.Subtract(DateTime.Now);

            while (span.TotalMilliseconds < 0)
            {
                due = due.AddMinutes(1);
                span = due.Subtract(DateTime.Now);
            }

            AnsiConsole.MarkupLine(
                "[Bold Italic Green]Setting scraper to check at {0:dd MMM yyyy HH:mm} (~{1} minutes)[/]",
                due,
                Math.Round(span.TotalMinutes, 0)
            );

            // Create new timer with initial delay and 1 hour interval
            Checker = new Timer(
                TimerCallback, 
                null, 
                (int)span.TotalMilliseconds,
                TimeSpan.FromHours(1).Milliseconds
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
        }

        return Task.CompletedTask;
    }
}
