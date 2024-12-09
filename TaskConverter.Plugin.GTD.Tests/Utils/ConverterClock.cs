using NodaTime;

namespace TaskConverter.Plugin.GTD.Tests.Utils;

public class ConverterClock : IClock
{
    public Instant GetCurrentInstant()
    {
        return Instant.FromUnixTimeMilliseconds(1601244000000);
    }
}
