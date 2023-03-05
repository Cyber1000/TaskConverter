namespace Converter.Core.GTD.ConversionHelper;

public class ExactToMinuteLocalDateTimeConverter<T> : NodaNullablePatternConverter<T>
{
    public ExactToMinuteLocalDateTimeConverter()
        : base(LocalDateTimeNullablePattern<T>.CreateWithInvariantCulture("yyyy-MM-dd HH:mm")) { }
}
