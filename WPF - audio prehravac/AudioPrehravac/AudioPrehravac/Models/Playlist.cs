using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPrehravac.Models
{
    public class Playlist
    {
        public string Name { get; set; }
        public List<Song> Songs { get; set; } = new List<Song>();
    }
}
