
using PraiseBoxSongConverter;
using System.Data.OleDb;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
  static void Main(string[] args)
  {
    WriteInConsole($"||||| -- PraisBox Database Converter -- |||||", 5);
    WriteInConsole($"STARTING................", 70);
    WriteInConsole($"WELCOME {Environment.UserName}", 5);
    Console.WriteLine();

    // Place praisbox db in root folder
    string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\..\..\..\PraiseBox_Data.pbd";

    // get raw song from DB
    List<RawSong> rawSong = new();
    bool success = ReadRawTextFromDb(connectionString, ref rawSong);
    List<CleanedSong> cleanedSongList = new();

    int counter = 0;
    foreach (var item in rawSong)
    {
      counter++;
      var title = item.SongTitle.RemoveSpecialCharacters();
      var song = AddSongTitleAndInfosToText(title, GenerateVersesAndChoruses(RichTextStripper.StripRichTextFormat(RemoveSecondLanguage(item.RawSongText))));

      cleanedSongList.Add(new CleanedSong(title, song));
      Console.Write(".");
    }

    if (success && CreateSong(ref cleanedSongList))
    {
      WriteInConsole($"Songs successfully converted: {counter}", 1);
    }

    WriteInConsole("Press any key to exit converter", 1);
    Console.ReadKey();
  }

  /// <summary>
  /// removes a second language in the song. The second language starts with smaller font size --> \fs20, 
  /// </summary>
  /// <param name="text"></param>
  /// <returns>string</returns>
  static string RemoveSecondLanguage(string text)
  {
    StringBuilder sb = new();
    string cleaned = string.Empty;
    using (StringReader reader = new(text))
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

  /// <summary>
  /// adds song title to the beginning of the song and some additional info to the end
  /// the end is needed that OpenLP can recognize the song
  /// </summary>
  /// <param name="title"></param>
  /// <param name="song"></param>
  /// <returns></returns>
  static string AddSongTitleAndInfosToText(string title, string song)
  {
    StringBuilder sb = new();
    sb.AppendLine(title);
    using (StringReader reader = new StringReader(song))
    {
      string line;
      while ((line = reader.ReadLine()) != null)
      {
        sb.AppendLine(line);
      }
    }
    sb.AppendLine(Environment.NewLine);
    sb.AppendLine("CCLI-Liednummer -");
    sb.AppendLine("© Words:");

    return sb.ToString();
  }

  /// <summary>
  /// for every verse and chorus generate the keyword with a new line after
  /// </summary>
  /// <param name="song"></param>
  /// <returns></returns>
  static string GenerateVersesAndChoruses(string song)
  {
    for (int i = 1; i < 20; i++)
    {
      var regexVers = new Regex(Regex.Escape($"{i}: "));
      var regexChorus = new Regex(Regex.Escape($"R{i}: "));
      song = regexChorus.Replace(song, $"{Environment.NewLine}Chorus {i}{Environment.NewLine}", 1);
      song = regexVers.Replace(song, $"{Environment.NewLine}Vers {i}{Environment.NewLine}", 1);
    }

    return song;
  }

  /// <summary>
  /// creates the actual song in a textfile
  /// </summary>
  /// <param name="song"></param>
  static bool CreateSong(ref List<CleanedSong> song)
  {
    bool success = true;
    // create folder in root directory of the project
    var folder = "..//..//..//..//converted";
    Directory.CreateDirectory(folder);

    foreach (var item in song)
    {
      try
      {
        File.WriteAllText($"{folder}/{item.Title}.txt", item.Song);
      }
      catch (Exception ex)
      {
        Console.WriteLine();
        Console.WriteLine(ex.Message);
        Console.WriteLine("Converting failed!");
        success = false;
        break;
      }
    }
    return success;
  }

  /// <summary>
  /// get raw songText, songNumber, songTitle and copyright from db
  /// </summary>
  /// <param name="connectionString"></param>
  /// <returns>List<RawSong></returns>
  static bool ReadRawTextFromDb(string connectionString, ref List<RawSong> listOfRawSongs)
  {
    bool success = true;
    using (OleDbConnection connection = new OleDbConnection(connectionString))
    {
      string strSQL = "SELECT * FROM SongText";

      OleDbCommand command = new OleDbCommand(strSQL, connection);
      try
      {
        connection.Open();

        using OleDbDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
          listOfRawSongs.Add(new RawSong()
          {
            SongTitle = reader["SongTitle"].ToString().TrimEnd('*').Trim(),
            RawSongText = reader["SongTextFormatted"].ToString(),
          });
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine();
        Console.WriteLine(ex.Message);
        Console.WriteLine("Database not correct! Aborting...");
        success = false;
      }
    }
    return success;
  }

  /// <summary>
  /// jsut for fun... and becaus I can! :)
  /// </summary>
  /// <param name="text"></param>
  /// <param name="ms"></param>
  private static void WriteInConsole(string text, int ms)
  {
    Console.WriteLine();
    for (int i = 0; i < text.Length; i++)
    {
      Console.Write(text[i]);
      Thread.Sleep(ms);
    }
  }
}