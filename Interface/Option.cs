namespace EU4FileInjector.Interface;

public class Option(string name, Action selected)
{
    public string Name { get; } = name;
    public Action Selected { get; } = selected;
}