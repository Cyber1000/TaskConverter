namespace TaskConverter.Model.Model;

public class KeyWordEnum : ExtensibleEnum
{
    public KeyWordEnum(string value) : base(value)
    {
    }

    public static KeyWordEnum Tag = new KeyWordEnum("Tag");
}