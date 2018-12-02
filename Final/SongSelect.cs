using GameStateManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final {
    public class SongSelect : MenuScreen {
        struct songInfo {
            public string songPath;
            public string songName;
        }

        songInfo[] songs;

        public SongSelect() : base("Song Select") {
            songs = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Songs").Where(f => f.Substring(f.LastIndexOf('.')).Equals(".mp3")).Select(fullPath => {
                var temp = fullPath.LastIndexOf("\\Songs") + 7;
                var length = fullPath.LastIndexOf(".") - temp;
                return new songInfo {
                    songName = fullPath.Substring(temp, length),
                    songPath = fullPath
                };
                }).ToArray();
            
            foreach (songInfo song in songs) {
                MenuEntry temp = new MenuEntry(song.songName);
                // attach an event here
                MenuEntries.Add(temp);
            }

        }

    }
}

//System.Uri uri = new System.Uri("content/background.mp3", System.UriKind.Relative);
//Song s = Song.FromUri()