using System;
using System.Collections.Generic;

namespace CSharpAlgorithms;

public static class Debug
{
    public static void Log<T>(T[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            WriteLine($"Index: {i}, Value: {values[i]}");
        }
    }
    public static void Log<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
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
        var properties = typeof(T).GetProperties();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);
            Console.WriteLine($"{prop.Name}: {value}");
        }
    }

    public static void Print(Exception ex)
    {
        Console.WriteLine($"Exception Type: {ex.GetType()}");
        Console.WriteLine($"Message: {ex.Message}");
        Console.WriteLine($"Source: {ex.Source}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
        Console.WriteLine($"TargetSite: {ex.TargetSite}");
        if (ex.InnerException != null)
        {
            Console.WriteLine("Inner Exception:");
            Print(ex.InnerException);
        }
    }

    public static void WriteErrorLine(string message) => WriteLine(message, ConsoleColor.Red);
    public static void WriteLine(string message, ConsoleColor colour = ConsoleColor.White)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}