using Application.Interfaces;

namespace Infrastructure.Database;

public class SqlFileReader
{
    private readonly string _basePath;

    public SqlFileReader()
    {
        _basePath = Path.Combine(
            AppContext.BaseDirectory,
            "SQL");
    }

    public string Get(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException(
                $"Arquivo SQL não encontrado: {fullPath}");

        return File.ReadAllText(fullPath);
    }
}