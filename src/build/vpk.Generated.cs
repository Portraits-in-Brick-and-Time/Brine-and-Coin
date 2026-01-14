
using JetBrains.Annotations;
using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools;
using Nuke.Common.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;


/// <summary><p>Velopack creates installers and auto-update packages for cross-platform desktop apps.</p><p>For more details, visit the <a href="https://docs.velopack.io/">official website</a>.</p></summary>
[PublicAPI]
[ExcludeFromCodeCoverage]
[NuGetTool(Id = PackageId, Executable = PackageExecutable)]
public partial class VelopackTasks : ToolTasks, IRequireNuGetPackage
{
    public static string VelopackPath { get => new VelopackTasks().GetToolPathInternal(); set => new VelopackTasks().SetToolPath(value); }
    public const string PackageId = "vpk";
    public const string PackageExecutable = "vpk.dll|vpk.exe";
    /// <summary><p>Velopack creates installers and auto-update packages for cross-platform desktop apps.</p><p>For more details, visit the <a href="https://docs.velopack.io/">official website</a>.</p></summary>
    public static IReadOnlyCollection<Output> Velopack(ArgumentStringHandler arguments, string workingDirectory = null, IReadOnlyDictionary<string, string> environmentVariables = null, int? timeout = null, bool? logOutput = null, bool? logInvocation = null, Action<OutputType, string> logger = null, Func<IProcess, object> exitHandler = null) => new VelopackTasks().Run(arguments, workingDirectory, environmentVariables, timeout, logOutput, logInvocation, logger, exitHandler);
    /// <summary><p>Creates platform release packages (`vpk [windows|linux] pack`).</p><p>For more details, visit the <a href="https://docs.velopack.io/reference/cli">official website</a>.</p></summary>
    /// <remarks><p>This is a <a href="https://www.nuke.build/docs/common/cli-tools/#fluent-api">CLI wrapper with fluent API</a> that allows to modify the following arguments:</p><ul><li><c>--skip-updates</c> via <see cref="VelopackPackSettings.SkipUpdates"/></li><li><c>--verbose</c> via <see cref="VelopackPackSettings.Verbose"/></li><li><c>-e</c> via <see cref="VelopackPackSettings.MainExe"/></li><li><c>-o</c> via <see cref="VelopackPackSettings.OutputDir"/></li><li><c>-p</c> via <see cref="VelopackPackSettings.PackDir"/></li><li><c>-r</c> via <see cref="VelopackPackSettings.Runtime"/></li><li><c>-u</c> via <see cref="VelopackPackSettings.PackId"/></li><li><c>-v</c> via <see cref="VelopackPackSettings.PackVersion"/></li><li><c>-x</c> via <see cref="VelopackPackSettings.LegacyConsole"/></li><li><c>-y</c> via <see cref="VelopackPackSettings.Yes"/></li><li><c>[</c> via <see cref="VelopackPackSettings.Channel"/></li></ul></remarks>
    public static IReadOnlyCollection<Output> VelopackPack(VelopackPackSettings options = null) => new VelopackTasks().Run<VelopackPackSettings>(options);
    /// <inheritdoc cref="VelopackTasks.VelopackPack(.VelopackPackSettings)"/>
    public static IReadOnlyCollection<Output> VelopackPack(Configure<VelopackPackSettings> configurator) => new VelopackTasks().Run<VelopackPackSettings>(configurator.Invoke(new VelopackPackSettings()));
    /// <inheritdoc cref="VelopackTasks.VelopackPack(.VelopackPackSettings)"/>
    public static IEnumerable<(VelopackPackSettings Settings, IReadOnlyCollection<Output> Output)> VelopackPack(CombinatorialConfigure<VelopackPackSettings> configurator, int degreeOfParallelism = 1, bool completeOnFailure = false) => configurator.Invoke(VelopackPack, degreeOfParallelism, completeOnFailure);
    /// <summary><p>Downloads latest release from GitHub (`vpk [windows|linux] download github`).</p><p>For more details, visit the <a href="https://docs.velopack.io/reference/cli">official website</a>.</p></summary>
    /// <remarks><p>This is a <a href="https://www.nuke.build/docs/common/cli-tools/#fluent-api">CLI wrapper with fluent API</a> that allows to modify the following arguments:</p><ul><li><c>--pre</c> via <see cref="VelopackDownloadGithubSettings.Pre"/></li><li><c>--repoUrl</c> via <see cref="VelopackDownloadGithubSettings.RepoUrl"/></li><li><c>--skip-updates</c> via <see cref="VelopackDownloadGithubSettings.SkipUpdates"/></li><li><c>--token</c> via <see cref="VelopackDownloadGithubSettings.Token"/></li><li><c>--verbose</c> via <see cref="VelopackDownloadGithubSettings.Verbose"/></li><li><c>-o</c> via <see cref="VelopackDownloadGithubSettings.OutputDir"/></li><li><c>-x</c> via <see cref="VelopackDownloadGithubSettings.LegacyConsole"/></li><li><c>-y</c> via <see cref="VelopackDownloadGithubSettings.Yes"/></li><li><c>[</c> via <see cref="VelopackDownloadGithubSettings.Channel"/></li></ul></remarks>
    public static IReadOnlyCollection<Output> VelopackDownloadGithub(VelopackDownloadGithubSettings options = null) => new VelopackTasks().Run<VelopackDownloadGithubSettings>(options);
    /// <inheritdoc cref="VelopackTasks.VelopackDownloadGithub(.VelopackDownloadGithubSettings)"/>
    public static IReadOnlyCollection<Output> VelopackDownloadGithub(Configure<VelopackDownloadGithubSettings> configurator) => new VelopackTasks().Run<VelopackDownloadGithubSettings>(configurator.Invoke(new VelopackDownloadGithubSettings()));
    /// <inheritdoc cref="VelopackTasks.VelopackDownloadGithub(.VelopackDownloadGithubSettings)"/>
    public static IEnumerable<(VelopackDownloadGithubSettings Settings, IReadOnlyCollection<Output> Output)> VelopackDownloadGithub(CombinatorialConfigure<VelopackDownloadGithubSettings> configurator, int degreeOfParallelism = 1, bool completeOnFailure = false) => configurator.Invoke(VelopackDownloadGithub, degreeOfParallelism, completeOnFailure);
    /// <summary><p>Generates delta patch (`vpk [windows|linux] delta generate`).</p><p>For more details, visit the <a href="https://docs.velopack.io/">official website</a>.</p></summary>
    /// <remarks><p>This is a <a href="https://www.nuke.build/docs/common/cli-tools/#fluent-api">CLI wrapper with fluent API</a> that allows to modify the following arguments:</p><ul><li><c>--skip-updates</c> via <see cref="VelopackDeltaGenerateSettings.SkipUpdates"/></li><li><c>--verbose</c> via <see cref="VelopackDeltaGenerateSettings.Verbose"/></li><li><c>-b</c> via <see cref="VelopackDeltaGenerateSettings.Base"/></li><li><c>-n</c> via <see cref="VelopackDeltaGenerateSettings.New"/></li><li><c>-o</c> via <see cref="VelopackDeltaGenerateSettings.Output"/></li><li><c>-x</c> via <see cref="VelopackDeltaGenerateSettings.LegacyConsole"/></li><li><c>-y</c> via <see cref="VelopackDeltaGenerateSettings.Yes"/></li><li><c>[</c> via <see cref="VelopackDeltaGenerateSettings.Channel"/></li></ul></remarks>
    public static IReadOnlyCollection<Output> VelopackDeltaGenerate(VelopackDeltaGenerateSettings options = null) => new VelopackTasks().Run<VelopackDeltaGenerateSettings>(options);
    /// <inheritdoc cref="VelopackTasks.VelopackDeltaGenerate(.VelopackDeltaGenerateSettings)"/>
    public static IReadOnlyCollection<Output> VelopackDeltaGenerate(Configure<VelopackDeltaGenerateSettings> configurator) => new VelopackTasks().Run<VelopackDeltaGenerateSettings>(configurator.Invoke(new VelopackDeltaGenerateSettings()));
    /// <inheritdoc cref="VelopackTasks.VelopackDeltaGenerate(.VelopackDeltaGenerateSettings)"/>
    public static IEnumerable<(VelopackDeltaGenerateSettings Settings, IReadOnlyCollection<Output> Output)> VelopackDeltaGenerate(CombinatorialConfigure<VelopackDeltaGenerateSettings> configurator, int degreeOfParallelism = 1, bool completeOnFailure = false) => configurator.Invoke(VelopackDeltaGenerate, degreeOfParallelism, completeOnFailure);
    /// <summary><p>Uploads to GitHub (`vpk [windows|linux] upload github`).</p><p>For more details, visit the <a href="https://docs.velopack.io/">official website</a>.</p></summary>
    /// <remarks><p>This is a <a href="https://www.nuke.build/docs/common/cli-tools/#fluent-api">CLI wrapper with fluent API</a> that allows to modify the following arguments:</p><ul><li><c>--publish</c> via <see cref="VelopackUploadGithubSettings.Publish"/></li><li><c>--repoUrl</c> via <see cref="VelopackUploadGithubSettings.RepoUrl"/></li><li><c>--skip-updates</c> via <see cref="VelopackUploadGithubSettings.SkipUpdates"/></li><li><c>--token</c> via <see cref="VelopackUploadGithubSettings.Token"/></li><li><c>--verbose</c> via <see cref="VelopackUploadGithubSettings.Verbose"/></li><li><c>-o</c> via <see cref="VelopackUploadGithubSettings.OutputDir"/></li><li><c>-x</c> via <see cref="VelopackUploadGithubSettings.LegacyConsole"/></li><li><c>-y</c> via <see cref="VelopackUploadGithubSettings.Yes"/></li><li><c>[</c> via <see cref="VelopackUploadGithubSettings.Channel"/></li></ul></remarks>
    public static IReadOnlyCollection<Output> VelopackUploadGithub(VelopackUploadGithubSettings options = null) => new VelopackTasks().Run<VelopackUploadGithubSettings>(options);
    /// <inheritdoc cref="VelopackTasks.VelopackUploadGithub(.VelopackUploadGithubSettings)"/>
    public static IReadOnlyCollection<Output> VelopackUploadGithub(Configure<VelopackUploadGithubSettings> configurator) => new VelopackTasks().Run<VelopackUploadGithubSettings>(configurator.Invoke(new VelopackUploadGithubSettings()));
    /// <inheritdoc cref="VelopackTasks.VelopackUploadGithub(.VelopackUploadGithubSettings)"/>
    public static IEnumerable<(VelopackUploadGithubSettings Settings, IReadOnlyCollection<Output> Output)> VelopackUploadGithub(CombinatorialConfigure<VelopackUploadGithubSettings> configurator, int degreeOfParallelism = 1, bool completeOnFailure = false) => configurator.Invoke(VelopackUploadGithub, degreeOfParallelism, completeOnFailure);
}
#region VelopackPackSettings
/// <inheritdoc cref="VelopackTasks.VelopackPack(.VelopackPackSettings)"/>
[PublicAPI]
[ExcludeFromCodeCoverage]
[Command(Type = typeof(VelopackTasks), Command = nameof(VelopackTasks.VelopackPack), Arguments = "pack")]
public partial class VelopackPackSettings : ToolOptions
{
    /// <summary>Platform channel: 'windows' or 'linux'</summary>
    [Argument(Format = "[{value}]")] public string Channel => Get<string>(() => Channel);
    /// <summary>App bundle ID</summary>
    [Argument(Format = "-u={value}")] public string PackId => Get<string>(() => PackId);
    /// <summary>App version</summary>
    [Argument(Format = "-v={value}")] public string PackVersion => Get<string>(() => PackVersion);
    /// <summary>App files directory</summary>
    [Argument(Format = "-p={value}", Position = 1)] public string PackDir => Get<string>(() => PackDir);
    /// <summary>Output directory</summary>
    [Argument(Format = "-o={value}")] public string OutputDir => Get<string>(() => OutputDir);
    /// <summary>Target runtime</summary>
    [Argument(Format = "-r={value}")] public string Runtime => Get<string>(() => Runtime);
    /// <summary>Main executable name</summary>
    [Argument(Format = "-e={value}")] public string MainExe => Get<string>(() => MainExe);
    /// <summary>Print diagnostic messages</summary>
    [Argument(Format = "--verbose")] public bool? Verbose => Get<bool?>(() => Verbose);
    /// <summary>Auto-confirm prompts</summary>
    [Argument(Format = "-y")] public bool? Yes => Get<bool?>(() => Yes);
    /// <summary>Disable colors</summary>
    [Argument(Format = "-x")] public bool? LegacyConsole => Get<bool?>(() => LegacyConsole);
    /// <summary>Skip vpk updates</summary>
    [Argument(Format = "--skip-updates")] public bool? SkipUpdates => Get<bool?>(() => SkipUpdates);
}
#endregion
#region VelopackDownloadGithubSettings
/// <inheritdoc cref="VelopackTasks.VelopackDownloadGithub(.VelopackDownloadGithubSettings)"/>
[PublicAPI]
[ExcludeFromCodeCoverage]
[Command(Type = typeof(VelopackTasks), Command = nameof(VelopackTasks.VelopackDownloadGithub), Arguments = "download github")]
public partial class VelopackDownloadGithubSettings : ToolOptions
{
    /// <summary>Platform channel: 'windows' or 'linux'</summary>
    [Argument(Format = "[{value}]")] public string Channel => Get<string>(() => Channel);
    /// <summary>GitHub repository URL (e.g. https://github.com/{GitHubActions.Instance.Repository})</summary>
    [Argument(Format = "--repoUrl={value}")] public string RepoUrl => Get<string>(() => RepoUrl);
    /// <summary>GitHub OAuth token (e.g. {GitHubToken})</summary>
    [Argument(Format = "--token={value}", Secret = true)] public string Token => Get<string>(() => Token);
    /// <summary>Output directory [default: Releases]</summary>
    [Argument(Format = "-o={value}")] public string OutputDir => Get<string>(() => OutputDir);
    /// <summary>Get latest pre-release instead of stable</summary>
    [Argument(Format = "--pre")] public bool? Pre => Get<bool?>(() => Pre);
    /// <summary>Print diagnostic messages</summary>
    [Argument(Format = "--verbose")] public bool? Verbose => Get<bool?>(() => Verbose);
    /// <summary>Auto-confirm prompts</summary>
    [Argument(Format = "-y")] public bool? Yes => Get<bool?>(() => Yes);
    /// <summary>Disable colors</summary>
    [Argument(Format = "-x")] public bool? LegacyConsole => Get<bool?>(() => LegacyConsole);
    /// <summary>Skip vpk updates</summary>
    [Argument(Format = "--skip-updates")] public bool? SkipUpdates => Get<bool?>(() => SkipUpdates);
}
#endregion
#region VelopackDeltaGenerateSettings
/// <inheritdoc cref="VelopackTasks.VelopackDeltaGenerate(.VelopackDeltaGenerateSettings)"/>
[PublicAPI]
[ExcludeFromCodeCoverage]
[Command(Type = typeof(VelopackTasks), Command = nameof(VelopackTasks.VelopackDeltaGenerate), Arguments = "delta generate")]
public partial class VelopackDeltaGenerateSettings : ToolOptions
{
    /// <summary>Platform channel: 'windows' or 'linux'</summary>
    [Argument(Format = "[{value}]")] public string Channel => Get<string>(() => Channel);
    /// <summary>Base package</summary>
    [Argument(Format = "-b={value}", Position = 1)] public string Base => Get<string>(() => Base);
    /// <summary>New package</summary>
    [Argument(Format = "-n={value}")] public string New => Get<string>(() => New);
    /// <summary>Output path</summary>
    [Argument(Format = "-o={value}")] public string Output => Get<string>(() => Output);
    /// <summary>Print diagnostic messages</summary>
    [Argument(Format = "--verbose")] public bool? Verbose => Get<bool?>(() => Verbose);
    /// <summary>Auto-confirm prompts</summary>
    [Argument(Format = "-y")] public bool? Yes => Get<bool?>(() => Yes);
    /// <summary>Disable colors</summary>
    [Argument(Format = "-x")] public bool? LegacyConsole => Get<bool?>(() => LegacyConsole);
    /// <summary>Skip vpk updates</summary>
    [Argument(Format = "--skip-updates")] public bool? SkipUpdates => Get<bool?>(() => SkipUpdates);
}
#endregion
#region VelopackUploadGithubSettings
/// <inheritdoc cref="VelopackTasks.VelopackUploadGithub(.VelopackUploadGithubSettings)"/>
[PublicAPI]
[ExcludeFromCodeCoverage]
[Command(Type = typeof(VelopackTasks), Command = nameof(VelopackTasks.VelopackUploadGithub), Arguments = "upload github")]
public partial class VelopackUploadGithubSettings : ToolOptions
{
    /// <summary>Platform channel: 'windows' or 'linux'</summary>
    [Argument(Format = "[{value}]")] public string Channel => Get<string>(() => Channel);
    /// <summary>GitHub repository URL</summary>
    [Argument(Format = "--repoUrl={value}")] public string RepoUrl => Get<string>(() => RepoUrl);
    /// <summary>GitHub OAuth token</summary>
    [Argument(Format = "--token={value}", Secret = true)] public string Token => Get<string>(() => Token);
    /// <summary>Packages directory</summary>
    [Argument(Format = "-o={value}", Position = 1)] public string OutputDir => Get<string>(() => OutputDir);
    /// <summary>Publish the release</summary>
    [Argument(Format = "--publish")] public bool? Publish => Get<bool?>(() => Publish);
    /// <summary>Print diagnostic messages</summary>
    [Argument(Format = "--verbose")] public bool? Verbose => Get<bool?>(() => Verbose);
    /// <summary>Auto-confirm prompts</summary>
    [Argument(Format = "-y")] public bool? Yes => Get<bool?>(() => Yes);
    /// <summary>Disable colors</summary>
    [Argument(Format = "-x")] public bool? LegacyConsole => Get<bool?>(() => LegacyConsole);
    /// <summary>Skip vpk updates</summary>
    [Argument(Format = "--skip-updates")] public bool? SkipUpdates => Get<bool?>(() => SkipUpdates);
}
#endregion
#region VelopackPackSettingsExtensions
/// <inheritdoc cref="VelopackTasks.VelopackPack(.VelopackPackSettings)"/>
[PublicAPI]
[ExcludeFromCodeCoverage]
public static partial class VelopackPackSettingsExtensions
{
    #region Channel
    /// <inheritdoc cref="VelopackPackSettings.Channel"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Channel))]
    public static T SetChannel<T>(this T o, string v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.Channel, v));
    /// <inheritdoc cref="VelopackPackSettings.Channel"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Channel))]
    public static T ResetChannel<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.Channel));
    #endregion
    #region PackId
    /// <inheritdoc cref="VelopackPackSettings.PackId"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.PackId))]
    public static T SetPackId<T>(this T o, string v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.PackId, v));
    /// <inheritdoc cref="VelopackPackSettings.PackId"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.PackId))]
    public static T ResetPackId<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.PackId));
    #endregion
    #region PackVersion
    /// <inheritdoc cref="VelopackPackSettings.PackVersion"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.PackVersion))]
    public static T SetPackVersion<T>(this T o, string v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.PackVersion, v));
    /// <inheritdoc cref="VelopackPackSettings.PackVersion"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.PackVersion))]
    public static T ResetPackVersion<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.PackVersion));
    #endregion
    #region PackDir
    /// <inheritdoc cref="VelopackPackSettings.PackDir"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.PackDir))]
    public static T SetPackDir<T>(this T o, string v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.PackDir, v));
    /// <inheritdoc cref="VelopackPackSettings.PackDir"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.PackDir))]
    public static T ResetPackDir<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.PackDir));
    #endregion
    #region OutputDir
    /// <inheritdoc cref="VelopackPackSettings.OutputDir"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.OutputDir))]
    public static T SetOutputDir<T>(this T o, string v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.OutputDir, v));
    /// <inheritdoc cref="VelopackPackSettings.OutputDir"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.OutputDir))]
    public static T ResetOutputDir<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.OutputDir));
    #endregion
    #region Runtime
    /// <inheritdoc cref="VelopackPackSettings.Runtime"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Runtime))]
    public static T SetRuntime<T>(this T o, string v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.Runtime, v));
    /// <inheritdoc cref="VelopackPackSettings.Runtime"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Runtime))]
    public static T ResetRuntime<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.Runtime));
    #endregion
    #region MainExe
    /// <inheritdoc cref="VelopackPackSettings.MainExe"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.MainExe))]
    public static T SetMainExe<T>(this T o, string v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.MainExe, v));
    /// <inheritdoc cref="VelopackPackSettings.MainExe"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.MainExe))]
    public static T ResetMainExe<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.MainExe));
    #endregion
    #region Verbose
    /// <inheritdoc cref="VelopackPackSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Verbose))]
    public static T SetVerbose<T>(this T o, bool? v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.Verbose, v));
    /// <inheritdoc cref="VelopackPackSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Verbose))]
    public static T ResetVerbose<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.Verbose));
    /// <inheritdoc cref="VelopackPackSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Verbose))]
    public static T EnableVerbose<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.Verbose, true));
    /// <inheritdoc cref="VelopackPackSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Verbose))]
    public static T DisableVerbose<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.Verbose, false));
    /// <inheritdoc cref="VelopackPackSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Verbose))]
    public static T ToggleVerbose<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.Verbose, !o.Verbose));
    #endregion
    #region Yes
    /// <inheritdoc cref="VelopackPackSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Yes))]
    public static T SetYes<T>(this T o, bool? v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.Yes, v));
    /// <inheritdoc cref="VelopackPackSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Yes))]
    public static T ResetYes<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.Yes));
    /// <inheritdoc cref="VelopackPackSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Yes))]
    public static T EnableYes<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.Yes, true));
    /// <inheritdoc cref="VelopackPackSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Yes))]
    public static T DisableYes<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.Yes, false));
    /// <inheritdoc cref="VelopackPackSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.Yes))]
    public static T ToggleYes<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.Yes, !o.Yes));
    #endregion
    #region LegacyConsole
    /// <inheritdoc cref="VelopackPackSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.LegacyConsole))]
    public static T SetLegacyConsole<T>(this T o, bool? v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.LegacyConsole, v));
    /// <inheritdoc cref="VelopackPackSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.LegacyConsole))]
    public static T ResetLegacyConsole<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.LegacyConsole));
    /// <inheritdoc cref="VelopackPackSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.LegacyConsole))]
    public static T EnableLegacyConsole<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.LegacyConsole, true));
    /// <inheritdoc cref="VelopackPackSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.LegacyConsole))]
    public static T DisableLegacyConsole<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.LegacyConsole, false));
    /// <inheritdoc cref="VelopackPackSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.LegacyConsole))]
    public static T ToggleLegacyConsole<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.LegacyConsole, !o.LegacyConsole));
    #endregion
    #region SkipUpdates
    /// <inheritdoc cref="VelopackPackSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.SkipUpdates))]
    public static T SetSkipUpdates<T>(this T o, bool? v) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.SkipUpdates, v));
    /// <inheritdoc cref="VelopackPackSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.SkipUpdates))]
    public static T ResetSkipUpdates<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Remove(() => o.SkipUpdates));
    /// <inheritdoc cref="VelopackPackSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.SkipUpdates))]
    public static T EnableSkipUpdates<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.SkipUpdates, true));
    /// <inheritdoc cref="VelopackPackSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.SkipUpdates))]
    public static T DisableSkipUpdates<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.SkipUpdates, false));
    /// <inheritdoc cref="VelopackPackSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackPackSettings), Property = nameof(VelopackPackSettings.SkipUpdates))]
    public static T ToggleSkipUpdates<T>(this T o) where T : VelopackPackSettings => o.Modify(b => b.Set(() => o.SkipUpdates, !o.SkipUpdates));
    #endregion
}
#endregion
#region VelopackDownloadGithubSettingsExtensions
/// <inheritdoc cref="VelopackTasks.VelopackDownloadGithub(.VelopackDownloadGithubSettings)"/>
[PublicAPI]
[ExcludeFromCodeCoverage]
public static partial class VelopackDownloadGithubSettingsExtensions
{
    #region Channel
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Channel"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Channel))]
    public static T SetChannel<T>(this T o, string v) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Channel, v));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Channel"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Channel))]
    public static T ResetChannel<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Remove(() => o.Channel));
    #endregion
    #region RepoUrl
    /// <inheritdoc cref="VelopackDownloadGithubSettings.RepoUrl"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.RepoUrl))]
    public static T SetRepoUrl<T>(this T o, string v) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.RepoUrl, v));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.RepoUrl"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.RepoUrl))]
    public static T ResetRepoUrl<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Remove(() => o.RepoUrl));
    #endregion
    #region Token
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Token"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Token))]
    public static T SetToken<T>(this T o, [Secret] string v) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Token, v));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Token"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Token))]
    public static T ResetToken<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Remove(() => o.Token));
    #endregion
    #region OutputDir
    /// <inheritdoc cref="VelopackDownloadGithubSettings.OutputDir"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.OutputDir))]
    public static T SetOutputDir<T>(this T o, string v) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.OutputDir, v));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.OutputDir"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.OutputDir))]
    public static T ResetOutputDir<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Remove(() => o.OutputDir));
    #endregion
    #region Pre
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Pre"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Pre))]
    public static T SetPre<T>(this T o, bool? v) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Pre, v));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Pre"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Pre))]
    public static T ResetPre<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Remove(() => o.Pre));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Pre"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Pre))]
    public static T EnablePre<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Pre, true));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Pre"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Pre))]
    public static T DisablePre<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Pre, false));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Pre"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Pre))]
    public static T TogglePre<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Pre, !o.Pre));
    #endregion
    #region Verbose
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Verbose))]
    public static T SetVerbose<T>(this T o, bool? v) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Verbose, v));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Verbose))]
    public static T ResetVerbose<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Remove(() => o.Verbose));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Verbose))]
    public static T EnableVerbose<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Verbose, true));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Verbose))]
    public static T DisableVerbose<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Verbose, false));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Verbose))]
    public static T ToggleVerbose<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Verbose, !o.Verbose));
    #endregion
    #region Yes
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Yes))]
    public static T SetYes<T>(this T o, bool? v) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Yes, v));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Yes))]
    public static T ResetYes<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Remove(() => o.Yes));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Yes))]
    public static T EnableYes<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Yes, true));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Yes))]
    public static T DisableYes<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Yes, false));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.Yes))]
    public static T ToggleYes<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.Yes, !o.Yes));
    #endregion
    #region LegacyConsole
    /// <inheritdoc cref="VelopackDownloadGithubSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.LegacyConsole))]
    public static T SetLegacyConsole<T>(this T o, bool? v) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.LegacyConsole, v));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.LegacyConsole))]
    public static T ResetLegacyConsole<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Remove(() => o.LegacyConsole));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.LegacyConsole))]
    public static T EnableLegacyConsole<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.LegacyConsole, true));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.LegacyConsole))]
    public static T DisableLegacyConsole<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.LegacyConsole, false));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.LegacyConsole))]
    public static T ToggleLegacyConsole<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.LegacyConsole, !o.LegacyConsole));
    #endregion
    #region SkipUpdates
    /// <inheritdoc cref="VelopackDownloadGithubSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.SkipUpdates))]
    public static T SetSkipUpdates<T>(this T o, bool? v) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.SkipUpdates, v));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.SkipUpdates))]
    public static T ResetSkipUpdates<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Remove(() => o.SkipUpdates));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.SkipUpdates))]
    public static T EnableSkipUpdates<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.SkipUpdates, true));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.SkipUpdates))]
    public static T DisableSkipUpdates<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.SkipUpdates, false));
    /// <inheritdoc cref="VelopackDownloadGithubSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackDownloadGithubSettings), Property = nameof(VelopackDownloadGithubSettings.SkipUpdates))]
    public static T ToggleSkipUpdates<T>(this T o) where T : VelopackDownloadGithubSettings => o.Modify(b => b.Set(() => o.SkipUpdates, !o.SkipUpdates));
    #endregion
}
#endregion
#region VelopackDeltaGenerateSettingsExtensions
/// <inheritdoc cref="VelopackTasks.VelopackDeltaGenerate(.VelopackDeltaGenerateSettings)"/>
[PublicAPI]
[ExcludeFromCodeCoverage]
public static partial class VelopackDeltaGenerateSettingsExtensions
{
    #region Channel
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Channel"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Channel))]
    public static T SetChannel<T>(this T o, string v) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Channel, v));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Channel"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Channel))]
    public static T ResetChannel<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Remove(() => o.Channel));
    #endregion
    #region Base
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Base"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Base))]
    public static T SetBase<T>(this T o, string v) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Base, v));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Base"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Base))]
    public static T ResetBase<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Remove(() => o.Base));
    #endregion
    #region New
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.New"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.New))]
    public static T SetNew<T>(this T o, string v) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.New, v));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.New"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.New))]
    public static T ResetNew<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Remove(() => o.New));
    #endregion
    #region Output
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Output"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Output))]
    public static T SetOutput<T>(this T o, string v) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Output, v));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Output"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Output))]
    public static T ResetOutput<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Remove(() => o.Output));
    #endregion
    #region Verbose
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Verbose))]
    public static T SetVerbose<T>(this T o, bool? v) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Verbose, v));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Verbose))]
    public static T ResetVerbose<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Remove(() => o.Verbose));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Verbose))]
    public static T EnableVerbose<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Verbose, true));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Verbose))]
    public static T DisableVerbose<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Verbose, false));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Verbose))]
    public static T ToggleVerbose<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Verbose, !o.Verbose));
    #endregion
    #region Yes
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Yes))]
    public static T SetYes<T>(this T o, bool? v) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Yes, v));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Yes))]
    public static T ResetYes<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Remove(() => o.Yes));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Yes))]
    public static T EnableYes<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Yes, true));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Yes))]
    public static T DisableYes<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Yes, false));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.Yes))]
    public static T ToggleYes<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.Yes, !o.Yes));
    #endregion
    #region LegacyConsole
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.LegacyConsole))]
    public static T SetLegacyConsole<T>(this T o, bool? v) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.LegacyConsole, v));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.LegacyConsole))]
    public static T ResetLegacyConsole<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Remove(() => o.LegacyConsole));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.LegacyConsole))]
    public static T EnableLegacyConsole<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.LegacyConsole, true));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.LegacyConsole))]
    public static T DisableLegacyConsole<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.LegacyConsole, false));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.LegacyConsole))]
    public static T ToggleLegacyConsole<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.LegacyConsole, !o.LegacyConsole));
    #endregion
    #region SkipUpdates
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.SkipUpdates))]
    public static T SetSkipUpdates<T>(this T o, bool? v) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.SkipUpdates, v));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.SkipUpdates))]
    public static T ResetSkipUpdates<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Remove(() => o.SkipUpdates));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.SkipUpdates))]
    public static T EnableSkipUpdates<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.SkipUpdates, true));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.SkipUpdates))]
    public static T DisableSkipUpdates<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.SkipUpdates, false));
    /// <inheritdoc cref="VelopackDeltaGenerateSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackDeltaGenerateSettings), Property = nameof(VelopackDeltaGenerateSettings.SkipUpdates))]
    public static T ToggleSkipUpdates<T>(this T o) where T : VelopackDeltaGenerateSettings => o.Modify(b => b.Set(() => o.SkipUpdates, !o.SkipUpdates));
    #endregion
}
#endregion
#region VelopackUploadGithubSettingsExtensions
/// <inheritdoc cref="VelopackTasks.VelopackUploadGithub(.VelopackUploadGithubSettings)"/>
[PublicAPI]
[ExcludeFromCodeCoverage]
public static partial class VelopackUploadGithubSettingsExtensions
{
    #region Channel
    /// <inheritdoc cref="VelopackUploadGithubSettings.Channel"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Channel))]
    public static T SetChannel<T>(this T o, string v) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Channel, v));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Channel"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Channel))]
    public static T ResetChannel<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Remove(() => o.Channel));
    #endregion
    #region RepoUrl
    /// <inheritdoc cref="VelopackUploadGithubSettings.RepoUrl"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.RepoUrl))]
    public static T SetRepoUrl<T>(this T o, string v) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.RepoUrl, v));
    /// <inheritdoc cref="VelopackUploadGithubSettings.RepoUrl"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.RepoUrl))]
    public static T ResetRepoUrl<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Remove(() => o.RepoUrl));
    #endregion
    #region Token
    /// <inheritdoc cref="VelopackUploadGithubSettings.Token"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Token))]
    public static T SetToken<T>(this T o, [Secret] string v) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Token, v));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Token"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Token))]
    public static T ResetToken<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Remove(() => o.Token));
    #endregion
    #region OutputDir
    /// <inheritdoc cref="VelopackUploadGithubSettings.OutputDir"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.OutputDir))]
    public static T SetOutputDir<T>(this T o, string v) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.OutputDir, v));
    /// <inheritdoc cref="VelopackUploadGithubSettings.OutputDir"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.OutputDir))]
    public static T ResetOutputDir<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Remove(() => o.OutputDir));
    #endregion
    #region Publish
    /// <inheritdoc cref="VelopackUploadGithubSettings.Publish"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Publish))]
    public static T SetPublish<T>(this T o, bool? v) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Publish, v));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Publish"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Publish))]
    public static T ResetPublish<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Remove(() => o.Publish));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Publish"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Publish))]
    public static T EnablePublish<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Publish, true));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Publish"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Publish))]
    public static T DisablePublish<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Publish, false));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Publish"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Publish))]
    public static T TogglePublish<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Publish, !o.Publish));
    #endregion
    #region Verbose
    /// <inheritdoc cref="VelopackUploadGithubSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Verbose))]
    public static T SetVerbose<T>(this T o, bool? v) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Verbose, v));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Verbose))]
    public static T ResetVerbose<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Remove(() => o.Verbose));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Verbose))]
    public static T EnableVerbose<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Verbose, true));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Verbose))]
    public static T DisableVerbose<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Verbose, false));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Verbose"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Verbose))]
    public static T ToggleVerbose<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Verbose, !o.Verbose));
    #endregion
    #region Yes
    /// <inheritdoc cref="VelopackUploadGithubSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Yes))]
    public static T SetYes<T>(this T o, bool? v) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Yes, v));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Yes))]
    public static T ResetYes<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Remove(() => o.Yes));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Yes))]
    public static T EnableYes<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Yes, true));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Yes))]
    public static T DisableYes<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Yes, false));
    /// <inheritdoc cref="VelopackUploadGithubSettings.Yes"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.Yes))]
    public static T ToggleYes<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.Yes, !o.Yes));
    #endregion
    #region LegacyConsole
    /// <inheritdoc cref="VelopackUploadGithubSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.LegacyConsole))]
    public static T SetLegacyConsole<T>(this T o, bool? v) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.LegacyConsole, v));
    /// <inheritdoc cref="VelopackUploadGithubSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.LegacyConsole))]
    public static T ResetLegacyConsole<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Remove(() => o.LegacyConsole));
    /// <inheritdoc cref="VelopackUploadGithubSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.LegacyConsole))]
    public static T EnableLegacyConsole<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.LegacyConsole, true));
    /// <inheritdoc cref="VelopackUploadGithubSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.LegacyConsole))]
    public static T DisableLegacyConsole<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.LegacyConsole, false));
    /// <inheritdoc cref="VelopackUploadGithubSettings.LegacyConsole"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.LegacyConsole))]
    public static T ToggleLegacyConsole<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.LegacyConsole, !o.LegacyConsole));
    #endregion
    #region SkipUpdates
    /// <inheritdoc cref="VelopackUploadGithubSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.SkipUpdates))]
    public static T SetSkipUpdates<T>(this T o, bool? v) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.SkipUpdates, v));
    /// <inheritdoc cref="VelopackUploadGithubSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.SkipUpdates))]
    public static T ResetSkipUpdates<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Remove(() => o.SkipUpdates));
    /// <inheritdoc cref="VelopackUploadGithubSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.SkipUpdates))]
    public static T EnableSkipUpdates<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.SkipUpdates, true));
    /// <inheritdoc cref="VelopackUploadGithubSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.SkipUpdates))]
    public static T DisableSkipUpdates<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.SkipUpdates, false));
    /// <inheritdoc cref="VelopackUploadGithubSettings.SkipUpdates"/>
    [Pure] [Builder(Type = typeof(VelopackUploadGithubSettings), Property = nameof(VelopackUploadGithubSettings.SkipUpdates))]
    public static T ToggleSkipUpdates<T>(this T o) where T : VelopackUploadGithubSettings => o.Modify(b => b.Set(() => o.SkipUpdates, !o.SkipUpdates));
    #endregion
}
#endregion
