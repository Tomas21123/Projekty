using AudioPrehravac.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPrehravac.ViewModel
{
    public class NactiPlaylistyViewModel
    {
        public ObservableCollection<Playlist> Playlists { get; set; } = new ObservableCollection<Playlist>();

        public void NactiPlaylisty(string folderPath)
        {
            // složka pro playlisty
            string playlistFolder = Path.Combine(folderPath, "playlisty");

            // pokud složka neexistuje, vytvoř ji
            if (!Directory.Exists(playlistFolder))
            {
                Directory.CreateDirectory(playlistFolder);
            }

            // vyčisti staré playlisty
            Playlists.Clear();

            // projdi všechny .txt soubory
            var playlistFiles = Directory.GetFiles(playlistFolder, "*.txt");
            foreach (var file in playlistFiles)
            {
                try
                {
                    // název playlistu je název souboru bez přípony
                    string playlistName = Path.GetFileNameWithoutExtension(file);
                    var playlist = new Playlist { Name = playlistName };

                    // načti všechny řádky (písničky)
                    var lines = File.ReadAllLines(file);

                    foreach (var line in lines)
                    {
                        string songPath = Path.Combine(folderPath, line.Trim());
                        if (File.Exists(songPath))
                        {
                            playlist.Songs.Add(new Song { FileName = line.Trim() });
                        }
                    }

                    Playlists.Add(playlist);
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
