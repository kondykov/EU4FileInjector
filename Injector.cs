using EU4FileInjector.Interface;

namespace EU4FileInjector;

public class Injector
{
    private const string ExecutableFileName = "eu4.exe";

    private static readonly string AppdataLocalPath =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    private readonly List<string> _injectionFilesPaths =
    [
        @"Injection\cream_api.ini",
        @"Injection\steam_api.dll",
        @"Injection\steam_api64.dll",
        @"Injection\steam_api64_o.dll"
    ];

    private readonly List<string> _localAppDataLauncherFolders = [];
    public static string Path { set; get; } = null!;

    private bool CheckExistsInjectionFiles()
    {
        if (Path == null!)
        {
            Console.Clear();
            Program.WriteAppInfo();
            Console.WriteLine("The path is not chosen.");
            Thread.Sleep(3000);
            return false;
        }

        if (_injectionFilesPaths.Select(File.Exists).Any(e => e == false))
        {
            Console.Clear();
            Program.WriteAppInfo();
            Console.WriteLine("The injection folder is empty. Aborted.");
            Thread.Sleep(3000);
            return false;
        }
        PathRepository.SavePath(Path);
        return true;
    }

    private void FindLocalAppdataLauncherFolders()
    {
        try
        {
            var directories = Directory.GetDirectories($@"{AppdataLocalPath}\Programs\Paradox Interactive\launcher");
            foreach (var directory in directories)
                if (directory.Contains("launcher-v"))
                    _localAppDataLauncherFolders.Add(directory);
        }
        catch (Exception e)
        {
            Console.Clear();
            Program.WriteAppInfo();
            Console.WriteLine(e.Message);
            Thread.Sleep(5000);
            Menu.Handle(Program.DefaultOptions);
        }
    }

    private void Inject()
    {
        if (_localAppDataLauncherFolders.Count <= 0)
        {
            Console.WriteLine("");
            return;
        }

        foreach (var filePath in _injectionFilesPaths)
        {
            var last = filePath.Split(System.IO.Path.DirectorySeparatorChar).Last();
            File.Copy($@"{Program.WorkDirectory}\{filePath}", $@"{Path}\{last}", true);
            foreach (var launcherFolder in _localAppDataLauncherFolders)
                File.Copy($@"{Program.WorkDirectory}\{filePath}", $@"{launcherFolder}\resources\app\dist\main\{last}", true);
            Console.WriteLine($"File {filePath.Split(System.IO.Path.DirectorySeparatorChar).Last()} injected.");
        }

        Menu.Handle(Program.DefaultOptions);
    }

    // TODO: Доделать ijnection
    public void Run()
    {
        if (!CheckExistsInjectionFiles()) Menu.Handle(Program.DefaultOptions);
        FindLocalAppdataLauncherFolders();
        var files = Directory.GetFiles(Path);
        foreach (var file in files)
        {
            var last = file.Split(System.IO.Path.DirectorySeparatorChar).Last();
            if (last != ExecutableFileName) continue;
            Console.WriteLine($"Executable file {last} found.");
            Inject();
        }
        Console.Clear();
        Program.WriteAppInfo();
        Console.WriteLine("Finished!");
        Thread.Sleep(3000);
        Menu.Handle(Program.DefaultOptions);
    }
}