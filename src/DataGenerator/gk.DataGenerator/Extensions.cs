using gk.DataGenerator.Generators;

internal static class Extensions
{
    public static bool IsEscaped(string template, int ndx)
    {
        int slashes = 0;
        var c = ndx-1;
        while (c >= 0)
        {
            if (template[c] != '\\') break;
            slashes++;
            c--;
        }
        return (slashes != 0) && slashes%2 != 0;
    }

    public static string AppendRepeated(this string root, string value, int scale)
    {
        var t = "";
        for (int i = 0; i < scale; i++)
        {
            t += value;
        }
        return root+t;
    }
}