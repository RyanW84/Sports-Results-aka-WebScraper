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
    IEmailService emailService)
{
    private readonly IConfiguration _configuration = configuration;
    internal Timer Checker;

    private async void TimerCallback(object state)
    {
        try
        {
            var results = await scraper.Run();
            if (results != null)
                await emailService.SendEmail(results);
            else
                AnsiConsole.MarkupLine("[Bold  Red]Scraper returned null results[/]");

            await SetTimer();
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
            var due = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second
            );
            var span = due.Subtract(DateTime.Now);

            while (span.TotalMilliseconds < 0)
            {
                due = due.AddDays(1);
                span = due.Subtract(DateTime.Now);
            }

            AnsiConsole.MarkupLine(
                "[Bold Italic Green]Setting scraper to check at {0:dd MMM yyyy HH:mm} (~{1} minutes)[/]",
                due,
                Math.Round(span.TotalMinutes, 0)
            );

            Checker = new Timer(
                TimerCallback,
                null,
                (int)span.TotalMinutes,
                Timeout.Infinite
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
        }

        return Task.CompletedTask;
    }
}