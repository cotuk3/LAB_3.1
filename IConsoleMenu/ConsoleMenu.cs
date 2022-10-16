using System.Collections;

namespace ConsoleMenu;

public abstract class ConsoleMenu : IConsoleMenu
{
    public abstract void Info();
    public abstract void Show(IEnumerable collection);
    public abstract void Start();
}

