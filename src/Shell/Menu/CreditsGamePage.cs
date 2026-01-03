using BrineAndCoin.Core;

namespace BrineAndCoin;

public class CreditsGamePage : MenuPage
{
    public override string? Title => "Credits";

    protected override void Render()
    {
        Console.WriteLine("The Game is written by Chris Anders");
    }
}
