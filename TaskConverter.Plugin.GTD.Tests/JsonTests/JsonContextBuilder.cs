using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.JsonTests;

public static class JsonContextBuilderExtensions
{
    public static JsonContextBuilder JsonContext(this IObjectBuilder _) => new();
}

public class JsonContextBuilder
{
    private readonly Dictionary<string, object> context = [];

    public JsonContextBuilder()
    {
        context["UUID"] = "";
        context["TITLE"] = "Test";
    }

    public JsonContextBuilder WithUuid(string uuid)
    {
        context["UUID"] = uuid;
        return this;
    }

    public JsonContextBuilder WithTitle(string title)
    {
        context["TITLE"] = title;
        return this;
    }

    public Dictionary<string, object> Build()
    {
        return context;
    }
}
