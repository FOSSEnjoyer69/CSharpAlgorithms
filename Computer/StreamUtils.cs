namespace CSharpAlgorithms.Computer;

public static class StreamUtils
{
    public static async Task<int> ReadFullOrEndAsync(Stream stream, byte[] buffer, CancellationToken cancellationToken)
    {
        int totalRead = 0;

        while (totalRead < buffer.Length)
        {
            int n = await stream.ReadAsync(buffer.AsMemory(totalRead, buffer.Length - totalRead), cancellationToken);

            if (n == 0)
                break;

            totalRead += n;
        }

        return totalRead;
    }
    public static int ReadFullOrEnd(Stream stream, byte[] buffer)
    {
        int totalRead = 0;

        while (totalRead < buffer.Length)
        {
            int n = stream.Read(buffer, totalRead, buffer.Length - totalRead);

            if (n == 0)
                break;

            totalRead += n;
        }

        return totalRead;
    }
}