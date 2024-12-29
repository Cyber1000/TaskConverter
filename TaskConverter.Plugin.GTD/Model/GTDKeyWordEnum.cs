using TaskConverter.Model.Model;

namespace TaskConverter.Plugin.GTD.Model;

public class GTDKeyWordEnum(string value) : KeyWordEnum(value)
{
    public static readonly KeyWordEnum Context = new("Context");
    public static readonly KeyWordEnum Folder = new("Folder");
}
