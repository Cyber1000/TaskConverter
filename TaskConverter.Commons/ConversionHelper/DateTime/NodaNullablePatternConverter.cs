using NodaTime.Serialization.SystemTextJson;
using NodaTime.Text;

namespace TaskConverter.Commons.ConversionHelper.DateTime;

public class NodaNullablePatternConverter<T>(IPattern<T> pattern) : DelegatingConverterBase<T>(new NodaPatternConverter<T>(pattern))
{
    public override bool HandleNull => true;
}
