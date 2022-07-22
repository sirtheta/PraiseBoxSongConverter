namespace PraiseBoxSongConverter
{
  /// <summary>
  /// Class for the cleand song, ready to generate into transfer file
  /// </summary>
  internal class CleanedSong
  {
    public CleanedSong(string title, string song)
    {
      Title = title;
      Song = song;
    }

    public string Title { get; set; }
    public string Song { get; set; }
  }
}
