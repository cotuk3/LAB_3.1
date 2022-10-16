using System.Collections;

namespace IConsoleMenu;

public interface IConsoleMenu
{
    void Start();
    void Info();
    void Show(IEnumerable collection);

}
