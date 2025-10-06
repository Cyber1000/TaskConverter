namespace TaskConverter.Plugin.Base.ConversionHelper.DateTime;

public class ExactLocalDateTimeConverter<T> : NodaNullablePatternConverter<T>
{
    public ExactLocalDateTimeConverter()
        : base(LocalDateTimeNullablePattern<T>.CreateWithInvariantCulture("yyyy-MM-dd HH:mm:ss.fff")) { }
}
