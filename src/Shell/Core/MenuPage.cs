using NetAF.Logging.Events;
using Spectre.Console;

namespace BrineAndCoin.Core;

public abstract class MenuPage
{
    public MenuPage PreviousPage;
    protected virtual bool RenderTitle { get; } = true;

    public void Back()
    {
        if (PreviousPage is null) return;

        PreviousPage.Display();
    }

    public abstract string? Title { get; }

    protected abstract void Render();

    public void Display()
    {
        if (Title is not null && RenderTitle)
        {
            var panel = new Panel(Title);
            AnsiConsole.Write(panel);
        }

        Render();
        if (this is not StartNewGamePage)
        {
            Console.ReadLine();
        }
        Back();
    }
}