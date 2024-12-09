using System.Text.Json;

namespace TaskConverter.Tests.JsonTests;

public static class JsonDataBuilderExtensions
{
    public static JsonDataBuilder JsonData(this IObjectBuilder _) => new();
}

public class JsonDataBuilder
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    private int version = 3;
    private readonly List<Dictionary<string, object>> tasks = [];
    private readonly List<Dictionary<string, object>> tags = [];
    private string preferences = "";

    public JsonDataBuilder WithVersion(int version)
    {
        this.version = version;
        return this;
    }

    public JsonDataBuilder AddTask(Dictionary<string, object> task)
    {
        tasks.Add(task);
        return this;
    }

    public JsonDataBuilder AddTag(Dictionary<string, object> tag)
    {
        tags.Add(tag);
        return this;
    }

    public JsonDataBuilder WithPreferences(string preferences)
    {
        this.preferences = preferences;
        return this;
    }

    public string Build()
    {
        var jsonObject = new
        {
            version,
            TASK = tasks,
            TAG = tags,
            Preferences = new[] { new Dictionary<string, string> { { "com.dg.gtd.android.lite_preferences", preferences } } },
        };
        return JsonSerializer.Serialize(jsonObject, SerializerOptions);
    }
}
