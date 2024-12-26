using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.JsonTests;

public static class JsonFolderBuilderExtensions
{
    public static JsonFolderBuilder JsonFolder(this IObjectBuilder _) => new();
}

public class JsonFolderBuilder
{
    private readonly Dictionary<string, object> folder = [];

    public JsonFolderBuilder()
    {
        folder["UUID"] = "";
        folder["TITLE"] = "Test";
    }

    public JsonFolderBuilder WithUuid(string uuid)
    {
        folder["UUID"] = uuid;
        return this;
    }

    public JsonFolderBuilder WithTitle(string title)
    {
        folder["TITLE"] = title;
        return this;
    }

    public Dictionary<string, object> Build()
    {
        return folder;
    }
}
