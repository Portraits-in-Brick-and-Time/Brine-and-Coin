using BrineAndCoin.Core;

namespace BrineAndCoin;

public class LoadGamePage : MenuPage
{
    public override string? Title => "Load Game";
    protected override bool RenderTitle { get; } = false;

    protected override void Render()
    {
        Console.Clear();

        var game = BrineAndCoinGame.Load();
        game.Execute();
    }
}
