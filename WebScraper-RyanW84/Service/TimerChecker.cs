using Spectre.Console;

namespace WebScraper_RyanW84.Service
{
    public class TimerChecker
    {
        internal Timer checker;
        readonly BasketballScraper basketballScraper = new();

        private void TimerCallback(object? state)
        {
            basketballScraper.Run();
            SetTimer(); // Reschedule for the next day
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
                    50,
                    0
                );
                var span = due.Subtract(DateTime.Now);

                while (span.TotalMilliseconds < 0)
                {
                    due = due.AddDays(1);
                    span = due.Subtract(DateTime.Now);
                }

                AnsiConsole.Markup(
                    "...setting scraper to check at {0:dd MMM yyyy HH:mm} (~{1} minutes)\n",
                    due,
                    Math.Round(span.TotalMinutes, 0)
                );

                checker = new Timer(
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
