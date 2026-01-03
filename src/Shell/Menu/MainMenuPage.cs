using BrineAndCoin.Core;
using Spectre.Console;

namespace BrineAndCoin;

public class MainMenuPage : MenuPage
{
    public override string? Title => null;

    protected override void Render()
    {
        Console.Clear();

        AnsiConsole.Write(new Rule() { Border = BoxBorder.Double });
        var figlet = new FigletText("Brine and Coin") { Justification = Justify.Center };
        AnsiConsole.Write(figlet);
        AnsiConsole.Write(new Rule() { Border = BoxBorder.Double });

        var prompt = new SelectionPrompt<MenuPage>();

        prompt.AddChoice(new StartGamePage());
        prompt.AddChoice(new CreditsGamePage());
        prompt.AddChoice(new ExitGamePage());
        prompt.Converter = page => page.Title!;

        var selectedPage = AnsiConsole.Prompt(prompt);
        selectedPage.PreviousPage = this;
        selectedPage.Display();
    }
}
