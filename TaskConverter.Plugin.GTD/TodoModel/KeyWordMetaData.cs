using System.Drawing;
using Ical.Net.DataTypes;

namespace TaskConverter.Plugin.GTD.TodoModel
{
    public enum KeyWordType
    {
        Context,
        Folder,
        Tag,
    }

    public readonly struct KeyWordMetaData(int id, string name, KeyWordType keyWordType, IDateTime created, IDateTime modified, Color? color, bool isVisible) : IEquatable<KeyWordMetaData>
    {
        public int Id { get; } = id;
        public string Name { get; } = name;
        public KeyWordType KeyWordType { get; } = keyWordType;
        public IDateTime Created { get; } = created;
        public IDateTime Modified { get; } = modified;
        public Color? Color { get; } = color;
        public bool IsVisible { get; } = isVisible;

        public bool Equals(KeyWordMetaData other) => Id == other.Id && Name == other.Name && KeyWordType == other.KeyWordType;

        public override bool Equals(object? obj) => obj is KeyWordMetaData other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Id, Name, KeyWordType);

        public static bool operator ==(KeyWordMetaData left, KeyWordMetaData right) => left.Equals(right);

        public static bool operator !=(KeyWordMetaData left, KeyWordMetaData right) => !left.Equals(right);
    }
}
