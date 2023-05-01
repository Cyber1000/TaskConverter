using System.Text;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.JsonDiffPatch.Diffs.Formatters;
using System.Text.Json.Nodes;

namespace TaskConverter.Plugin.GTD.Utils;

public class JsonDeltaFormatter : JsonPatchDeltaFormatter
{
    private const string PropertyNameOperation = "op";
    private const string PropertyNamePath = "path";
    private const string PropertyNameValue = "value";

    private const string PropertyNameOriginal = "original";
    private const string PropertyNameNew = "new";

    private const string OperationNameAdd = "additional";
    private const string OperationNameRemove = "missing";
    private const string OperationNameReplace = "modified";

    protected override JsonNode? FormatArrayElement(
        in JsonDiffDelta.ArrayChangeEntry arrayChange,
        JsonNode? left,
        JsonNode? existingValue
    )
    {
        var index = arrayChange.Index.ToString();
        if (left is JsonObject obj && obj.TryGetPropertyValue("ID", out var id))
        {
            index = $"ID-{id}";
        }
        using var _ = new PropertyPathScope(PathBuilder, index);
        return base.FormatArrayElement(arrayChange, left, existingValue);
    }

    protected override JsonNode? FormatAdded(ref JsonDiffDelta delta, JsonNode? existingValue)
    {
        var op = new JsonObject
        {
            { PropertyNameOperation, OperationNameAdd },
            { PropertyNamePath, PathBuilder.ToString() },
            { PropertyNameValue, delta.GetAdded() }
        };
        existingValue!.AsArray().Add(op);
        return existingValue;
    }

    protected override JsonNode? FormatModified(ref JsonDiffDelta delta, JsonNode? left, JsonNode? existingValue)
    {
        var op = new JsonObject
        {
            { PropertyNameOperation, OperationNameReplace },
            { PropertyNamePath, PathBuilder.ToString() },
            { PropertyNameOriginal, delta.GetOldValue() },
            { PropertyNameNew, delta.GetNewValue() }
        };
        existingValue!.AsArray().Add(op);
        return existingValue;
    }

    protected override JsonNode? FormatDeleted(ref JsonDiffDelta delta, JsonNode? left, JsonNode? existingValue)
    {
        var op = new JsonObject { { PropertyNameOperation, OperationNameRemove }, { PropertyNamePath, PathBuilder.ToString() } };
        existingValue!.AsArray().Add(op);
        return existingValue;
    }

    private readonly struct PropertyPathScope : IDisposable
    {
        private readonly StringBuilder _pathBuilder;
        private readonly int _startIndex;
        private readonly int _length;

        public PropertyPathScope(StringBuilder pathBuilder, string propertyName)
        {
            _pathBuilder = pathBuilder;
            _startIndex = pathBuilder.Length;
            pathBuilder.Append('/');
            pathBuilder.Append(Escape(propertyName));
            _length = pathBuilder.Length - _startIndex;
        }

        public PropertyPathScope(StringBuilder pathBuilder, int index)
        {
            _pathBuilder = pathBuilder;
            _startIndex = pathBuilder.Length;
            pathBuilder.Append('/');
            pathBuilder.Append(index.ToString("D"));
            _length = pathBuilder.Length - _startIndex;
        }

        public void Dispose()
        {
            _pathBuilder.Remove(_startIndex, _length);
        }

        private static string Escape(string str)
        {
            // Escape Json Pointer as per https://datatracker.ietf.org/doc/html/rfc6901#section-3
            var sb = new StringBuilder(str);
            for (var i = 0; i < sb.Length; i++)
            {
                if (sb[i] == '/')
                {
                    sb.Insert(i, '~');
                    sb[++i] = '1';
                }
                else if (sb[i] == '~')
                {
                    sb.Insert(i, '~');
                    sb[++i] = '0';
                }
            }

            return sb.ToString();
        }
    }
}
