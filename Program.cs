﻿using Microsoft.Extensions.Configuration;
using WebScraper_RyanW84.Service;

namespace WebScraper_RyanW84;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Helpers helpers = new();
        // Example: args = ["basketball"] or ["halestorm"]
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .AddUserSecrets<Program>()
            .Build();

        IEmailService emailService = new Email(config);

        if (args.Length == 0)
            throw new ArgumentException(
                "Please specify which scraper to use: '-h' for Halestorm Gigs or '-b' for Basketball Results.");

        IScraper scraper = args[0].ToLowerInvariant() switch
        {
            "-h" => new HalestormScraper(helpers),
            "-b" => new BasketballScraper(helpers),
            _ => throw new ArgumentException("Invalid scraper selection. Use '-h' or '-b'.")
        };

        var timerChecker = new TimerChecker(config, scraper, emailService);
        await timerChecker.SetTimer();

        // Prevent app from exiting
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}