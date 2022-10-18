using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace OOP_LAB_3.DataSerialization;

public class CustomSerializer : IFormatter
{
    Type _type;
    Type _genericType;
    public CustomSerializer(Type type, Type genericType = null)
    {
        _type = type;
        _genericType = genericType;
    }
    public void Serialize(Stream serializationStream, object graph)
    {
        PropertyInfo[] propertyInfo;
        using (StreamWriter sw = new StreamWriter(serializationStream))
        {
            if (graph.GetType().IsArray)
            {
                sw.WriteLine($"{_type.Name}");
                Array arr = graph as Array;

                propertyInfo = arr.GetType().GetElementType().GetProperties();

                sw.WriteLine("Length:" + arr.Length);

                foreach (var item in arr)
                {
                    foreach (var property in propertyInfo)
                    {
                        sw.WriteLine($"{property.Name}:{property.GetValue(item)}");
                    }
                    sw.WriteLine();
                }
            } // serialize arr
            else if (graph.GetType().IsGenericType && graph is IList)
            {
                sw.WriteLine($"{_type.Name} [{_genericType.Name}]");

                propertyInfo = _genericType.GetProperties();
                IList list = graph as IList;
                sw.WriteLine("Length:" + list.Count);

                foreach (var item in list)
                {
                    foreach (var property in propertyInfo)
                    {
                        sw.WriteLine($"{property.Name}:{property.GetValue(item)}");
                    }
                    sw.WriteLine();
                }
            } // serialize IList<_genericType>
            else
            {
                propertyInfo = _type.GetProperties();
                sw.WriteLine();
                foreach (var property in propertyInfo)
                {
                    sw.WriteLine($"{property.Name}:{property.GetValue(graph)}");
                }
            }   // serialize object

            sw.Flush();
        }
    }
    public object Deserialize(Stream serializationStream)
    {
        dynamic obj;
        if (_type.IsArray)
        {
            using (StreamReader sr = new StreamReader(serializationStream))
            {
                sr.ReadLine();

                string[] length = sr.ReadLine().Split(":");

                Type element = _type.GetElementType();

                obj = Array.CreateInstance(element, int.Parse(length[1]));

                string content = sr.ReadToEnd();

                string[] pairs = content.Split(new String[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                dynamic key, value;
                int index = 0;
                foreach (var pair in pairs)
                {
                    var keyValue = pair.Split(":");
                    key = keyValue[0];
                    value = keyValue[1];


                    PropertyInfo propertyInfo = element.GetProperty(key);

                    if (propertyInfo.GetSetMethod() != null)
                    {
                        dynamic ob = Activator.CreateInstance(element);
                        propertyInfo.SetValue(ob, value, null);
                        obj[index++] = ob;
                    }
                }

            }
        } // serialize arr
        else if (_genericType != null)
        {
            using (StreamReader sr = new StreamReader(serializationStream))
            {
                string s = sr.ReadLine();
                if (string.IsNullOrEmpty(s)) return null;

                string[] length = sr.ReadLine().Split(":"); // Length of list format Length : Value

                var closedType = _type.MakeGenericType(_genericType);

                obj = Activator.CreateInstance(closedType); // Creating List<T> where T _genericType

                //IList<NewMyString> list = obj as IList<NewMyString>;


                string content = sr.ReadToEnd();

                string[] pairs = content.Split(new String[] { "\n", "\r\n" }, StringSplitOptions.None);

                dynamic key, value;
                dynamic listObject = Activator.CreateInstance(_genericType);
                int index = 0;
                foreach (var pair in pairs)
                {
                    if (pair == "")
                    {
                        if (index == int.Parse(length[1])) break;

                        obj.Add(listObject);
                        listObject = Activator.CreateInstance(_genericType);
                        index++;

                        continue;
                    }

                    var keyValue = pair.Split(":");
                    key = keyValue[0];

                    value = keyValue[1];
                    if (value == "True") value = true;
                    else if (value == "False") value = false;

                    PropertyInfo propertyInfo = _genericType.GetProperty(key);

                    if (propertyInfo.GetSetMethod() != null) propertyInfo.SetValue(listObject, value, null);

                }
            }
        } // deserialize IList<_genericType>
        else
        {
            obj = Activator.CreateInstance(_type);
            using (StreamReader sr = new StreamReader(serializationStream))
            {
                sr.ReadLine();

                string content = sr.ReadToEnd();

                string[] pairs = content.Split(new String[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                string key, value;
                foreach (var pair in pairs)
                {
                    var keyValue = pair.Split(":");
                    key = keyValue[0];
                    value = keyValue[1];

                    PropertyInfo propertyInfo = _type.GetProperty(key);
                    if (propertyInfo.GetSetMethod() != null)
                        propertyInfo.SetValue(obj, value, null);
                }
            }
        } // deserialize object
        return obj;
    }

    #region UnUsed

    public SerializationBinder? Binder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public StreamingContext Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public ISurrogateSelector? SurrogateSelector { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    #endregion
}
