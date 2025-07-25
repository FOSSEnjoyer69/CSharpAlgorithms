namespace CSharpAlgorithms;

public static class Calculator
{
    public static int NextPowerOf2(int x)
    {
    if (x < 1) return 1;
        x--;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        return x + 1;
    }
}

