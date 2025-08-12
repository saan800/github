namespace Examples.DotnetPackage;

public static class StringExtensions
{
    /// <summary>
    /// For each word uppercase the first character, and ensure rest of the characters are lower case
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return str;

        var words = str.Split(' ');
        var newWords = new List<string>();
        foreach (var word in words)
        {
            newWords.Add(char.ToUpper(word[0]) + word[1..].ToLower());
        }
        return string.Join(" ", newWords);
    }
}
