using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Text.Json.JsonDiffPatch;
using System.Xml;
using Newtonsoft.Json;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Tests.JsonTests;

public class JsonConverterTests
{
    [Theory]
    [InlineData(1234, 0)]
    [InlineData(5678, 1)]
    public void Tag_ColorAndVisible(int color, int visible)
    {
        AssertAndVerify<GTDDataModel>(
            json => json.AddTag(Create.A.JsonTag().WithColor(color).WithVisible(visible).Build()),
            taskInfo =>
            {
                Assert.Equal(visible == 1, taskInfo?.Tag?[0].Visible);
                Assert.Equal(color, taskInfo?.Tag?[0].Color);
            }
        );
    }

    [Fact]
    public void Task_DueDateFormat()
    {
        AssertAndVerify<GTDDataModel>(
            json => json.AddTask(Create.A.JsonTask().WithDueDate("2023-12-31 00:00").Build()),
            taskInfo => Assert.Equal(new LocalDateTime(2023, 12, 31, 0, 0), taskInfo?.Task?[0].DueDate)
        );
    }

    [Fact]
    public void Task_DueDateModifier()
    {
        AssertAndVerify<GTDDataModel>(
            json => json.AddTask(Create.A.JsonTask().WithDueDateModifier("3").Build()),
            taskInfo => Assert.Equal(DueDateModifier.OptionallyOn, taskInfo?.Task?[0].DueDateModifier)
        );
    }

    [Fact]
    public void Task_DueDateProjectFormat()
    {
        AssertAndVerify<GTDDataModel>(
            json => json.AddTask(Create.A.JsonTask().WithDueDateProject("2024-01-02 00:00").Build()),
            taskInfo => Assert.Equal(new LocalDateTime(2024, 1, 2, 0, 0), taskInfo?.Task?[0].DueDateProject)
        );
    }

    [Fact]
    public void Task_DueTimeSet()
    {
        AssertAndVerify<GTDDataModel>(json => json.AddTask(Create.A.JsonTask().WithDueTimeSet(1).Build()), taskInfo => Assert.Equal(true, taskInfo?.Task?[0].DueTimeSet));
    }

    [Fact]
    public void Task_ReminderValue()
    {
        // Unix timestamp for 2017-11-14 12:00:00 UTC
        const long reminderUnixTime = 1510642800000;
        AssertAndVerify<GTDDataModel>(json => json.AddTask(Create.A.JsonTask().WithReminder(reminderUnixTime).Build()), taskInfo => Assert.Equal(reminderUnixTime, taskInfo?.Task?[0].Reminder));
    }

    [Fact]
    public void Task_Alarm()
    {
        AssertAndVerify<GTDDataModel>(
            json => json.AddTask(Create.A.JsonTask().WithAlarm("2023-12-31 00:00").Build()),
            taskInfo => Assert.Equal(new LocalDateTime(2023, 12, 31, 0, 0), taskInfo?.Task?[0].Alarm)
        );
    }

    [Fact]
    public void Task_RepeatNewWithNoRepeat_ShouldBeNull()
    {
        AssertAndVerify<GTDDataModel>(json => json.AddTask(Create.A.JsonTask().WithRepeatNew("Norepeat").Build()), taskInfo => Assert.Null(taskInfo?.Task?[0].RepeatNew));
    }

    [Theory]
    [InlineData("Every 1 day")]
    [InlineData("Daily")]
    [InlineData("Every 1 week")]
    [InlineData("Weekly")]
    [InlineData("Biweekly")]
    [InlineData("Every 2 weeks")]
    [InlineData("Monthly")]
    [InlineData("Bimonthly")]
    [InlineData("Quarterly")]
    [InlineData("Semiannually")]
    [InlineData("Yearly")]
    public void Task_RepeatNew(string repeatModifier)
    {
        AssertAndVerify<GTDDataModel>(json => json.AddTask(Create.A.JsonTask().WithRepeatNew(repeatModifier).Build()), taskInfo => Assert.NotNull(taskInfo?.Task?[0].RepeatNew));
    }

    [Fact]
    public void Preferences_ShouldPreserveOriginalJson()
    {
        AssertAndVerify<GTDDataModel>(json => json.WithPreferences(JsonDataBuilder.DefaultXmlString), taskInfo => { });
    }

    [Fact]
    public void Preferences_ShouldPreserveXmlStructure()
    {
        AssertAndVerify<GTDDataModel>(
            json => json.WithPreferences(JsonDataBuilder.DefaultXmlString),
            taskInfo =>
            {
                XmlDocument originalXml = new();
                originalXml.LoadXml(JsonDataBuilder.DefaultXmlString);
                var recreatedXml = taskInfo!.Preferences![0].XmlConfig;

                var (hasError, xmlDiff) = XmlUtil.DiffXml(originalXml, recreatedXml!);
                Assert.False(hasError, $"Xml differs: {xmlDiff}");
            }
        );
    }

    private static void AssertAndVerify<T>(Func<JsonDataBuilder, JsonDataBuilder> buildJson, Action<T?> verifyAction)
        where T : class
    {
        var originalJson = buildJson(Create.A.JsonData()).Build();
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile("GTD.json", new MockFileData(originalJson));
        var jsonReader = new JsonConfigurationReader(mockFileSystem, new TestJsonConfigurationSerializer());
        var model = jsonReader.Read("GTD.json");
        var jsonWriter = new JsonConfigurationWriter(mockFileSystem, new TestJsonConfigurationSerializer());
        jsonWriter.Write("outfile", model);
        var outFile = mockFileSystem.GetFile("outfile");
        var recreatedJson = outFile.TextContents;
        AssertJsonEqual(originalJson, recreatedJson);

        verifyAction(jsonReader.Read("GTD.json") as T);
    }

    private static void AssertJsonEqual(string expectedJson, string actualJson)
    {
        var expectedBytes = Encoding.UTF8.GetBytes(JsonUtil.NormalizeText(expectedJson));
        var actualBytes = Encoding.UTF8.GetBytes(actualJson);

        var diff = JsonDiffPatcher.Diff(expectedBytes, actualBytes, JsonUtil.GetDiffOptions());
        Assert.Null(diff);
    }
}
