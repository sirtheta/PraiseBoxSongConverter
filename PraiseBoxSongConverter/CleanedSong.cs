using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PraiseBoxSongConverter
{
  /// <summary>
  /// Class for the cleand song, ready to generate into transfer file
  /// </summary>
  internal class CleanedSong
  {
    public CleanedSong(string title, List<string> verse, List<string> refrain, string author = "unbekannt", string songCopyright = "kein")
    {
      Title = title;
      Verse = verse;
      Refrain = refrain;
      SongCopyright = songCopyright;
      Author = author;
    }

    public string Title { get; set; }
    public List<string> Verse { get; set; }
    public List<string> Refrain { get; set; }
    public string SongCopyright { get; set; }
    public string Author { get; set; }
  }
}
