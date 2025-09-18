using System;

namespace CSharpAlgorithms.UUID;

public static class UUIDUtils
{
    public static string CreateUUID()
    {
        return Guid.NewGuid().ToString();
    }
}