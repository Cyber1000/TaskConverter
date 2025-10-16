using System.Drawing;
using Ical.Net.DataTypes;
using TaskConverter.Plugin.Base.ConversionHelper;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public class KeyWordMetaDataExtensionTests
{
    [Fact]
    public void GetStringRepresentation_SerializesAndDeserializesCorrectly()
    {
        var created = new CalDateTime(new DateTime(2025, 10, 16, 12, 30, 0, DateTimeKind.Utc));
        var modified = new CalDateTime(new DateTime(2025, 10, 17, 15, 45, 0, DateTimeKind.Utc));
        var color = Color.FromArgb(255, 10, 20, 30);
        var original = new KeyWordMetaData(
            id: 1,
            name: "TestName",
            nameWithType: "Type-TestName",
            keyWordType: KeyWordType.Context,
            created: created,
            modified: modified,
            color: color,
            isVisible: true
        );

        var json = original.GetStringRepresentation();
        var deserialized = json.KeyWordMetaDataFromString();

        Assert.NotNull(deserialized);
        Assert.Equal(original.Id, deserialized.Value.Id);
        Assert.Equal(original.Name, deserialized.Value.Name);
        Assert.Equal(original.KeyWordType, deserialized.Value.KeyWordType);
        Assert.Equal(original.Created.Value, deserialized.Value.Created.Value);
        Assert.Equal(original.Modified.Value, deserialized.Value.Modified.Value);
        Assert.Equal(original.Color, deserialized.Value.Color);
        Assert.Equal(original.IsVisible, deserialized.Value.IsVisible);
    }

    [Fact]
    public void KeyWordMetaDataFromString_ReturnsNullForNullOrEmptyInput()
    {
        Assert.Null(((string?)null).KeyWordMetaDataFromString());
        Assert.Null(string.Empty.KeyWordMetaDataFromString());
        Assert.Null("  ".KeyWordMetaDataFromString());
    }

    [Fact]
    public void KeyWordMetaDataFromString_ReturnsNullForInvalidJson()
    {
        var invalidJson = "{ invalid json }";

        var result = invalidJson.KeyWordMetaDataFromString();

        Assert.Null(result);
    }

    [Fact]
    public void ColorSerializationAndDeserialization_ShouldBeConsistent()
    {
        var colorOriginal = Color.FromArgb(128, 50, 100, 150);
        var serialized = colorOriginal.ToStringRepresentation();
        var deserialized = serialized.ColorFromStringRepresentation();

        Assert.NotNull(deserialized);
        Assert.Equal(colorOriginal.ToArgb(), deserialized.Value.ToArgb());
    }

    [Theory]
    [InlineData("True", true)]
    [InlineData("False", false)]
    [InlineData(null, false)]
    [InlineData("", false)]
    public void IsVisibleToBool_StringConversions(string input, bool expected)
    {
        var result = input.ToBool();

        Assert.Equal(expected, result);
    }
}
