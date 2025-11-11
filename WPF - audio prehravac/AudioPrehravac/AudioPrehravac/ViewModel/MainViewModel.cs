using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AudioPrehravac.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public NacteniSonguViewModel NacteniSongu { get; set; } = new NacteniSonguViewModel();
        public NactiPlaylistyViewModel NactiPlaylisty { get; set; } = new NactiPlaylistyViewModel();

        private bool _isPlaylist;
        public bool IsPlaylist
        {
            get => _isPlaylist;
            set
            {
                _isPlaylist = value;
                OnPropertyChanged();
            }
        }

        private string _aktualniPisnicka;
        public string AktualniPisnicka
        {
            get => _aktualniPisnicka;
            set { _aktualniPisnicka = value; OnPropertyChanged(); }
        }

        private string _aktualniPlaylist;
        public string AktualniPlaylist  // FALSE = všechny pisnicky, TRUE = jedna dokola
        {
            get => _aktualniPlaylist;
            set { _aktualniPlaylist = value; OnPropertyChanged(); }
        }

        private bool _rezimPrehravani;
        public bool RezimPrehravani
        {
            get => _rezimPrehravani;
            set { _rezimPrehravani = value; OnPropertyChanged(); }
        }

        private bool _nahodnePrehravani;
        public bool NahodnePrehravani
        {
            get => _nahodnePrehravani;
            set { _nahodnePrehravani = value; OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
