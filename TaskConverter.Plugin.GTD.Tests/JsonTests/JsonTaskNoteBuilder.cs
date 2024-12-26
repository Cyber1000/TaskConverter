using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.JsonTests;

public static class JsonTaskNoteBuilderExtensions
{
    public static JsonTaskNoteBuilder JsonTaskNote(this IObjectBuilder _) => new();
}

public class JsonTaskNoteBuilder
{
    private readonly Dictionary<string, object> taskNote = [];

    public Dictionary<string, object> Build()
    {
        return taskNote;
    }
}
