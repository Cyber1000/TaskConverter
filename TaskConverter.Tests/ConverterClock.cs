using NodaTime;

namespace TaskConverter.Tests;

public class ConverterClock : IClock
{
    public Instant GetCurrentInstant()
    {
        return Instant.FromUnixTimeMilliseconds(1601244000000);
    }
}
