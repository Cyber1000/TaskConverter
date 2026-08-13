using System.IO.Abstractions;
using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.MappingTests;

namespace TaskConverter.Plugin.GTD.Tests;

/// <summary>
/// Tests against <c>TestData/GTD_Fixture.json</c>: a reduced, anonymised backup produced by the
/// app itself. Titles and notes are replaced, keyword names are pseudonyms, and identical original
/// titles map to the same pseudonym so that name collisions between entity types survive.
/// <para>
/// The point of the fixture is coverage the builders do not reach: every repeat form the app
/// actually writes, all status values, hide 0 and 160, due date modifier 0 and 3, reminders of all
/// four kinds, 0 to 5 tags, parent chains, empty and multi line notes - and two structural quirks
/// that only show up in real data, a folder and a tag sharing a title, and task ids overlapping
/// notebook ids.
/// </para>
/// </summary>
public class FixtureTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    private const string FixturePath = "TestData/GTD_Fixture.json";

    private static GTDDataModel ReadFixture()
    {
        var reader = new JsonConfigurationReader(new FileSystem(), new JsonConfigurationSerializer());
        return reader.Read(FixturePath) ?? throw new Exception($"{FixturePath} could not be read.");
    }

    [Fact]
    public void Fixture_PassesValidation()
    {
        var gtdDataModel = ReadFixture();

        Assert.Equal(70, gtdDataModel.Task!.Count);
        Assert.Equal(29, gtdDataModel.Notebook!.Count);
        Assert.Equal(16, gtdDataModel.Folder!.Count);
    }

    [Fact]
    public void Fixture_CheckSourceSucceeds()
    {
        var result = new JsonConfigurationReader(new FileSystem(), new JsonConfigurationSerializer()).CheckSource(FixturePath);

        Assert.True(result.Success, result.Exception?.Message);
    }

    [Fact]
    public void Fixture_ContainsFolderAndTagWithSameTitle()
    {
        var gtdDataModel = ReadFixture();

        var sharedTitles = gtdDataModel.Folder!.Select(f => f.Title).Intersect(gtdDataModel.Tag!.Select(t => t.Title)).ToList();

        Assert.NotEmpty(sharedTitles);
    }

    [Fact]
    public void Fixture_ContainsTaskAndNotebookWithSameId()
    {
        var gtdDataModel = ReadFixture();

        var sharedIds = gtdDataModel.Task!.Select(t => t.Id).Intersect(gtdDataModel.Notebook!.Select(n => n.Id)).ToList();

        Assert.NotEmpty(sharedIds);
    }

    [Fact]
    public void Fixture_MapThroughTextKeepsEveryTaskAndKeyWord()
    {
        var gtdDataModel = ReadFixture();

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(gtdDataModel.Task!.Count, remapped!.Task!.Count);
        Assert.Equal(gtdDataModel.Notebook!.Count, remapped.Notebook!.Count);
        Assert.Equal(gtdDataModel.Context!.Select(c => c.Id).Order(), remapped.Context!.Select(c => c.Id).Order());
        Assert.Equal(gtdDataModel.Tag!.Select(t => t.Id).Order(), remapped.Tag!.Select(t => t.Id).Order());
    }

    [Fact]
    public void Fixture_MapThroughTextKeepsTaskCoreFields()
    {
        var gtdDataModel = ReadFixture();

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var original = gtdDataModel.Task!.ToDictionary(t => t.Id);
        foreach (var task in remapped!.Task!)
        {
            var expected = original[task.Id];
            Assert.Equal(expected.Title, task.Title);
            Assert.Equal(expected.DueDate, task.DueDate);
            Assert.Equal(expected.Folder, task.Folder);
            Assert.Equal(expected.Context, task.Context);
            Assert.Equal(expected.Tag.Order(), task.Tag.Order());
            Assert.Equal(expected.Parent, task.Parent);
            Assert.Equal(expected.Starred, task.Starred);
            Assert.Equal(expected.Reminder, task.Reminder);
        }
    }
}
