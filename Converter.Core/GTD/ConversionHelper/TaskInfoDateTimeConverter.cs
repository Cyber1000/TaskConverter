using System.Text.Json;
using System.Text.Json.Serialization;
using Converter.Core.Utils;

namespace Converter.Core.GTD.ConversionHelper;

public abstract class TaskInfoDateTimeConverterBase<TDestination> : JsonConverter<TDestination>
{
    public enum OriginalTypes
    {
        DateTime,
        UnixTimeStamp
    }

    public virtual OriginalTypes OriginalType => OriginalTypes.DateTime;

    public override bool HandleNull => true;

    protected virtual string ConversionFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";

    private static string GetTimeZoneId() => SettingsHelper.GetAppSetting("TimeZoneId", TimeZoneInfo.Local.Id);

    public override TDestination? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        DateTime? dateTime;
        if (OriginalType == OriginalTypes.DateTime)
        {
            dateTime = ReadFromDateString(reader);
        }
        else if (OriginalType == OriginalTypes.UnixTimeStamp)
        {
            dateTime = ReadFromUnixTimeStamp(reader);
        }
        else
        {
            throw new NotImplementedException($"Type {OriginalType} is not implemented");
        }
        if (dateTime is null)
            return default;

        return (TDestination)Convert.ChangeType(dateTime, typeof(DateTime));
    }

    private static DateTime? ReadFromDateString(Utf8JsonReader reader)
    {
        var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(GetTimeZoneId());
        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString))
            return default;

        var dateTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(dateString), localTimeZone);
        return dateTime;
    }

    private static DateTime? ReadFromUnixTimeStamp(Utf8JsonReader reader)
    {
        var unixTimeStamp = reader.GetInt64();
        if (unixTimeStamp <= 0)
            return default;

        return DateTime.UnixEpoch.AddMilliseconds(unixTimeStamp);
    }

    public override void Write(Utf8JsonWriter writer, TDestination? value, JsonSerializerOptions options)
    {
        var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(GetTimeZoneId());
        if (value == null || value is not DateTime dateValue)
        {
            WriteDefaultValue(writer);
            return;
        }

        if (OriginalType == OriginalTypes.DateTime)
        {
            var dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateValue, localTimeZone);
            writer.WriteStringValue(dateTime.ToString(ConversionFormat));
        }
        else
        {
            writer.WriteNumberValue(dateValue.Subtract(DateTime.UnixEpoch).TotalMilliseconds);
        }
    }

    protected virtual void WriteDefaultValue(Utf8JsonWriter writer)
    {
        writer.WriteStringValue("");
    }
}

public class TaskInfoDateTimeConverter : TaskInfoDateTimeConverterBase<DateTime>
{
    public TaskInfoDateTimeConverter() { }

    public TaskInfoDateTimeConverter(string conversionFormat)
    {
        ConversionFormat = conversionFormat;
    }
}

public class TaskInfoDateTimeNullableConverter : TaskInfoDateTimeConverterBase<DateTime?>
{
    public TaskInfoDateTimeNullableConverter() { }

    public TaskInfoDateTimeNullableConverter(string conversionFormat)
    {
        ConversionFormat = conversionFormat;
    }
}

public class TaskInfoUnixTimeStampConverter : TaskInfoDateTimeConverterBase<DateTime?>
{
    public override OriginalTypes OriginalType => OriginalTypes.UnixTimeStamp;

    protected override void WriteDefaultValue(Utf8JsonWriter writer)
    {
        writer.WriteNumberValue(0);
    }
}