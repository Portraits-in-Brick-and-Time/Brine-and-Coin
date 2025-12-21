using NetAF.Assets.Characters;
using NetAF.Assets.Locations;
using NetAF.Logic;
using NetAF.Rendering.FrameBuilders;
using NetAF.Targets.Console;
using NetAF.Utilities;
using Shell.Core;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Structs;
using Spectre.Console;
using Splat;
using Velopack;
using Velopack.Sources;

namespace BrineAndCoin;

public class Program
{
    public static async Task Main(string[] args)
    {
        VelopackApp
                   .Build()
#if WINDOWS
                       .OnAfterInstallFastCallback(v => new Shortcuts().CreateShortcutForThisExe(ShortcutLocation.Desktop))
#endif
                   .Run();

#if RELEASE
                   CheckForUpdatesAsync();
#endif

        var engine = new MiniAudioEngine();
        var playbackDevice = engine.InitializePlaybackDevice(null, AudioFormat.DvdHq);
        playbackDevice.Start();

        Locator.CurrentMutable.RegisterConstant<MiniAudioEngine>(engine);
        Locator.CurrentMutable.RegisterConstant<AudioPlaybackDevice>(playbackDevice);
        Locator.CurrentMutable.Register<TypeWriter>(() => new TypeWriter(engine, playbackDevice));

        var writer = Locator.Current.GetService<TypeWriter>()!;
        await writer.PlayAsync("Assets/Texts/intro.txt", "Assets/Voice/intro.mp3");

        Console.Clear();
        HelloWorldGame();
        Console.Clear();
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
    
    static void HelloWorldGame()
    {
        Console.Clear();
        // create region maker. the region maker simplifies creating in game regions. a region contains a series of rooms
        var regionMaker = new RegionMaker("Mountain", "An imposing volcano just East of town.")
        {
            // add a room to the region at position x 0, y 0, z 0
            [0, 0, 0] = new Room("Cavern", "A dark cavern set in to the base of the mountain.")
        };
        
        // create overworld maker. the overworld maker simplifies creating in game overworlds. an overworld contains a series or regions
        var overworldMaker = new OverworldMaker("Daves World", "An ancient kingdom.", regionMaker);
        
        // create the callback for generating new instances of the game
        // - information about the game
        // - an introduction to the game, displayed at the star
        // - asset generation for the overworld and the player
        // - the conditions that end the game
        // - the configuration for the game
        var gameCreator = Game.Create(
                        new GameInfo("Portraits in Brick and Time - Brine and Coin", "Brine and Coin is an open source text adventure where you experience the history of Schwäbisch Hall.", "Chris Anders"),
                        "",
                        AssetGenerator.Custom(overworldMaker.Make, CreatePlayer),
                        new GameEndConditions(IsGameComplete, IsGameOver),
                        new GameConfiguration(new ConsoleAdapter(), FrameBuilderCollections.Console, new(90, 30)));
        
        GameExecutor.Execute(gameCreator, new ConsoleExecutionController());
    }

    private static void CheckForUpdatesAsync()
    {
        AnsiConsole.Progress()
            .Start(async ctx => 
            {
                var mgr = new UpdateManager(new GithubSource("https://github.com/Portraits-in-Brick-and-Time/Brine-and-Coin", null, false));
        
                var newVersion = await mgr.CheckForUpdatesAsync();
                if (newVersion == null)
                    return;
                    
                var task1 = ctx.AddTask("Downloading Updates");
    
                await mgr.DownloadUpdatesAsync(newVersion, progress =>
                {
                    task1.Increment(progress);
                });
        
                mgr.ApplyUpdatesAndRestart(newVersion);
            });
    }
}