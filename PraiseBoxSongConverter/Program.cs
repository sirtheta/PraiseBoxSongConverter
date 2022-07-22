
using PraiseBoxSongConverter;

class Program
{
  static void Main(string[] args)
  {
    Logic.WriteInConsole($"||||| -- PraisBox Database Converter -- |||||", 5);
    Logic.WriteInConsole($"STARTING................", 70);
    Logic.WriteInConsole($"WELCOME {Environment.UserName}", 5);
    Console.WriteLine();

    // Place praisbox db in root folder
    string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\..\..\..\PraiseBox_Data.pbd";

    // get raw song from DB
    List<RawSong> rawSong = new();
    bool success = Logic.ReadRawTextFromDb(connectionString, ref rawSong);
    List<CleanedSong> cleanedSongList = new();

    int counter = 0;
    foreach (var item in rawSong)
    {
      counter++;
      var title = item.SongTitle.RemoveSpecialCharacters();
      var song = Logic.AddSongTitleAndInfosToText(title, Logic.GenerateVersesAndChoruses(RichTextStripper.StripRichTextFormat(Logic.RemoveSecondLanguage(item.RawSongText))));

      cleanedSongList.Add(new CleanedSong(title, song));
      Console.Write(".");
    }

    if (success && Logic.CreateSong(ref cleanedSongList))
    {
      Logic.WriteInConsole($"Songs successfully converted: {counter}", 1);
    }

    Logic.WriteInConsole("Press any key to exit converter", 1);
    Console.ReadKey();
  }
}