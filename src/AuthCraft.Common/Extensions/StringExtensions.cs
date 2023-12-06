namespace AuthCraft.Common.Extensions;

public static class StringExtensions
{
    public static string Limit(this string input, int maxSize)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return input.Length <= maxSize
            ? input
            : input.Substring(0, maxSize) + " ***** LIMITED TO " + maxSize + " BYTES *****";
    }
}
