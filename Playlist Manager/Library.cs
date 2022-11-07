using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playlist_Manager
{
    public class Library
    {
        public List<Playlist> Playlists;
    }
    
    public class Playlist
    {
        public string Name;
        public List<Track> Tracks;
    }

    public class Track
    {
        public int Id;
        public string Artist;
        public string Title;

    }
}
