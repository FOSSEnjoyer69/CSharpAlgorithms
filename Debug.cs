using System;
using System.Collections.Generic;
using CSharpAlgorithms.Audio;

namespace CSharpAlgorithms;

public static class Debug
{
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

    public static void WriteLine(int number, ConsoleColor colour = ConsoleColor.White) => WriteLine(number.ToString(), colour);

    public static void WriteErrorLine(string message) => WriteLine(message, ConsoleColor.Red);
    public static void WriteLine(string message, ConsoleColor colour = ConsoleColor.White)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteWarning(string message) => WriteLine(message, ConsoleColor.Yellow);
}