using TaskConverter.Commons.ConversionHelper.DateTime;

namespace TaskConverter.Plugin.GTD.ConversionHelper;

public class ExactToMinuteLocalDateTimeConverter<T> : NodaNullablePatternConverter<T>
{
    public ExactToMinuteLocalDateTimeConverter()
        : base(LocalDateTimeNullablePattern<T>.CreateWithInvariantCulture("yyyy-MM-dd HH:mm")) { }
}
