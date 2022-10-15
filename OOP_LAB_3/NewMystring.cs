namespace OOP_LAB_3;

[Serializable]
public class NewMyString : MyString
{
    public NewMyString()
        : base("")
    {

    }
    public NewMyString(string item)
        : base(item)
    {

    }

    public static implicit operator NewMyString(String s)
    {
        NewMyString res = new NewMyString(s);
        return res;
    }
}


