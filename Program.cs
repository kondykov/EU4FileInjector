using EU4FileInjector.Interface;

namespace EU4FileInjector;

public static class Program
{
    private const string AppName = "EU4Injector";
    private const string Version = "v1.2";
    public static readonly string WorkDirectory = Environment.CurrentDirectory;

    public static readonly List<Option> DefaultOptions =
    [
        new Option("About", WriteAboutMessage),
        new Option("Execute injection", Inject),
        new Option("Show selected path", Injector.ShowSelectedPath),
        new Option("File manager (BETA)", () => Menu.Handle(FileManager.Options)),
        new Option("Exit", () => Environment.Exit(0))
    ];


    private static void Main(string[] args)
    {
        Console.WriteLine($"{args}");
        foreach (var arg in args)
        {
            if (!arg.Contains("--path?")) continue;
            Injector.Path = arg.Remove(0, 7);
            Console.WriteLine(arg.Remove(7));
        }

        if (args.Contains("--run"))
        {
            Inject();
            return;
        }

        Menu.Handle(DefaultOptions);
        Console.ReadKey();
    }

    public static void WriteAppInfo()
    {
        Console.WriteLine($"{AppName}. Version {Version}\n");
    }

    private static void Inject()
    {
        Console.Clear();
        Injector injector = new();
        injector.Run();
    }

    private static void WriteAboutMessage()
    {
        Console.Clear();
        WriteAppInfo();
        Console.WriteLine("Program for injecting files into EU4.");
        Thread.Sleep(3000);
        Menu.Handle(DefaultOptions);
    }

    public static void ReadGameRootFolder()
    {
        Console.Clear();
        WriteAppInfo();
        Console.CursorVisible = true;
        Console.Write("Write path to root folder: ");
        var path = Console.ReadLine();
        Console.CursorVisible = false;
        if (Directory.Exists(path))
        {
            if (Directory.GetFiles(path).Contains("eu4.exe")) return;
            Console.Clear();
            Console.WriteLine($"Path: \"{path}\".");
            /*Console.WriteLine("File \"eu4.exe\" not found. Continue? (yes/no)");

            var keyInfo = Console.ReadLine();
            if (keyInfo == "yes" || keyInfo == "y") Menu.Handle(DefaultOptions);*/
            Injector.Path = path;
            Console.WriteLine($"Path \"{path}\" selected.");
        }
        else
        {
            Console.WriteLine("Invalid path!");
            Thread.Sleep(3000);
            Menu.Handle(DefaultOptions);
        }

        Thread.Sleep(3000);
        Menu.Handle(DefaultOptions);
    }
}