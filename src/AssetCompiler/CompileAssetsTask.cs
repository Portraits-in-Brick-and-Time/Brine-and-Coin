using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ObjectModel.IO;

namespace AssetCompiler;

public class CompileAssetsTask : Task
{
    [Required]
    public ITaskItem[] Sources { get; set; }

    [Required]
    public string OutputFilename { get; set; }

    public override bool Execute()
    {
        try
        {
            Log.LogMessage(MessageImportance.High, $"Compiling assets: {Sources} -> {OutputFilename}");

            using var elf = File.Create(OutputFilename);
            var objectWriter = new GameAssetWriter(elf);
            foreach (var source in Sources)
            {
                objectWriter.WriteObjects(source.ToString());
            }
            objectWriter.Close();

            Log.LogMessage(MessageImportance.Low, "Asset compilation completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Log.LogError($"Asset compilation failed: {ex.Message}");
            return false;
        }
    }
}
