namespace TaskConverter.Plugin.Base
{
    public interface IReader<T>
    {
        T Result { get; }

        (bool isError, string validationError) CheckSource();
    }
}
