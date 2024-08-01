﻿namespace EU4FileInjector;

public class Injector
{
    //private const string DebugGamePath = @"L:\Steam\steamapps\common\Europa Universalis IV";
    private static readonly string WorkDirectory = Environment.CurrentDirectory;
    private const string DebugGamePath = @"C:\Users\kondy\OneDrive\Рабочий стол\Новая папка";
    private const string ExecutableFileName = "eu4.exe";

    private static readonly string AppdataLocalPath =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    private readonly List<string> _injectionFilesPaths =
    [
        @"Injection\cream_api.ini",
        @"Injection\steam_api.dll",
        @"Injection\steam_api64.dll",
        @"Injection\steam_api64_o.dll",
    ];

    private readonly List<string> _localAppDataLauncherFolders = [];

    private void CheckExistsInjectionFiles()
    {
        if (_injectionFilesPaths.Select(File.Exists).Any(e => e == false))
            throw new FileNotFoundException();
    }

    private void FindLocalAppdataLauncherFolders()
    {
        var directories = Directory.GetDirectories($@"{AppdataLocalPath}\Programs\Paradox Interactive\launcher");
        foreach (var directory in directories)
            if (directory.Contains("launcher-v"))
                _localAppDataLauncherFolders.Add(directory);
    }

    private void Inject()
    {
        CheckExistsInjectionFiles();
        FindLocalAppdataLauncherFolders();
        if (_localAppDataLauncherFolders.Count <= 0)
            throw new DirectoryNotFoundException();

        foreach (var filePath in _injectionFilesPaths)
        {
            var last = filePath.Split(Path.DirectorySeparatorChar).Last();
            File.Copy($@"{WorkDirectory}\{filePath}", $@"{DebugGamePath}\{last}", true);
            foreach (var launcherFolder in _localAppDataLauncherFolders)
                File.Copy($@"{WorkDirectory}\{filePath}", $@"{launcherFolder}\resources\app\dist\main\{last}", true);
            Console.WriteLine($"File {filePath.Split(Path.DirectorySeparatorChar).Last() } injected.");
        }
    }

    public void Run()
    {
        var files = Directory.GetFiles(DebugGamePath);
        foreach (var file in files)
        {
            var last = file.Split(Path.DirectorySeparatorChar).Last();
            if (last != ExecutableFileName) continue;
            Console.WriteLine($"Executable file {last} found.");
            Inject();
        }
    }
}