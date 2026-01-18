using BrineAndCoin.Core;
using NetAF.Interpretation;
using NetAF.Logic;
using NetAF.Logic.Modes;
using NetAF.Rendering.FrameBuilders;
using NetAF.Targets.Console;
using ObjectModel;
using ObjectModel.Evaluation;
using Shell.Core;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Editing.Mapping;
using SoundFlow.Structs;
using Splat;

namespace BrineAndCoin;

public class StartNewGamePage : MenuPage
{
    public override string? Title => "New Game";
    protected override bool RenderTitle { get; } = false;

    protected override void Render()
    {
        Console.Clear();

        //await writer.PlayAsync("Assets/Texts/intro.txt", "Assets/Voice/intro.mp3");

        var game = BrineAndCoinGame.NewGame();
        game.Execute();
    }
}
