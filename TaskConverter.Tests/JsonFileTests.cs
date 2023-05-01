using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD;

namespace TaskConverter.Tests;

public class JsonFileTests
{
    private const string originalFilePath = "./GTD.json";
    private const string resultFilePath = "./GTD_new.json";

    [Fact]
    public void ReadAndWrite_Json_ShouldBeValid()
    {
        var originalJson =
            "{\"version\": 3, \"TASK\": [{\"ID\": 8, \"UUID\": \"\", \"PARENT\": 0, \"CREATED\": \"2013-04-18 06:04:48.334\", \"MODIFIED\": \"2022-05-01 10:24:01.774\", "
            + "\"TITLE\": \"Test\", \"START_DATE\": \"\", \"START_TIME_SET\": 0, \"DUE_DATE\": \"2022-07-31 00:00\", \"DUE_DATE_PROJECT\": "
            + "\"2022-07-31 00:00\", \"DUE_TIME_SET\": 0, \"DUE_DATE_MODIFIER\": \"0\", \"REMINDER\": -1, \"ALARM\": \"\", \"REPEAT_NEW\": \"Every 1 week\", "
            + "\"REPEAT_FROM\": 0, \"DURATION\": 0, \"STATUS\": 1, \"CONTEXT\": 7, \"GOAL\": 0, \"FOLDER\": 8, \"TAG\": [8], \"STARRED\": 0, "
            + "\"PRIORITY\": 0, \"NOTE\": \"\", \"COMPLETED\": \"\", \"TYPE\": 1, \"TRASH_BIN\": \"\", \"IMPORTANCE\": 0, \"METAINF\": \"\", "
            + "\"FLOATING\": 0, \"HIDE\": 160, \"HIDE_UNTIL\": 1643583600000}] }";
        var originalFile = new FileInfo(originalFilePath);
        var resultFile = new FileInfo(resultFilePath);

        File.WriteAllText(originalFilePath, originalJson);

        var jsonReader = new JsonConfigurationReader(originalFile);
        jsonReader.Write(resultFile);
    }

    [Fact]
    public void ReadAndWrite_Zip_ShouldBeValid()
    {
        var originalJson =
            "{\"version\": 3, \"TASK\": [{\"ID\": 8, \"UUID\": \"\", \"PARENT\": 0, \"CREATED\": \"2013-04-18 06:04:48.334\", \"MODIFIED\": \"2022-05-01 10:24:01.774\", "
            + "\"TITLE\": \"Test\", \"START_DATE\": \"\", \"START_TIME_SET\": 0, \"DUE_DATE\": \"2022-07-31 00:00\", \"DUE_DATE_PROJECT\": "
            + "\"2022-07-31 00:00\", \"DUE_TIME_SET\": 0, \"DUE_DATE_MODIFIER\": \"0\", \"REMINDER\": -1, \"ALARM\": \"\", \"REPEAT_NEW\": \"Every 1 week\", "
            + "\"REPEAT_FROM\": 0, \"DURATION\": 0, \"STATUS\": 1, \"CONTEXT\": 7, \"GOAL\": 0, \"FOLDER\": 8, \"TAG\": [8], \"STARRED\": 0, "
            + "\"PRIORITY\": 0, \"NOTE\": \"\", \"COMPLETED\": \"\", \"TYPE\": 1, \"TRASH_BIN\": \"\", \"IMPORTANCE\": 0, \"METAINF\": \"\", "
            + "\"FLOATING\": 0, \"HIDE\": 160, \"HIDE_UNTIL\": 1643583600000}] }";
        var zipFile = new FileInfo($"{originalFilePath}.zip");
        var resultFile = new FileInfo(resultFilePath);

        zipFile.WriteToZip(originalJson);

        var jsonReader = new JsonConfigurationReader(zipFile);
        jsonReader.Write(resultFile);
    }
}
