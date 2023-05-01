using System.IO.Compression;

namespace TaskConverter.Commons.Utils;

public static class FileInfoExtensions
{
    public static void WriteToZip(this FileInfo zipFileInfo, string jsonString)
    {
        var zipFilePath = zipFileInfo.FullName;
        var filePath = zipFilePath[..zipFilePath.LastIndexOf(".")];
        var entryName = Path.GetFileName(filePath);

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var taskFile = archive.CreateEntry(entryName);

            using var entryStream = taskFile.Open();
            using var streamWriter = new StreamWriter(entryStream);
            streamWriter.Write(jsonString);
        }

        using var fileStream = new FileStream(zipFilePath, FileMode.Create);
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.CopyTo(fileStream);
    }

    public static string ReadFromZip(this FileInfo zipFileInfo)
    {
        var zipFilePath = zipFileInfo.FullName;
        var filePath = zipFilePath[..zipFilePath.LastIndexOf(".")];
        var entryName = Path.GetFileName(filePath);

        using var file = File.OpenRead(zipFilePath);
        using var archive = new ZipArchive(file, ZipArchiveMode.Read);
        var taskFile = archive.GetEntry(entryName);
        if (taskFile == null)
            return "";
        using var entryStream = taskFile.Open();
        using var streamReader = new StreamReader(entryStream);
        return streamReader.ReadToEnd();
    }
}
