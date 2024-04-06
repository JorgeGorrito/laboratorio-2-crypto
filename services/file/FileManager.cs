using System.IO.Compression;

public class FileManager : IFileManager
{
    private string folderFiles;
    public FileManager(string folderFiles)
    {
        this.folderFiles = folderFiles;
        Directory.CreateDirectory(folderFiles);
    }

    public string GetFolderFiles()
    {
        return folderFiles;
    }


    public void CreateTextFile(string fileName, string content)
    {
        // Obtener la ruta completa del archivo
        string filePath = Path.Combine(folderFiles, fileName);

        // Escribir el contenido en el archivo
        File.WriteAllText(filePath, content);
    }

    public string CreateZipFile(string[] fileNames)
    {
        // Crear el nombre y la ruta del archivo ZIP
        string zipFileName = $"{DateTime.Now:dd-MM-yyyy-HH.mm.ss}.zip";
        string zipFilePath = Path.Combine(folderFiles, zipFileName);

        // Crear el archivo ZIP y agregar los archivos temporales
        using (ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
        {
            foreach (var fileName in fileNames)
            {
                string filePath = Path.Combine(folderFiles, fileName);
                zip.CreateEntryFromFile(filePath, fileName);
            }
        }

        return zipFilePath;
    }


    public void ExtractZipFile(string zipFileName, string destinationFolder)
    {
        string zipFilePath = Path.Combine(folderFiles, zipFileName);
        ZipFile.ExtractToDirectory(zipFilePath, destinationFolder);
    }

    public void DeleteFile(string[] filesName)
    {
        foreach(var fileName in filesName)
        {
            string filePath = Path.Combine(folderFiles, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}