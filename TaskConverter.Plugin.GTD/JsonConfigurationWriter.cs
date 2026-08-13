using System.IO.Abstractions;
using System.Text;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.Base.Utils;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD;

public class JsonConfigurationWriter(IFileSystem FileSystem, IJsonConfigurationSerializer JsonConfigurationSerializer) : IWriter<GTDDataModel?>
{
    public void Write(string destination, GTDDataModel? model)
    {
        if (model == null)
            return;

        var serializedModel = JsonConfigurationSerializer.Serialize(model);
        var outputFile = FileSystem.FileInfo.New(destination);

        // Symmetric to the reader: it accepts a zip, so a destination ending in .zip has to become
        // one, otherwise the app cannot read back what was written here.
        if (outputFile.FullName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            outputFile.WriteToZip(serializedModel);
        else
            FileSystem.File.WriteAllText(destination, serializedModel, Encoding.UTF8);
    }
}
