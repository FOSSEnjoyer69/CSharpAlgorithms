using System.Collections.Concurrent;

namespace CSharpAlgorithms;

public static class Collections
{
    public static void Clear<T>(BlockingCollection<T> blockingCollection)
    {
        while (blockingCollection.TryTake(out _)) { }
    }
}