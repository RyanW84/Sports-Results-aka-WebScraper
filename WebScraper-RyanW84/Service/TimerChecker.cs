using Microsoft.Extensions.Configuration;

using Spectre.Console;

namespace WebScraper_RyanW84.Service
{
	public class TimerChecker
	{
		private readonly IConfiguration configuration;
		internal Timer checker;
		internal readonly BasketballScraper basketballScraper;

		public TimerChecker(IConfiguration configuration , BasketballScraper basketballScraper)
		{
			this.configuration = configuration;
			this.basketballScraper = basketballScraper;
		}

		private async void TimerCallback(object state)
		{
			try
			{
				var results = await basketballScraper.Run(); // Await the Task to get BasketballResults
				var email = new Email(configuration);
				await email.SendEmail(results); // Pass the resolved BasketballResults object
				await SetTimer(); // Reschedule for the next day
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex}");
			}
		}

		public async Task SetTimer( )
		{
			try
			{
				var due = new DateTime(
					DateTime.Now.Year ,
					DateTime.Now.Month ,
					DateTime.Now.Day ,
					22 ,
					11 ,
					0
				);
				var span = due.Subtract(DateTime.Now);

				while (span.TotalMilliseconds < 0)
				{
					due = due.AddDays(1);
					span = due.Subtract(DateTime.Now);
				}

				AnsiConsole.MarkupLine(
					"[Bold Italic Green]Setting scraper to check at {0:dd MMM yyyy HH:mm} (~{1} minutes)[/]" ,
					due ,
					Math.Round(span.TotalMinutes , 0)
				);

				checker = new Timer(
					TimerCallback ,
					null ,
					(long)span.TotalMilliseconds ,
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
