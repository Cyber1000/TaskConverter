using NodaTime.Serialization.SystemTextJson;
using NodaTime.Text;

namespace TaskConverter.Commons.ConversionHelper;

public class NodaNullablePatternConverter<T> : DelegatingConverterBase<T>
{
    public override bool HandleNull => true;

    public NodaNullablePatternConverter(IPattern<T> pattern)
        : base(new NodaPatternConverter<T>(pattern)) { }
}
