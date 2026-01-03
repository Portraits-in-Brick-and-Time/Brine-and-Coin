using Spectre.Console;

namespace BrineAndCoin.Core;

public abstract class MenuPage
{
    public MenuPage PreviousPage;

    public void Back()
    {
        if (PreviousPage is null) return;

        PreviousPage.Display();
    }

    public abstract string? Title { get; }

    protected abstract void Render();

    public void Display()
    {
        if (Title is not null)
        {
            var panel = new Panel(Title);
            AnsiConsole.Write(panel);
        }

        Render();
        Console.ReadLine();
        Back();
    }
}