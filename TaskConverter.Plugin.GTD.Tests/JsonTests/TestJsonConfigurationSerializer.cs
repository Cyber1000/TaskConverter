namespace TaskConverter.Plugin.GTD.Tests.JsonTests
{
    public class TestJsonConfigurationSerializer : JsonConfigurationSerializer
    {
        private string _overrideString = string.Empty;

        public void OverrideSerializerString(string overrideString)
        {
            _overrideString = overrideString;
        }

        public void ResetSerializerString()
        {
            _overrideString = string.Empty;
        }

        public override string Serialize<TValue>(TValue value)
        {
            return string.IsNullOrEmpty(_overrideString) ? base.Serialize(value) : _overrideString;
        }
    }
}
