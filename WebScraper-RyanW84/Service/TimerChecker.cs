using Spectre.Console;

namespace WebScraper_RyanW84.Service
{
    public class TimerChecker
    {
        internal Timer checker;
        readonly BasketballScraper basketballScraper = new ();

        public void SetTimer()
        {
            try
            {
                var due = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    21,
                    30,
                    0
                );
                var span = due.Subtract(DateTime.Now);

                while (span.TotalMilliseconds < 0)
                {
                    due = due.AddDays(1);
                    span = due.Subtract(DateTime.Now);
                }

                AnsiConsole.Markup(
                    "...setting scraper to check at {0:dd MMM yyyy HH:mm} (~{1} minutes)",
                    due,
                    Math.Round(span.TotalMinutes, 0)
                );

                checker = new Timer(
                    ob => basketballScraper.Run(),
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
