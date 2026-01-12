using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpAlgorithms.Computer;
public static class SaveUtils
{
    public static void SaveDict<T1, T2>(string filePath, Dictionary<T1, T2> dict) 
        where T1: notnull
    {
        string text = string.Empty;

        foreach (var item in dict)
            text += $"{item.Key}:{item.Value}\n";


        File.WriteAllText(filePath, text);
    }

    public static Dictionary<T1, T2> LoadDict<T1, T2>(string filePath) 
        where T1: IParsable<T1>
        where T2: IParsable<T2>
    {
        Dictionary<T1, T2> dict = [];

        string[] lines = File.ReadAllLines(filePath);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] linesSplitted = line.Split(":");

            T1 key = T1.Parse(linesSplitted.First(), null);
            T2 value = T2.Parse(linesSplitted.Last(), null);

            dict[key] = value;
        }

        return dict;
    }
}