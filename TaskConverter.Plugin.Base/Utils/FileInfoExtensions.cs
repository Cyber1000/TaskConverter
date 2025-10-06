using System.IO.Abstractions;
using System.IO.Compression;

namespace TaskConverter.Plugin.Base.Utils;

public static class FileInfoExtensions
{
    public static void WriteToZip(this IFileInfo zipFileInfo, string jsonString)
    {
        var zipFilePath = zipFileInfo.FullName;
        var filePath = zipFilePath[..zipFilePath.LastIndexOf('.')];
        var entryName = Path.GetFileName(filePath);

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var taskFile = archive.CreateEntry(entryName);

            using var entryStream = taskFile.Open();
            using var streamWriter = new StreamWriter(entryStream);
            streamWriter.Write(jsonString);
        }

        using var fileStream = zipFileInfo.FileSystem.File.Create(zipFilePath);
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.CopyTo(fileStream);
    }

    public static string ReadFromZip(this IFileInfo zipFileInfo)
    {
        var zipFilePath = zipFileInfo.FullName;
        var filePath = zipFilePath[..zipFilePath.LastIndexOf('.')];
        var entryName = Path.GetFileName(filePath);

        using var file = zipFileInfo.FileSystem.File.OpenRead(zipFilePath);
        using var archive = new ZipArchive(file, ZipArchiveMode.Read);
        var taskFile = archive.GetEntry(entryName);
        if (taskFile == null)
            return "";
        using var entryStream = taskFile.Open();
        using var streamReader = new StreamReader(entryStream);
        return streamReader.ReadToEnd();
    }
}
