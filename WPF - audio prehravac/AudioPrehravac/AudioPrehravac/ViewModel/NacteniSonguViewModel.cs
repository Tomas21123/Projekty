using AudioPrehravac.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AudioPrehravac.ViewModel
{
    public class NacteniSonguViewModel
    {
        public ObservableCollection<Song> Songs { get; set; } = new ObservableCollection<Song>();

        public void LoadSongs(string folderPath)
        {
            Songs.Clear();
            string[] files = Directory.GetFiles(folderPath, "*.mp3");
            int id = 1;
            foreach (var file in files)
            {
                Songs.Add(new Song
                {
                    Id = id,
                    FileName = Path.GetFileName(file)
                });
                id++;
            }
        }
    }
}
