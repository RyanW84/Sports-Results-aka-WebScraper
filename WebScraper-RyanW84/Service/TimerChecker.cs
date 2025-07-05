using Microsoft.Extensions.Configuration;
namespace WebScraper_RyanW84.Service
{
    public interface IEmailService
    {
        Task SendEmail(BasketballResults results);
    }

    public class TimerChecker
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly BasketballScraper _basketballScraper;
        internal Timer _checker;

        public TimerChecker(
            IConfiguration configuration,
            BasketballScraper basketballScraper,
            IEmailService emailService)
        {
            _configuration = configuration;
            _basketballScraper = basketballScraper;
            _emailService = emailService;
        }

        private void TimerCallback(object state)
        {
            // Create a Task to handle the async work
            Task.Run(async () =>
            {
                try
                {
                    var results = await _basketballScraper.Run();
                    await SendBasketballResults(results);
                    await SetTimer();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                }
            }).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Console.WriteLine($"Background task error: {t.Exception}");
                }
            }, TaskScheduler.Current);
        }

        private async Task SendBasketballResults(BasketballResults results)
        {
            await _emailService.SendEmail(results);
        }

        public async Task SetTimer()
        {
            try
            {
                var due = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    21,
                    27,
                    0
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

                _checker = new Timer(
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
}