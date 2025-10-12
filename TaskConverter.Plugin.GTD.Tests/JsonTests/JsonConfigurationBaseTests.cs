namespace TaskConverter.Plugin.GTD.Tests.JsonTests;

public abstract class JsonConfigurationBaseTests
{
    protected static byte[] CreateZipContent(string filePath, string content)
    {
        using var zipStream = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
        {
            var entry = archive.CreateEntry(Path.GetFileNameWithoutExtension(filePath));
            using var entryStream = entry.Open();
            using var writer = new StreamWriter(entryStream);
            writer.Write(content);
        }
        return zipStream.ToArray();
    }
}
