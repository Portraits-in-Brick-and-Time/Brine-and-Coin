using BrineAndCoin.Core;

namespace BrineAndCoin;

public class ExitGamePage : MenuPage
{
    public override string? Title => "Exit";

    protected override void Render()
    {
        Environment.Exit(0);
    }
}
