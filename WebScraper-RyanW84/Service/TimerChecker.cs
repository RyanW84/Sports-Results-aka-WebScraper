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

public class TimerChecker
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IScraper _scraper;
    internal Timer Checker;

    public TimerChecker(
        IConfiguration configuration,
        IScraper scraper,
        IEmailService emailService)
    {
        _configuration = configuration;
        _scraper = scraper;
        _emailService = emailService;
    }

    private void TimerCallback(object state)
    {
        Task.Run(async () =>
        {
            try
            {
                var results = await _scraper.Run();
                await _emailService.SendEmail(results);
                await SetTimer();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }).ContinueWith(t =>
        {
            if (t.IsFaulted) Console.WriteLine($"Background task error: {t.Exception}");
        }, TaskScheduler.Current);
    }

    public async Task SetTimer()
    {
        try
        {
            var due = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                22, 04, 0
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
                (long)span.TotalMilliseconds,
                Timeout.Infinite
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
        }
    }
}