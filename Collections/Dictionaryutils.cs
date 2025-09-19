using System.Collections.Generic;
using System.Linq;

namespace CSharpAlgorithms.Audio;

public static class DictionaryUtils
{
    public static TKey GetLastKey<TKey, TValue>(Dictionary<TKey, TValue> dictionary) => dictionary.Last().Key;
}