namespace TaskConverter.Commons.ConversionHelper;

public class ExactLocalDateTimeConverter<T> : NodaNullablePatternConverter<T>
{
    public ExactLocalDateTimeConverter()
        : base(LocalDateTimeNullablePattern<T>.CreateWithInvariantCulture("yyyy-MM-dd HH:mm:ss.fff")) { }
}
