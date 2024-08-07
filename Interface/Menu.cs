namespace EU4FileInjector.Interface;

public static class Menu
{
    public static void WriteMenu(List<Option> options, Option selectedOption)
    {
        Console.Clear();
        Console.WriteLine($"{Program.AppName}. Version {Program.Version}\n");
        
        foreach (var option in options)
        {
            Console.Write(option == selectedOption ? "> " : " ");
            Console.WriteLine(option.Name);
        }
    }

    public static void Run(List<Option> options)
    {
        if (options.Count <= 0) return;
        
        var index = 0;
        WriteMenu(options, options[index]);
        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey();
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
                case ConsoleKey.Enter:
                    options[index].Selected.Invoke();
                    index = 0;
                    break;
            }
        } while (keyInfo.Key != ConsoleKey.X);
    }
}