public interface IFileManager
{
    public string CreateZipFile(string[] fileNames);

    public void CreateTextFile(string fileName, string content);
    public void ExtractZipFile(string zipFileName, string destinationFolder);

    public void DeleteFile(string[] fileName);
    public string GetFolderFiles();
}