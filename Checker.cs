using System.Reflection;

namespace CSharpAlgorithms;

public static class Checker
{
    public static Type[] BlittableTypes =
    [
        typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
        typeof(int), typeof(uint), typeof(long), typeof(ulong),
        typeof(float), typeof(double), typeof(bool), typeof(char),
        typeof(IntPtr), typeof(UIntPtr)
    ];

    public static bool CheckBlittable<T>(bool printBlittable = true) => CheckBlittable(typeof(T), printBlittable);
    public static bool CheckBlittable(Type type, bool printBlittable = true)
    {
        if (BlittableTypes.Contains(type))
        {
            if (printBlittable)
                Console.WriteLine($"{type.Name} is a blittable type.");
            return true;
        }

        if (!type.IsValueType)
        {
            if (printBlittable)
                Console.WriteLine($"{type.Name} is not a value type, hence not blittable.");
            return false;
        }

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            bool isBlittable = CheckBlittable(field.FieldType);
            if (!isBlittable)
            {
                Console.WriteLine($"{type.Name} contains non-blittable field: {field.Name} of type {field.FieldType.Name}");
                return false;
            }
        }

        return true;
    }

}