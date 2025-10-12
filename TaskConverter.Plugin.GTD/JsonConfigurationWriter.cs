using System.IO.Abstractions;
using System.Text;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD;

public class JsonConfigurationWriter(IFileSystem FileSystem, IJsonConfigurationSerializer JsonConfigurationSerializer) : IWriter<GTDDataModel?>
{
    public void Write(string destination, GTDDataModel? model)
    {
        if (model == null)
            return;

        var serializedModel = JsonConfigurationSerializer.Serialize(model);
        FileSystem.File.WriteAllText(destination, serializedModel, Encoding.UTF8);
    }
}
