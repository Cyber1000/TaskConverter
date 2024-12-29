namespace TaskConverter.Plugin.GTD
{
    public interface IJsonConfigurationSerializer
    {
        string Serialize<TValue>(TValue value);
        TValue? Deserialize<TValue>(string json);
    }
}
