using EU4FileInjector.Interface;

namespace EU4FileInjector;

public static class Program
{
    public const string AppName = "EU4Injector";
    public const string Version = "v1.0"; 
    public static readonly List<Option> DefaultOptions = 
    [
        new Option("About", WriteAboutMessage),
        new Option("Select game folder", ReadGameRootFolder),
        new Option("Execute injection", Inject),
        new Option("Exit", () => Environment.Exit(0))
    ];


    private static void Main(string[] args)
    {
        Console.WriteLine($"{args}");
        if (args.Contains("--run"))
        {
            Inject();
            return;
        }
        Menu.Run(DefaultOptions);
        Console.ReadKey();
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
        Console.WriteLine("Program for injecting files into EU4.");
        Thread.Sleep(3000);
        Menu.WriteMenu(DefaultOptions, DefaultOptions.First());
    }
    
    private static void ReadGameRootFolder()
    {
        Console.Clear();
        Console.Write("Write path to root folder: ");
        var path = Console.ReadLine();
        if (Directory.Exists(path))
        {
            if (!Directory.GetFiles(path).Contains("eu4.exe"))
            {
                Console.Clear();
                Console.WriteLine($"Path: \"{path}\".");
                Console.WriteLine("File \"eu4.exe\" not found. Continue? (yes/no)");
                var key = Console.ReadLine();
                if (key != "yes") Menu.Run(DefaultOptions);
                Injector.Path = path;
            }
            Inject();
        }
        else
        {
            Console.WriteLine("Invalid path!");
            Thread.Sleep(3000);
            Menu.Run(DefaultOptions);
        }
    }
}