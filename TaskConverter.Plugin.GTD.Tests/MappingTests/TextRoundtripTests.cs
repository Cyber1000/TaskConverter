using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

/// <summary>
/// Roundtrips that pass through real iCalendar text. The mapping-only tests cannot cover
/// these: ical.net uppercases property names when parsing, so anything looked up by a
/// mixed-case property name behaves differently once text is involved.
/// </summary>
public class TextRoundtripTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    [Fact]
    public void MapThroughText_KeepsKeyWordMetaData()
    {
        var gtdDataModel = Create
            .A.GTDDataModel()
            .AddFolder(TestConstants.DefaultFolderId)
            .AddContext(TestConstants.DefaultContextId)
            .AddTag(TestConstants.DefaultTagId)
            .AddTaskList(() =>
                [
                    Create
                        .A.GTDTaskModel(TestConstants.DefaultTaskId)
                        .WithFolder(TestConstants.DefaultFolderId)
                        .WithContext(TestConstants.DefaultContextId)
                        .WithTags([TestConstants.DefaultTagId])
                        .Build(),
                ]
            )
            .Build();

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var originalFolder = gtdDataModel.Folder!.Single();
        var remappedFolder = Assert.Single(remapped!.Folder!);
        Assert.Equal(originalFolder.Id, remappedFolder.Id);
        Assert.Equal(originalFolder.Color, remappedFolder.Color);
        Assert.Equal(originalFolder.Created, remappedFolder.Created);
        Assert.Equal(originalFolder.Modified, remappedFolder.Modified);
        Assert.Equal(originalFolder.Visible, remappedFolder.Visible);

        var originalTag = gtdDataModel.Tag!.Single();
        var remappedTag = Assert.Single(remapped.Tag!);
        Assert.Equal(originalTag.Id, remappedTag.Id);
        Assert.Equal(originalTag.Color, remappedTag.Color);
    }

    [Fact]
    public void MapThroughText_KeepsTaskKeyWordAssignment()
    {
        var gtdDataModel = Create
            .A.GTDDataModel()
            .AddFolder(TestConstants.DefaultFolderId)
            .AddContext(TestConstants.DefaultContextId)
            .AddTag(TestConstants.DefaultTagId)
            .AddTaskList(() =>
                [
                    Create
                        .A.GTDTaskModel(TestConstants.DefaultTaskId)
                        .WithFolder(TestConstants.DefaultFolderId)
                        .WithContext(TestConstants.DefaultContextId)
                        .WithTags([TestConstants.DefaultTagId])
                        .Build(),
                ]
            )
            .Build();

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var remappedTask = Assert.Single(remapped!.Task!);
        Assert.Equal(TestConstants.DefaultFolderId, remappedTask.Folder);
        Assert.Equal(TestConstants.DefaultContextId, remappedTask.Context);
        Assert.Equal([TestConstants.DefaultTagId], remappedTask.Tag);
    }

    /// <summary>
    /// A folder and a tag may carry the same title. With an empty GTD folder symbol - which is
    /// what App.config ships - both end up with the same keyword name, so the name alone must
    /// not be used as an identity.
    /// </summary>
    [Fact]
    public void MapThroughText_FolderAndTagWithSameTitle()
    {
        CurrentSettingsProvider.SetGTDFormatSymbol(KeyWordType.Folder, "");
        const string sharedTitle = "Ideas";

        var gtdDataModel = Create
            .A.GTDDataModel()
            .AddFolder(TestConstants.DefaultFolderId, sharedTitle)
            .AddTag(TestConstants.DefaultTagId, sharedTitle)
            .AddTaskList(() =>
                [
                    Create.A.GTDTaskModel(TestConstants.DefaultTaskId).WithFolder(TestConstants.DefaultFolderId).Build(),
                    Create.A.GTDTaskModel(TestConstants.DefaultTaskId + 1).WithTags([TestConstants.DefaultTagId]).Build(),
                ]
            )
            .Build();

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(sharedTitle, Assert.Single(remapped!.Folder!).Title);
        Assert.Equal(sharedTitle, Assert.Single(remapped.Tag!).Title);
    }
}
