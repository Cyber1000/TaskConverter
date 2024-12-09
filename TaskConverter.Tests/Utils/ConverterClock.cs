using NodaTime;

namespace TaskConverter.Tests.Utils;

public class ConverterClock : IClock
{
    public Instant GetCurrentInstant()
    {
        return Instant.FromUnixTimeMilliseconds(1601244000000);
    }
}
