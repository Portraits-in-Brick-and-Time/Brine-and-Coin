using BrineAndCoin.Core;
using Spectre.Console;
using Splat;

namespace BrineAndCoin;

public class MainMenuPage : MenuPage
{
    public override string? Title => null;

    protected override void Render()
    {
        Console.Clear();

        AnsiConsole.Write(new Rule() { Border = BoxBorder.Ascii });
        var font = FigletFont.Load("Assets/font.flf");
        Locator.CurrentMutable.RegisterConstant(font);
        var figlet = new FigletText(font, "Brine and Coin") { Justification = Justify.Center };
        AnsiConsole.Write(figlet);
        AnsiConsole.Write(new Rule() { Border = BoxBorder.Ascii });
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
