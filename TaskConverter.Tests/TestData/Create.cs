namespace TaskConverter.Tests.TestData;

public class Create : IObjectBuilder
{
    public static IObjectBuilder A { get; } = new Create();
    public static IObjectBuilder An => A;
}

public interface IObjectBuilder { }

public interface IBuilder { }

public interface IBuilder<out T> : IBuilder
{
    T Build();
}
