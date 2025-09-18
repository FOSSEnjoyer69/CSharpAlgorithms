using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using CSharpAlgorithms.Interfaces;

namespace CSharpAlgorithms.Collections;

public class BuffAndDump<T>
{
    private List<T> buffer;
    private IDataDump<T> dump;

    public BuffAndDump(int capacity, IDataDump<T> dumpSite)
    {
        buffer = new List<T>(capacity);
        dump = dumpSite;
    }

    public void Add(T[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (buffer.Count < buffer.Capacity)
                buffer.Add(items[i]);
            else
            {
                dump.Dump(CollectionsMarshal.AsSpan(buffer));
                buffer.Clear();
            }
        }
    }
}