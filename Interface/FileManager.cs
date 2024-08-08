namespace EU4FileInjector.Interface;

public class FileManager
{
    public static readonly List<Option> Options =
    [
        new Option("Test option", () => GenerateOptions(@"C:\Users\kondy\Desktop\goodbyedpi-0.2.2")),
        new Option("Back", () => Menu.Handle(Program.DefaultOptions))
    ];

    private static void WriteMenu(List<Option> options, Option selectedOption)
    {
        Console.SetCursorPosition(0, 0);
        Program.WriteAppInfo();

        Console.WriteLine($"Left arrow \"exit from folder\" | Enter \"select folder or file\"");
        Console.WriteLine($"Path \"{options[0].Name}\"");

        foreach (var option in options)
            if (option == selectedOption)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                try
                {
                    if (option.Name.Split(Path.DirectorySeparatorChar)[1] == string.Empty)
                        throw new Exception();
                    Console.WriteLine($"{option.Name.Split(Path.DirectorySeparatorChar).Last()}");
                }
                catch
                {
                    Console.WriteLine($"{option.Name}");
                }

                Console.ResetColor();
            }
            else
            {
                try
                {
                    if (option.Name.Split(Path.DirectorySeparatorChar)[1] == string.Empty)
                        throw new Exception();
                    Console.WriteLine(option.Name.Split(Path.DirectorySeparatorChar).Last());
                }
                catch
                {
                    Console.WriteLine($"{option.Name}");
                }
            }
    }

    public static void Handle(List<Option> options, bool isDriverOptions = false)
    {
        if (options.Count <= 0) return;
        Console.Clear();
        Console.CursorVisible = false;
        var index = 0;
        WriteMenu(options, options[index]);
        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.DownArrow:
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }

                    break;
                case ConsoleKey.UpArrow:
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }

                    break;
                case ConsoleKey.LeftArrow:
                    BackToParentFolder(options[index].Name);
                    break;
                case ConsoleKey.RightArrow:
                    try
                    {
                        Directory.GetDirectories(options[index].Name);
                        OpenFolder(options[index].Name);
                    }
                    catch
                    {
                        SelectFile(options[index].Name);
                    }

                    break;
                case ConsoleKey.Enter:
                    options[index].Selected.Invoke();
                    index = 0;
                    break;
            }
        } while (keyInfo.Key != ConsoleKey.X);
    }

    private static DriveInfo[] GetDrivers()
    {
        return DriveInfo.GetDrives();
    }

    private static void OpenFolder(string path, bool isDriver = false)
    {
        Console.WriteLine($"Folder {path.Split(Path.DirectorySeparatorChar).Last()}");
        GenerateOptions(path);
    }

    private static void BackToParentFolder(string path)
    {
        try
        {
            var last = path.Split(Path.DirectorySeparatorChar).Last();
            path = path.Split(Path.DirectorySeparatorChar).TakeWhile(p => p != last)
                .Aggregate("", (current, p) => current + (p + Path.DirectorySeparatorChar));
            path = path.Remove(path.Length - 1);
            last = path.Split(Path.DirectorySeparatorChar).Last();
            path = path.Split(Path.DirectorySeparatorChar).TakeWhile(p => p != last)
                .Aggregate("", (current, p) => current + (p + Path.DirectorySeparatorChar));
            GenerateOptions(path);
        }
        catch
        {
            List<Option> options = [];
            options.AddRange(GetDrivers().Select(driver => new Option(driver.Name, () => OpenFolder(driver.Name))));
            Handle(options);
        }
    }

    private static void SelectFile(string path)
    {
        Console.WriteLine($"File {path.Split(Path.DirectorySeparatorChar).Last()}");
    }

    private static void GenerateOptions(string? path)
    {
        var folders = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        var options = folders.Select(folder => new Option(folder, () => OpenFolder(folder))).ToList();
        options.AddRange(files.Select(file => new Option(file, () => SelectFile(file))));
        Handle(options);
        /*try
        {
            if (path?.Split(Path.DirectorySeparatorChar)[1] == string.Empty)
                throw new Exception();
        }
        catch
        {
            List<Option> options = [];
            options.AddRange(GetDrivers().Select(driver => new Option(driver.Name, () => OpenFolder(driver.Name))));
            Handle(options, true);
        }*/
    }
}