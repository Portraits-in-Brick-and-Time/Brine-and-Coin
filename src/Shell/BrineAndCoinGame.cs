using System;
using BrineAndCoin.Core;
using BrineAndCoin.Core.Functions;
using BrineAndCoin.Core.Questing;
using NetAF.Commands.Persistence;
using NetAF.Interpretation;
using NetAF.Logging.Events;
using NetAF.Logic;
using NetAF.Logic.Modes;
using NetAF.Persistence;
using NetAF.Rendering.FrameBuilders;
using NetAF.Targets.Console;
using ObjectModel;
using ObjectModel.Evaluation;
using Shell.Core;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Structs;
using Splat;

namespace BrineAndCoin;

public class BrineAndCoinGame
{
    private GameCreator? _gameCreator;

    private BrineAndCoinGame()
    {
    }

    public static BrineAndCoinGame NewGame()
    {
        return new BrineAndCoinGame
        {
            _gameCreator = InitGame()
        };
    }

    public static BrineAndCoinGame Load()
    {
        return new BrineAndCoinGame
        {
            _gameCreator = InitGame(game =>
            {
                new Load(GameAssetLoader.GetSaveFileName().path).Invoke(game);
            })
        };
    }

    private static void RegisterServices()
    {
        var engine = new MiniAudioEngine();
        var playbackDevice = engine.InitializePlaybackDevice(null, AudioFormat.DvdHq);
        playbackDevice.Start();

        var writer = new TypeWriter(engine, playbackDevice);
        Locator.CurrentMutable.RegisterConstant(engine);
        Locator.CurrentMutable.RegisterConstant(playbackDevice);
        Locator.CurrentMutable.RegisterConstant(writer);

        var evaluator = new Evaluator();
        evaluator.RootScope.AddNativeFunction<SetTimeFunction>();

        Locator.CurrentMutable.RegisterConstant(evaluator);
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

    static GameCreator InitGame(Action<Game>? configure = null)
    {
        RegisterServices();

        var clock = new Clock();
        var questManager = new QuestManager();
        // Commands have to be added before the asset file is being loaded
        CommandStore.Add("clock.show", clock.CreateCommand());
        CommandStore.Add("quest.show", questManager.CreateCommand());

        var world = GameAssetLoader.LoadFile(out var players);

        var sceneInterpreter = new InputInterpreter
        (
            new FrameCommandInterpreter(),
            new GlobalCommandInterpreter(),
            new ExecutionCommandInterpreter(),
            new CustomCommandInterpreter(),
            new SceneCommandInterpreter()
        );

        var configuration = new GameConfiguration(new ConsoleAdapter(), FrameBuilderCollections.Console, new(90, 30), StartModes.Scene);

        // change configuration prevent using the normal persistence interpreter as this is handled by custom commands
        configuration.InterpreterProvider.Register(typeof(SceneMode), sceneInterpreter);

        var gameCreator = Game.Create(
                        new GameInfo("Portraits in Brick and Time - Brine and Coin", "Brine and Coin is an open source text adventure where you experience the history of Schwäbisch Hall.", "Chris Anders"),
                        "",
                        AssetGenerator.Retained(world, players[0]),
                        new GameEndConditions(IsGameComplete, IsGameOver),
                        configuration,
                        game =>
                        {
                            Locator.CurrentMutable.RegisterConstant(game);

                            clock.Init(DateTime.Now);
                            questManager.Init();
                            Locator.CurrentMutable.RegisterConstant(clock);

                            configure?.Invoke(game);
                        });

        return gameCreator;
    }

    public void Execute()
    {
        GameExecutor.Execute(_gameCreator, new ConsoleExecutionController());
    }
}
