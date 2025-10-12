using TaskConverter.Commons;

namespace TaskConverter.Plugin.Base;

public interface IReader<T>
{
    T Read(string source);

    SourceResult CheckSource(string source);
}
