using OOP_LAB_3.DataSerialization;
using System.Collections;

namespace OOP_LAB_3.MyConsoleMenu;

class MyConsoleMenu : ConsoleMenu.ConsoleMenu
{
    DataSerializer current = new DataSerializer(typeof(NewMyString[]));

    static Dictionary<string, Action> commands = new Dictionary<string, Action>();
    static Dictionary<string, Func<DataSerializer, string, object>> deserialize = new Dictionary<string, Func<DataSerializer, string, object>>();

    static Func<string> askFilePath = () => { Console.Write("Enter file path:"); string s = Console.ReadLine(); return s; };

    NewMyString[][] autoFillArr = 
        { 
        new NewMyString[]{ "Hi", "my", "name", "Bohdan" },
        new NewMyString[]{ "Hallo", "ich", "heiße", "Bohdan"},
        new NewMyString[]{ "Привіт", "мена", "звати", "Богдан"},
    };
    public MyConsoleMenu()
    {
        commands.Add("/info", () => Info());
        commands.Add("/ser", () => StartSerialize(askFilePath()));
        commands.Add("/deser", () =>
        {
            string filePath = askFilePath();
            if (deserialize.ContainsKey(Path.GetExtension(filePath)))
                Show(deserialize[Path.GetExtension(filePath)](current, filePath) as IEnumerable);
            else
                throw new FileNotFoundException("Wrong File Name!");
        });
        commands.Add("/end", () => { Console.WriteLine("Bye, have a good time!"); Console.Read(); });
        commands.Add("/cls", () => { Console.Clear(); Info(); });

        deserialize.Add(".dat", (ds, filePath) => ds.BinaryDeserialize(filePath));
        deserialize.Add(".xml", (ds, filePath) => ds.XMLDeserialize(filePath));
        deserialize.Add(".json", (ds, filePath) => ds.JSONDeserialize(filePath));
        deserialize.Add(".txt", (ds, filePath) => ds.CustomDeserialize(filePath));
    }

    public override void Start()
    {
        string input = "";
        commands["/info"]();
        do
        {
            Console.Write("Enter a command: ");
            input = Console.ReadLine();
            int number;


            if (int.TryParse(input, out number))
            {
                switch (number)
                {
                    case 1:
                        input = "/info";
                        break;
                    case 2:
                        input = "/ser";
                        break;
                    case 3:
                        input = "/deser";
                        break;
                    case 4:
                        input = "/cls";
                        break;
                    case 5:
                        input = "/end";
                        break;
                }
            }

            try
            {
                if (commands.ContainsKey(input))
                    commands[input]();
                else
                    throw new MyException(input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        } while (input != "/end");
    }
    public override void Info()
    {
        string info = "\n***** Serialization *****\n" +
            "1./info\n" +
            "2./ser\n" +
            "3./deser\n" +
            "4./cls\n" +
            "5./end\n";
        Console.WriteLine(info);
    }
    public override void Show(IEnumerable collection)
    {
        foreach (var item in collection)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();
    }

    public void StartSerialize(string filePath)
    {
        int nameOfCollection = 0;
        do
        {
            Console.Write("Enter what you want to serialize:\n" +
                "1.Array;\n" +
                "2.List.\n:");
            nameOfCollection = int.Parse(Console.ReadLine());
        } while (nameOfCollection != 1 && nameOfCollection != 2);

        int howToFill = 0;
        do
        {
            Console.Write("Enter how you want to fill your collection:" +
                "\n1.Auto;" +
                "\n2.Manual." +
                "\n:");
            howToFill = int.Parse(Console.ReadLine());

        } while (howToFill != 1 && howToFill != 2);

        object graph = nameOfCollection == 1 ? FillArr(howToFill) : FillList(howToFill); //Path.GetExtension(filePath) == ".txt" ? FillArr(howToFill) :
        Serialize(Path.GetExtension(filePath), graph, filePath);
    }
    public void Serialize(string type, object graph, string filePath)
    {
        if (graph.GetType().IsGenericType && type == ".txt")
            current = new DataSerializer(typeof(List<>), typeof(NewMyString));
        else
            current = new DataSerializer(graph.GetType());


        switch (type)
        {
            case ".dat":
                current.BinarySerialize(graph, filePath);
                break;
            case ".xml":
                current.XMLSerialize(graph, filePath);
                break;
            case ".json":
                current.JSONSerialize(graph, filePath);
                break;
            case ".txt":
                current.CustomSerialize(graph, filePath);
                break;
        }
        Console.Write("Chose:" +
            "\n1.Deserialize;" +
            "\n2.Exit." +
            "\n:");
        if (int.Parse(Console.ReadLine()) == 1)
            Show(deserialize[type](current, filePath) as IEnumerable);

    }

    #region Auxiliary Methods
    public NewMyString[] FillArr(int fill)
    {
        if (fill == 1)
            return autoFillArr[new Random().Next(0,2)];

        else
        {
            string input = "";
            int index = 0;
            NewMyString[] arr = new NewMyString[1];
            Console.WriteLine("Enter your string:");
            do
            {

                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) break;
                if (index == arr.Length) Array.Resize(ref arr, arr.Length + 1);

                arr[index++] = input;

            } while (true);
            return arr;
        }
    }
    public List<NewMyString> FillList(int fill)
    {
        if (fill == 1)
            return new List<NewMyString>(autoFillArr[new Random().Next(0, 2)]);
        else
        {
            string input = "";
            int index = 0;
            List<NewMyString> list = new List<NewMyString>();
            Console.WriteLine("Enter your string:");
            do
            {
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    break;
                list.Add(input);

            } while (true);
            return list;
        }
    }
    #endregion
}
