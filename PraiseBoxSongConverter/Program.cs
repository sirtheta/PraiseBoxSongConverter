
using PraiseBoxSongConverter;
using System.Data.OleDb;
using System.Text;

class Program
{
  static void Main(string[] args)
  {
    string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Projects\VS\PraiseBoxSongConverter\PraiseBox_Data.pbd";

    // get raw songText, songNumber, songTitle and copyright from db
    List<RawSong> rawSong = ReadRawTextFromDb(connectionString);
    List<string> cleanedSongText = new();

    foreach (var item in rawSong)
    {
      cleanedSongText.Add(RichTextStripper.StripRichTextFormat(RemoveSecondLanguage(item.RawSongText)));
    }

    Console.ReadKey();
  }

  /// <summary>
  /// removes a second language in the song. The second language starts wits \lang0 and the size, which is normall \fs20
  /// </summary>
  /// <param name="text"></param>
  /// <returns>string</returns>
  static string RemoveSecondLanguage(string text)
  {
    StringBuilder sb = new();
    string cleaned = string.Empty;
    using (StringReader reader = new StringReader(text))
    {
      string line;
      while ((line = reader.ReadLine()) != null)
      {
        int startIndex = line.IndexOf("\\fs20");
        int endIndex = -1;
        string removedLine = line;
        if (startIndex != -1)
        {
          endIndex = line.IndexOf("\\par", startIndex);
          if (endIndex != -1)
          {
            removedLine = line.Remove(startIndex, endIndex - startIndex);
          }
        }
        if (removedLine != "\\par")
        {
          cleaned = sb.AppendLine(removedLine).ToString();
        }
      }
    }

    return cleaned;
  }


  static void CreateSong(string songTitle, string songText)
  {
    CleanedSong newSong = null;


  }

  /// <summary>
  /// get raw songText, songNumber, songTitle and copyright from db
  /// </summary>
  /// <param name="connectionString"></param>
  /// <returns>List<RawSong></returns>
  static List<RawSong> ReadRawTextFromDb(string connectionString)
  {
    List<RawSong> rawSong = new();

    using (OleDbConnection connection = new OleDbConnection(connectionString))
    {
      string strSQL = "SELECT * FROM SongText";

      OleDbCommand command = new OleDbCommand(strSQL, connection);
      try
      {
        connection.Open();

        using (OleDbDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            rawSong.Add(new RawSong()
            {
              SongTitle = reader["SongTitle"].ToString().TrimEnd('*'),
              RawSongText = reader["SongTextFormatted"].ToString(),
              SongCopyright = reader["SongCopyrightPure"].ToString()
            });
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
    return rawSong;
  }
}