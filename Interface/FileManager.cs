namespace EU4FileInjector.Interface;

public static class FileManager
{
    public static readonly List<Option> Options =
    [
        new Option("Select game folder", Program.ReadGameRootFolder),
        new Option("Open file manager", GetDriversOptions),
        new Option("Back", () => Menu.Handle(Program.DefaultOptions))
    ];

    private static void WriteMenu(List<Option> options, Option selectedOption)
    {
        if (options.Count >= Console.BufferHeight - 5) Console.Clear();
        Console.SetCursorPosition(0, 0);
        Program.WriteAppInfo();
        Console.WriteLine(
            "\"Left arrow\" exit from folder | \"Enter\" into folder or select file | \"Right arrow\" select folder or file");
        Console.WriteLine();
        var last = options[0].Name.Split(Path.DirectorySeparatorChar).Last();
        var path = options[0].Name.Remove(options[0].Name.Length - last.Length);
        Console.WriteLine(options[0].Name.Length <= 3 ? "Select driver: " : $"Path \"{path}\"");

        foreach (var option in options)
            if (option == selectedOption)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
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
                    catch (Exception e)
                    {
                        if (e.Message.Contains("is denied"))
                        {
                            var cursorPos = Console.GetCursorPosition();
                            Console.SetCursorPosition(0, 3);
                            Console.WriteLine(e.Message);
                            Console.SetCursorPosition(cursorPos.Left, cursorPos.Top);
                            break;
                        }

                        SelectPath(options[index].Name);
                    }

                    break;
                case ConsoleKey.Enter:
                    if (!IsAvailable(options[index].Name)) break;
                    SelectPath(options[index].Name);
                    index = 0;
                    break;
            }
        } while (keyInfo.Key != ConsoleKey.X);
    }

    private static void SelectPath(string path)
    {
        Injector.Path = path;
        Console.Clear();
        Program.WriteAppInfo();
        Console.WriteLine($"\nSelected {path}.");
        Thread.Sleep(3000);
        Menu.Handle(Program.DefaultOptions);
    }

    private static DriveInfo[] GetDrivers()
    {
        return DriveInfo.GetDrives();
    }

    private static bool IsAvailable(string path)
    {
        try
        {
            var tmp = Directory.GetDirectories(path);
            return true;
        }
        catch
        {
            return File.Exists(path);
        }
    }

    private static void OpenFolder(string path, bool isDriver = false)
    {
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
            GetDriversOptions();
        }
    }

    private static void GetDriversOptions()
    {
        List<Option> options = [];
        options.AddRange(GetDrivers().Select(driver => new Option(driver.Name, () => OpenFolder(driver.Name))));
        Handle(options);
    }

    private static void GenerateOptions(string? path)
    {
        var folders = Directory.GetDirectories(path);
        var options = folders.Select(folder => new Option(folder, () => OpenFolder(folder))).ToList();
        //var files = Directory.GetFiles(path);
        //options.AddRange(files.Select(file => new Option(file, () => SelectPath(file))));
        Handle(options);
    }
}