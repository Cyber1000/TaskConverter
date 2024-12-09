namespace TaskConverter.Tests;


public static class JsonDataBuilderExtensions
{
    public static JsonTagBuilder JsonTag(this IObjectBuilder _) => new();
}

public class JsonTagBuilder
{
    private readonly Dictionary<string, object> tag = [];

    public JsonTagBuilder()
    {
        tag["ID"] = 27;
        tag["UUID"] = "";
        tag["CREATED"] = "2018-07-20 09:16:35.617";
        tag["MODIFIED"] = "2018-07-20 09:16:35.617";
        tag["TITLE"] = "Anruf";
        tag["COLOR"] = -1048832;
        tag["VISIBLE"] = 1;
    }

    public JsonTagBuilder WithColor(int color)
    {
        tag["COLOR"] = color;
        return this;
    }

    public JsonTagBuilder WithVisible(int visible)
    {
        tag["VISIBLE"] = visible;
        return this;
    }

    public Dictionary<string, object> Build()
    {
        return tag;
    }
}
