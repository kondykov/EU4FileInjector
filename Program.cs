namespace EU4FileInjector;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("EU4 File injector started!");
        Injector injector = new();
        injector.Run();
        Console.WriteLine("EU4 File injector finished!");
        Console.ReadKey();
    }
}