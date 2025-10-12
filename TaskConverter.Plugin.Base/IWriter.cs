namespace TaskConverter.Plugin.Base;

public interface IWriter<T>
{
    void Write(string destination, T model);
}
