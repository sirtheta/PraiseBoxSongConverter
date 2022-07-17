using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PraiseBoxSongConverter
{
  /// <summary>
  /// Class to extract the raw song from DB
  /// </summary>
  internal class RawSong
  {
    public string RawSongText { get; set; }
    public string SongTitle { get; set; }
    public string SongCopyright { get; set; }
  }
}
