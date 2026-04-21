namespace CSharpAlgorithms;

public static class CSDebug
{
    public static void PrintArray<T>(T[] array)
    {
        string message = "";

        message += "[\n";

        for (int i = 0; i < array.Length; i++)
        {
            string elementMessage = $"{i}: {typeof(T)}, {array[i]}\n";
            message += $"   {elementMessage}";
        }

        message += "]";

        Console.WriteLine(message);
    }

    public static void Print<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        Console.WriteLine("{0,-15} {1,5}", "Key", "Value");
        Console.WriteLine(new string('-', 22));

        foreach (var kvp in dictionary)
        {
            Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
        }
    }

    public static void PrintObject<T>(T obj)
    {
        Type type = typeof(T);
        var fields = type.GetFields();
        var properties = type.GetProperties();


        foreach (var field in fields)
        {
            var value = field.GetValue(obj);
            Console.WriteLine($"{field.Name}: {value}");
        }

        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);
            Console.WriteLine($"{prop.Name}: {value}");
        }
    }

    public static void Print(Exception ex)
    {
        WriteErrorLine($"Exception Type: {ex.GetType()}");
        WriteErrorLine($"StackTrace: {ex.StackTrace}");
        WriteErrorLine($"Source: {ex.Source}");
        WriteErrorLine($"Message: {ex.Message}");
        WriteErrorLine($"TargetSite: {ex.TargetSite}");
        if (ex.InnerException is not null)
        {
            WriteErrorLine("Inner Exception:");
            Print(ex.InnerException);
        }
        WriteErrorLine($"HResult: {ex.HResult}");
        WriteErrorLine($"Data: {ex.Data}");
        WriteErrorLine($"HelpLink: {ex.HelpLink}");
    }

    public static void Print<T>(T[] array)
    {
        string message = "";

        message += "[\n";

        for (int i = 0; i < array.Length; i++)
        {
            string elementMessage = $"{i}: {typeof(T)}, {array[i]}\n";
            message += $"   {elementMessage}";
        }

        message += "]";

        Console.WriteLine(message);
    }

    public static void WriteErrorLine(string message) => WriteLine(message, ConsoleColor.Red);
    public static void WriteWarning(string message) => WriteLine(message, ConsoleColor.Yellow);
    public static void WriteSuccess(string message) => WriteLine(message, ConsoleColor.Green);

    public static void WriteLine(double number, ConsoleColor colour = ConsoleColor.White) => WriteLine(number.ToString(), colour);
    public static void WriteLine(int number, ConsoleColor colour = ConsoleColor.White) => WriteLine(number.ToString(), colour);
    public static void WriteLine(string message, ConsoleColor colour = ConsoleColor.White)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(message);
        Console.ResetColor();
    }

}