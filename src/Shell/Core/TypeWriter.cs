using System.Text.RegularExpressions;
using SoundFlow.Abstracts;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Components;
using SoundFlow.Providers;

namespace Shell.Core;

public class TypeWriter
{
    private readonly int _baseDelay = 250;
    private int _delayModifier = 0;
    private AudioEngine _engine;
    private AudioPlaybackDevice _playbackDevice;

    public TypeWriter(AudioEngine engine, AudioPlaybackDevice playbackDevice)
    {
        _engine = engine;
        _playbackDevice = playbackDevice;
    }

    public async Task WriteAsync(string text, CancellationToken cancellationToken)
    {
        int i = 0;

        while (i < text.Length)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                // Skip delays and immediately display the rest of the text
                Console.Write(text[i..]);
                break;
            }

            var c = text[i];

            if (char.IsWhiteSpace(c))
            {
                Console.Write(c);
                i++;
                await Task.Delay(Math.Max(_baseDelay + _delayModifier, 0), cancellationToken);
                continue;
            }
            if (c is '.' or ',')
            {
                Console.Write(c);
                i++;
                await Task.Delay(_baseDelay * 3 + _delayModifier, cancellationToken);
                continue;
            }

            var match = Regex.Match(text[i..], @"^\[(-?\d+)\]");
            if (match.Success)
            {
                _delayModifier = int.Parse(match.Groups[1].Value);
                i += match.Length;
                if (text[i] == ' ') i++;
                continue;
            }

            Console.Write(c);
            i++;
        }
    }

    public async Task PlayAsync(string textFilename, string audioFilename)
    {
        var audio = File.OpenRead(audioFilename);
        var dataProvider = new StreamDataProvider(_engine, audio);
        var player = new SoundPlayer(_engine, _playbackDevice.Format, dataProvider);
        _playbackDevice.MasterMixer.AddComponent(player);
        player.Play();

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        player.PlaybackEnded += (sender, args) =>
        {
            _playbackDevice.MasterMixer.RemoveComponent(player);
            dataProvider.Dispose();
            audio.Dispose();
        };

        Task inputMonitor = Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!Console.IsInputRedirected && Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Spacebar)
                    {
                        cts.Cancel(); // Signal cancellation
                        player.Stop(); // Stop the audio playback
                    }
                }
            }
        }, cancellationToken);

        try
        {
            await WriteAsync(File.ReadAllText(textFilename), cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation (e.g., log or clean up if necessary)
        }
        finally
        {
            cts.Cancel(); // Ensure the input monitor task is canceled
            await inputMonitor; // Wait for the input monitor to finish
        }
    }
}
