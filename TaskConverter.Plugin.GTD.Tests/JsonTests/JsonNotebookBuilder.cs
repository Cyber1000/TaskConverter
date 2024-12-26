using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.JsonTests;

public static class JsonNotebookBuilderExtensions
{
    public static JsonNotebookBuilder JsonNotebook(this IObjectBuilder _) => new();
}

public class JsonNotebookBuilder
{
    private readonly Dictionary<string, object> notebook = [];

    public JsonNotebookBuilder()
    {
        notebook["UUID"] = "";
        notebook["TITLE"] = "Test";
    }

    public JsonNotebookBuilder WithUuid(string uuid)
    {
        notebook["UUID"] = uuid;
        return this;
    }

    public JsonNotebookBuilder WithTitle(string title)
    {
        notebook["TITLE"] = title;
        return this;
    }

    public Dictionary<string, object> Build()
    {
        return notebook;
    }
}
