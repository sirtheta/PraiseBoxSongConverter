using System.Text.RegularExpressions;

namespace PraiseBoxSongConverter
{
  internal static class Extension
  {
    public static IEnumerable<string> SplitToLines(this string input)
    {
      if (input == null)
      {
        yield break;
      }

      using (StringReader reader = new StringReader(input))
      {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
          yield return line;
        }
      }
    }

    public static string RemoveSpecialCharacters(this string str)
    {
      return Regex.Replace(str, "[^a-zA-ZäöüÄÖÜ0-9_. ]+", "", RegexOptions.Compiled);
    }
  }
}
