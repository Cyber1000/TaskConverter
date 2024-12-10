using System.Globalization;
using System.IO.Abstractions;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;

namespace TaskConverter.Plugin.GTD.Utils;

public static class JsonUtil
{
    private readonly static List<(string oldValue, string newValue)> RepeatInfoMapper =
        [
            ("Norepeat", ""),
            ("Daily", "Every 1 day"),
            ("Weekly", "Every 1 week"),
            ("Biweekly", "Every 2 weeks"),
            ("Monthly", "Every 1 month"),
            ("Bimonthly", "Every 2 months"),
            ("Quarterly", "Every 3 months"),
            ("Semiannually", "Every 6 months"),
            ("Yearly", "Every 1 year")
        ];

    public static string Read(this IFileInfo file)
    {
        return NormalizeText(file.FileSystem.File.ReadAllText(file.FullName));
    }

    public static string NormalizeText(string text)
    {
        var resultText = text;
        foreach (var (oldValue, newValue) in RepeatInfoMapper)
        {
            resultText = resultText.Replace("\"" + oldValue + "\"", "\"" + newValue + "\"", true, CultureInfo.InvariantCulture);
        }
        return resultText;
    }

    public static JsonDiffOptions GetDiffOptions()
    {
        var jsonDiffOptions = new JsonDiffOptions
        {
            ArrayObjectItemKeyFinder = (node, index) =>
            {
                if (node is JsonObject obj && obj.TryGetPropertyValue("ID", out var value))
                {
                    return value?.GetValue<int>() ?? 0;
                }

                return null;
            },
            ArrayObjectItemMatchByPosition = true,
            PropertyFilter = (prop, _) => !string.Equals(prop, "Preferences")
        };
        return jsonDiffOptions;
    }
}