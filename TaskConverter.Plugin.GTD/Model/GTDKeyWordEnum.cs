using TaskConverter.Model.Model;

namespace TaskConverter.Plugin.GTD.Model;

public class GTDKeyWordEnum : KeyWordEnum
{
    public GTDKeyWordEnum(string value) : base(value)
    {
    }

        public static KeyWordEnum Context = new KeyWordEnum("Context");
        public static KeyWordEnum Folder = new KeyWordEnum("Folder");
}