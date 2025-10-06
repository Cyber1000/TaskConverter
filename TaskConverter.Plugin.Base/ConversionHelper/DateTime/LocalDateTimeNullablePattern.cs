using System.Text;
using NodaTime;
using NodaTime.Text;

namespace TaskConverter.Plugin.Base.ConversionHelper.DateTime;

public class LocalDateTimeNullablePattern<T> : IPattern<T>
{
    private LocalDateTimePattern Pattern { get; }

    public static IPattern<T> CreateWithInvariantCulture(string pattern)
    {
        return new LocalDateTimeNullablePattern<T>(LocalDateTimePattern.CreateWithInvariantCulture(pattern));
    }

    private LocalDateTimeNullablePattern(LocalDateTimePattern pattern)
    {
        if (BaseType != typeof(LocalDateTime))
        {
            throw new ArgumentException($"Type {typeof(T)} is not allowed here, only LocalDateTime|Nullable<LocalDateTime> allowed.");
        }

        Pattern = pattern;
    }

    public StringBuilder AppendFormat(T value, StringBuilder builder)
    {
        return !HasValue(value) ? new StringBuilder() : Pattern.AppendFormat(GetLocalDateTime(value), builder);
    }

    public string Format(T value)
    {
        return !HasValue(value) ? string.Empty : Pattern.Format(GetLocalDateTime(value));
    }

    public ParseResult<T> Parse(string text)
    {
        ParseResult<LocalDateTime> parseResult = Pattern.Parse(text);

        if (!parseResult.Success)
        {
#pragma warning disable CS8604
            //Checking for IsValueType wouldn't be necessary, since LocalDateTime is always valuetype, compiler is still complaining that ForValue may get a null value
            if (string.IsNullOrEmpty(text) && IsNullable && typeof(T).IsValueType)
            {
                return ParseResult<T>.ForValue(default);
            }
#pragma warning restore CS8604

            return Pattern.Parse(text).ConvertError<T>();
        }
        return parseResult.Convert(GetT);
    }

    private readonly Type BaseType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

    private readonly bool IsNullable = Nullable.GetUnderlyingType(typeof(T)) != null;

    private static bool HasValue(T value)
    {
        return !EqualityComparer<T>.Default.Equals(value, default);
    }

    private static LocalDateTime GetLocalDateTime(T value)
    {
        return value == null ? default : (LocalDateTime)Convert.ChangeType(value, typeof(LocalDateTime));
    }

    private T GetT(LocalDateTime value)
    {
        return (T)Convert.ChangeType(value, BaseType);
    }
}
