using Spectre.Console;

namespace WebScraper_RyanW84.UI;

public static class Menu
{
    internal static bool display = true;

    public static void DisplayMenu()
    {
        while (display)
        {
            AnsiConsole.Write(
                new Rule("[bold italic blue]Basketball Results Scraper[/]")
                    .RuleStyle("blue")
                    .Centered()
            );

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Please select an option[/]")
                    .AddChoices("Basketball Results Scraper", "Custom Scraper", "Exit")
            );

            switch (choice)
            {
                case "Basketball Results Scraper":
                    //
                    break;
                case "Custom Scraper":
                    //
                    break;
                case "Exit":
                    display = false;
                    break;
                default:
                    AnsiConsole.MarkupLine("[Red]Please make a valid choice[/]");
                    break;
            }
        }
    }
}