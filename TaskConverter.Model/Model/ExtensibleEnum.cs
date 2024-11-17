namespace TaskConverter.Model.Model;

public abstract class ExtensibleEnum(string value)
{
    public string Value { get; } = value; 
    public override string ToString() => Value;
}