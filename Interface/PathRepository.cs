namespace EU4FileInjector.Interface;

public static class PathRepository
{
    private const string FileName = "SavedPath.txt";

    public static readonly List<Option> SelectionOptions =
    [
        new Option("Select saved path", () => Menu.Handle(Program.DefaultOptions)),
        new Option("Open file manager", () => FileManager.Handle(FileManager.Options))
    ];
    public static string? GetSavedPathOrNull()
    {
        if (!File.Exists($@"{Program.WorkDirectory}\{FileName}")) return null;
        var file = File.ReadAllTextAsync($@"{Program.WorkDirectory}\{FileName}");
        return file.Result;
    }

    public static void SavePath(string? path)
    {
        if (File.Exists(path))
        {
            File.WriteAllText($@"{Program.WorkDirectory}\{FileName}", path);
            return;
        }

        try
        {
            var tnp = Directory.GetDirectories(path);
            File.WriteAllText($@"{Program.WorkDirectory}\{FileName}", path);
        }
        catch
        {
            return;
        }
    }

    public static bool SavedPathExists()
    {
        return GetSavedPathOrNull() != null;
    }
}