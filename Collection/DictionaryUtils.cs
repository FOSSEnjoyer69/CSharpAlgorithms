using System.Collections.Generic;

namespace CSharpAlgorithms.Collection;
public static class DictionaryUtils
{
    public static TValue[] GetValues<Tkey, TValue>(Dictionary<Tkey, TValue> dic)
    {
        TValue[] values = new TValue[dic.Count];
        dic.Values.CopyTo(values, 0); 
        return values;
    }

    public static bool ContainsKey<Tkey, TValue>(this Dictionary<Tkey, TValue> dic, Tkey[] key)
    {
        foreach (Tkey k in key)
        {
            if (!dic.ContainsKey(k))
                return false;
        }
        return true;
    }
}