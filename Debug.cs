namespace CSharpAlgorithms;

public static class Debug
{
    public static void LogDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        Console.WriteLine("{0,-15} {1,5}", "Key", "Value");
        Console.WriteLine(new string('-', 22));

        foreach (var kvp in dictionary)
        {
            Console.WriteLine("{0,-15} {1,5}", kvp.Key, kvp.Value);
        }
    }

    public static void PrintArray<T>(T[] array)
    {
        foreach (var item in array)
        {
            Console.WriteLine(item);
        }
    }

    public static void PrintObject<T>(T obj)
    {
        var properties = typeof(T).GetProperties();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);
            Console.WriteLine($"{prop.Name}: {value}");
        }
    }
}