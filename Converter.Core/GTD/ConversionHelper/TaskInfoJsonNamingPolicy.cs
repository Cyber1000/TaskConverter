using System.Text.Json;
using System.Text.RegularExpressions;

namespace Converter.Core.GTD.ConversionHelper;

public partial class TaskInfoJsonNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return MyRegex().Replace(name, "_$1").ToUpper();
    }

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex MyRegex();
}
