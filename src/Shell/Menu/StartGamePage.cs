using BrineAndCoin.Core;
using NetAF.Assets.Characters;
using NetAF.Logic;
using NetAF.Rendering.FrameBuilders;
using NetAF.Targets.Console;
using ObjectModel;
using Shell.Core;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Structs;
using Splat;

namespace BrineAndCoin;

public class StartGamePage : MenuPage
{
    public override string? Title => "Start Game";

    protected override void Render()
    {
        Console.Clear();

        var engine = new MiniAudioEngine();
        var playbackDevice = engine.InitializePlaybackDevice(null, AudioFormat.DvdHq);
        playbackDevice.Start();

        var writer = new TypeWriter(engine, playbackDevice);
        Locator.CurrentMutable.RegisterConstant(engine);
        Locator.CurrentMutable.RegisterConstant(playbackDevice);
        Locator.CurrentMutable.RegisterConstant(writer);

        //await writer.PlayAsync("Assets/Texts/intro.txt", "Assets/Voice/intro.mp3");

        InitAndExecuteGame();
    }

    private static PlayableCharacter CreatePlayer()
    {
        return new PlayableCharacter("Dave", "A young boy on a quest to find the meaning of life.");
    }

    private static EndCheckResult IsGameComplete(Game game)
    {
        if (!game.Player.FindItem("Holy Grail", out _))
            return EndCheckResult.NotEnded;

        return new EndCheckResult(true, "Game Complete", "You have the Holy Grail!");
    }

    private static EndCheckResult IsGameOver(Game game)
    {
        if (game.Player.IsAlive)
            return EndCheckResult.NotEnded;

        return new EndCheckResult(true, "Game Over", "You died!");
    }

    static void InitAndExecuteGame()
    {
        var world = GameAssetLoader.LoadFile();

        var gameCreator = Game.Create(
                        new GameInfo("Portraits in Brick and Time - Brine and Coin", "Brine and Coin is an open source text adventure where you experience the history of Schwäbisch Hall.", "Chris Anders"),
                        "",
                        AssetGenerator.Retained(world, CreatePlayer()),
                        new GameEndConditions(IsGameComplete, IsGameOver),
                        new GameConfiguration(new ConsoleAdapter(), FrameBuilderCollections.Console, new(90, 30), StartModes.Scene));

        GameExecutor.Execute(gameCreator, new ConsoleExecutionController());
    }
}
