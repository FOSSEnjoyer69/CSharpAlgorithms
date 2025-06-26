namespace CSharpAlgorithms;

public static class Debug
{
    public static void LogDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        foreach (var kvp in dictionary)
        {
            Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
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