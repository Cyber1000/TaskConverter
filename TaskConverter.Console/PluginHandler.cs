using McMaster.NETCore.Plugins;

namespace TaskConverter.Console.PluginHandling;

class PluginHandler(string pluginBaseDir)
{
    public IEnumerable<T> GetAllCommands<T>(params object[] args)
    {
        foreach (var dir in Directory.GetDirectories(pluginBaseDir))
        {
            var dirName = Path.GetFileName(dir);
            var pluginDll = Path.Combine(dir, dirName + ".dll");
            if (File.Exists(pluginDll))
            {
                var loader = PluginLoader.CreateFromAssemblyFile(pluginDll, sharedTypes: [typeof(T)]);
                var commands = GetPluginCommands<T>(loader, args);
                foreach (var command in commands)
                {
                    yield return command;
                }
            }
        }
    }

    private static IEnumerable<T> GetPluginCommands<T>(PluginLoader loader, params object[] args)
    {
        foreach (Type type in loader.LoadDefaultAssembly().GetTypes().Where(t => typeof(T).IsAssignableFrom(t)))
        {
            T? result = (T?)Activator.CreateInstance(type, args);
            if (result != null)
            {
                yield return result;
            }
        }
    }
}
