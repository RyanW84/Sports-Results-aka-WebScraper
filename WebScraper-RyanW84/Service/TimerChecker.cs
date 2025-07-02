using Spectre.Console;

namespace WebScraper_RyanW84.Service
{
	public class TimerChecker
	{
		internal Timer checker;
		readonly BasketballScraper basketballScraper = new();

		private async void TimerCallback(object? state) // Changed return type to 'void' to match TimerCallback delegate signature
		{
			try
			{
				var results = await basketballScraper.Run(); // Await the Task to get BasketballResults
					var email = new Email(); // Create an instance of Email
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
					14 ,
					 07,
					0
				);
				var span = due.Subtract(DateTime.Now);

				while (span.TotalMilliseconds < 0)
				{
					due = due.AddDays(1);
					span = due.Subtract(DateTime.Now);
				}

				AnsiConsole.Markup(
					"...setting scraper to check at {0:dd MMM yyyy HH:mm} (~{1} minutes)\n" ,
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
