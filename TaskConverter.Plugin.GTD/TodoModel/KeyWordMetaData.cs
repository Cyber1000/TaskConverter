using System.Drawing;
using Ical.Net.DataTypes;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.TodoModel
{
    public enum KeyWordType
    {
        Context,
        Folder,
        Tag,
        Status,
    }

    public readonly struct KeyWordMetaData(int id, string name, string nameWithType, KeyWordType keyWordType, CalDateTime created, CalDateTime modified, Color? color, bool isVisible)
        : IEquatable<KeyWordMetaData>
    {
        public int Id { get; } = id;
        public string Name { get; } = name;
        public string NameWithType { get; } = nameWithType;
        public KeyWordType KeyWordType { get; } = keyWordType;
        public CalDateTime Created { get; } = created;
        public CalDateTime Modified { get; } = modified;
        public Color? Color { get; } = color;
        public bool IsVisible { get; } = isVisible;

        public bool Equals(KeyWordMetaData other) => Id == other.Id && Name == other.Name && KeyWordType == other.KeyWordType;

        public override bool Equals(object? obj) => obj is KeyWordMetaData other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Id, Name, KeyWordType);

        public static bool operator ==(KeyWordMetaData left, KeyWordMetaData right) => left.Equals(right);

        public static bool operator !=(KeyWordMetaData left, KeyWordMetaData right) => !left.Equals(right);

        public override string ToString() => this.GetStringRepresentation();

        public static KeyWordMetaData? FromString(string? json) => json.KeyWordMetaDataFromString();
    }
}
