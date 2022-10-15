using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
namespace OOP_LAB_3.DataSerialization;

class DataSerializer
{
    Type _type;
    Type _genericType;

    string extension;
    Regex path;
    ArgumentException wrongExtension = new ArgumentException("File has wrong extension");

    public DataSerializer(Type type, Type genericType = null)
    {
        _type = type;
        _genericType = genericType;
    }
    public void BinarySerialize(object graph, string filePath)
    {
        extension = "dat";
        path = new Regex($"\\w.{extension}");
        if (path.IsMatch(filePath))
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            using (FileStream fileStream = File.Create(filePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fileStream, graph);
            }
        }
        else
            throw wrongExtension;
    }
    public object BinaryDeserialize(string filePath)
    {

        extension = "dat";
        path = new Regex($"\\w.{extension}");
        if (path.IsMatch(filePath) && File.Exists(filePath))
        {
            object obj;
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                obj = bf.Deserialize(fileStream);
            }
            return obj;
        }
        else
            throw wrongExtension;


    }

    public void XMLSerialize(object graph, string filePath)
    {
        extension = "xml";
        path = new Regex($"\\w.{extension}");
        if (path.IsMatch(filePath))
        {
            if (File.Exists(filePath)) File.Delete(filePath);

            using (FileStream fileStream = File.Create(filePath))
            {

                XmlSerializer xs = new XmlSerializer(_type);
                xs.Serialize(fileStream, graph);

            }
        }
        else
            throw wrongExtension;
    }
    public object XMLDeserialize(string filePath)
    {
        extension = "xml";
        path = new Regex($"\\w.{extension}");
        if (path.IsMatch(filePath) && File.Exists(filePath))
        {
            object obj;
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                XmlSerializer xs = new XmlSerializer(_type);
                obj = xs.Deserialize(fileStream);

            }
            return obj;
        }
        else
            throw wrongExtension;
    }

    public void JSONSerialize(object graph, string filePath)
    {
        extension = "json";
        path = new Regex($"\\w.{extension}");
        if (path.IsMatch(filePath))
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            using (FileStream fileStream = File.Create(filePath))
            {
                JsonSerializer.Serialize(fileStream, graph, _type);
            }
        }
        else
            throw wrongExtension;

    }
    public object JSONDeserialize(string filePath)
    {
        extension = "json";
        path = new Regex($"\\w.{extension}");
        if (path.IsMatch(filePath) && File.Exists(filePath))
        {
            object obj;
            using (var fileStream = File.OpenRead(filePath))
            {
                obj = JsonSerializer.Deserialize(fileStream, _type);
                return obj;
            }
        }
        else
            throw wrongExtension;
    }

    public void CustomSerialize(object graph, string filePath)
    {
        extension = "txt";
        path = new Regex($"\\w.{extension}");
        if (path.IsMatch(filePath))
        {
            FileStream fileStream;
            if (File.Exists(filePath)) File.Delete(filePath);

            fileStream = File.Create(filePath);

            CustomSerializer cs = new CustomSerializer(_type, _genericType);
            cs.Serialize(fileStream, graph);

            fileStream.Close();
        }
        else
        {
            throw new ArgumentException("File has wrong extension");
        }
    }
    public object CustomDeserialize(string filePath)
    {
        extension = "txt";
        path = new Regex($"\\w.{extension}");
        if (path.IsMatch(filePath))
        {
            FileStream fileStream;

            fileStream = File.OpenRead(filePath);
            CustomSerializer cs = new CustomSerializer(_type, _genericType);
            object obj = cs.Deserialize(fileStream);

            fileStream.Close();
            return obj;
        }
        else
        {
            throw new ArgumentException("File has wrong extension");
        }
    }
}


