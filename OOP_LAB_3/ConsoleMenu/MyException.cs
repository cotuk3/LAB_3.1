namespace OOP_LAB_3.MyConsoleMenu;
class MyException : Exception
{
    public MyException()
        : base()
    {

    }

    public MyException(string commandName)
        : base(string.Format($"Command with name {commandName} doesn't exist!"))
    {

    }
}
