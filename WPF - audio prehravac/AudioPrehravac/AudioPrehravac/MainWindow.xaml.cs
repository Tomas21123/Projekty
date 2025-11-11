using AudioPrehravac.Models;
using AudioPrehravac.ViewModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace AudioPrehravac
{
    public partial class MainWindow : Window
    {
        private AudioPlayer player;
        private DispatcherTimer songTimer;

        ViewModel.MainViewModel mainVM = new ViewModel.MainViewModel();

        private string MusicFilePath = @"C:\Users\tomas\Music";

        private float currentAmplitude = 0;
        private DispatcherTimer visualizerTimer;



        public MainWindow()
        {
            InitializeComponent();
            DataContext = mainVM;

            mainVM.NacteniSongu.LoadSongs(MusicFilePath);
            mainVM.NactiPlaylisty.NactiPlaylisty(MusicFilePath);

            mainVM.IsPlaylist = false;
            mainVM.RezimPrehravani = false; // FALSE = všechny pisnicky, TRUE = jedna dokola

            player = new AudioPlayer(MusicFilePath);
            player.OnAmplitudeChanged += Player_OnAmplitudeChanged;
            player.SetVolume((int)Slider_Hlasitost.Value);
            player.SongFinished += Player_SongFinished;

            songTimer = new DispatcherTimer();
            songTimer.Interval = TimeSpan.FromMilliseconds(500);
            songTimer.Tick += SongTimer_Tick;
            songTimer.Start();

            visualizerTimer = new DispatcherTimer();
            visualizerTimer.Interval = TimeSpan.FromMilliseconds(30);
            visualizerTimer.Tick += VisualizerTimer_Tick;
            visualizerTimer.Start();
        }





        private void Player_SongFinished()
        {
            Dispatcher.Invoke(() =>
            {
                if (mainVM.RezimPrehravani && !string.IsNullOrEmpty(mainVM.AktualniPisnicka))
                {
                    // Opakujeme stejnou písničku
                    player.Seek(0);
                    player.Play();
                    //MessageBox.Show("Opakování: přehrávám stejnou písničku");
                }
                else
                {
                    if(mainVM.NahodnePrehravani)
                    {
                        PlayNahodnySong();
                    }
                    else
                    {
                        PlayNextSong();
                    }
                }
            });
        }

        private void PlayNahodnySong()
        {
            var songs = mainVM.IsPlaylist
                ? mainVM.NactiPlaylisty.Playlists
                    .FirstOrDefault(p => p.Name == mainVM.AktualniPlaylist)?.Songs.ToList()
                : mainVM.NacteniSongu.Songs.ToList();

            if (songs == null || songs.Count == 0)
                return;

            // Vyloučí aktuální písničku, pokud existuje více než jedna
            var dostupnePisnicky = songs.Count > 1
                ? songs.Where(s => s.FileName != mainVM.AktualniPisnicka).ToList()
                : songs;

            // Vyber náhodnou
            var rnd = new Random();
            var nextSong = dostupnePisnicky[rnd.Next(dostupnePisnicky.Count)];

            // Nastavení a přehrání
            player.SetSong(nextSong.FileName);
            player.Play();

            // Aktualizace informací v UI
            mainVM.AktualniPisnicka = nextSong.FileName;
            Slider_SongTime.Minimum = 0;
            Slider_SongTime.Maximum = (int)player.GetSongLengthSeconds();
            Slider_SongTime.Value = 0;

            // Zvýraznění ve výpisu
            SongsDataGrid.SelectedItem = nextSong;
            SongsDataGrid.ScrollIntoView(nextSong);
        }


        private void PlayNextSong()
        {
            // zjisti, jestli aktuálně hraje playlist nebo všechny písničky
            var songs = mainVM.IsPlaylist
                        ? mainVM.NactiPlaylisty.Playlists
                            .FirstOrDefault(p => p.Name == mainVM.AktualniPlaylist)?.Songs.ToList()
                        : mainVM.NacteniSongu.Songs.ToList();


            if (songs == null || songs.Count == 0)
                return;

            // najdi index aktuální písničky
            int currentIndex = songs.FindIndex(s => s.FileName == mainVM.AktualniPisnicka);

            // pokud jsme na konci, začni znovu od první (nebo skonči, pokud chceš)
            int nextIndex = currentIndex + 1;
            if (nextIndex >= songs.Count)
                nextIndex = 0; // nebo return; pokud nechceš přehrávat od začátku

            var nextSong = songs[nextIndex];

            // nastav novou písničku
            player.SetSong(nextSong.FileName);
            player.Play();
            mainVM.AktualniPisnicka = nextSong.FileName;

            // aktualizuj slider
            Slider_SongTime.Minimum = 0;
            Slider_SongTime.Maximum = (int)player.GetSongLengthSeconds();
            Slider_SongTime.Value = 0;

            // zvýrazni v DataGridu právě hrající song
            SongsDataGrid.SelectedItem = nextSong;
            SongsDataGrid.ScrollIntoView(nextSong);
        }

        // Funkce pro slider
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player == null) return;
            player.SetVolume((int)e.NewValue);
        }

        // Přehrávač - puštění / pauza
        private void Click_PlayStop(object sender, RoutedEventArgs e)
        {
            player.PlayStop();
        }

        private void Click_ZobrazitVsechnyPisnicky(object sender, RoutedEventArgs e)
        {
            mainVM.IsPlaylist = false;
            SongsDataGrid.ItemsSource = mainVM.NacteniSongu.Songs;
        }

        private void SongOptions_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void SongTimer_Tick(object sender, EventArgs e)
        {
            if (player.CurrentPositionSeconds() > 0 && Slider_SongTime != null)
            {
                Slider_SongTime.Value = player.CurrentPositionSeconds();
            }
        }

        private void Click_NastavSongu(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Song song)
            {
                player.SetSong(song.FileName);
                player.PlayStop();

                mainVM.AktualniPisnicka = song.FileName;

                Slider_SongTime.Minimum = 0;
                Slider_SongTime.Maximum = (int)player.GetSongLengthSeconds();
                Slider_SongTime.Value = 0;
            }
        }

        private void Slider_SongTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player != null && Slider_SongTime.IsMouseCaptureWithin)
            {
                player.Seek((int)Slider_SongTime.Value);
            }
        }

        private void Player_OnAmplitudeChanged(float amplitude)
        {
            currentAmplitude = amplitude;
        }

        private void VisualizerTimer_Tick(object sender, EventArgs e)
        {
            DrawVisualizer(currentAmplitude);
        }

        private void DrawVisualizer(float amplitude)
        {
            if (VisualizerCanvas == null || VisualizerCanvas.ActualHeight == 0 || VisualizerCanvas.ActualWidth == 0)
                return;

            VisualizerCanvas.Children.Clear();

            double width = VisualizerCanvas.ActualWidth;
            double height = VisualizerCanvas.ActualHeight;
            int barCount = 60; // počet pruhů
            double barWidth = width / barCount;
            Random rnd = new Random();

            double amplitudeScale = 0.5; // váška amplitudy

            for (int i = 0; i < barCount; i++)
            {
                double amp = amplitude * (0.9 + 0.2 * rnd.NextDouble());
                double barHeight = amp * height * amplitudeScale;

                barHeight = Math.Clamp(barHeight, 0, height);

                double x = i * barWidth;
                double y = (height - barHeight) / 2;

                y = Math.Clamp(y, 0, height - barHeight);

                var rect = new System.Windows.Shapes.Rectangle
                {
                    Width = barWidth - 2,
                    Height = barHeight,
                    Fill = Brushes.LimeGreen,
                    RadiusX = 2,
                    RadiusY = 2
                };

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                VisualizerCanvas.Children.Add(rect);
            }
        }

        private void Click_Playlist_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Playlist playlist)
            {
                SongsDataGrid.ItemsSource = playlist.Songs;
            }
        }

        private void Click_VytvořitPlaylist(object sender, RoutedEventArgs e)
        {
            InputDialog dialog = new InputDialog();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                string playlistName = dialog.InputText;
                //MessageBox.Show($"Nový playlist: {playlistName}");

                string folderPath = MusicFilePath + @"\playlisty";
                string fileName = playlistName + ".txt";
                string fullPath = System.IO.Path.Combine(folderPath, fileName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                if (!File.Exists(fullPath))
                {
                    File.WriteAllText(fullPath, string.Empty);
                }

                mainVM.NactiPlaylisty.NactiPlaylisty(MusicFilePath);
            }
        }

        private void Click_SetAktualPlaylist(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Playlist playlist)
            {
                // Nastavení aktuálního playlistu
                mainVM.AktualniPlaylist = playlist.Name;
                mainVM.IsPlaylist = true;

                // Načtení písniček do DataGridu vpravo
                SongsDataGrid.ItemsSource = playlist.Songs;
            }
        }

        private void Click_DeletePlaylist(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is Playlist playlist)
            {
                string folderPath = System.IO.Path.Combine(MusicFilePath, "playlisty");
                string fileName = playlist.Name + ".txt";
                string fullPath = System.IO.Path.Combine(folderPath, fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                mainVM.NactiPlaylisty.NactiPlaylisty(MusicFilePath);
            }
        }

        private void Click_PridatDoPlaylistuHelp(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Song selectedSong)
            {
                var contextMenu = new ContextMenu();

                // Přidat do playlistu
                var addToPlaylistItem = new MenuItem { Header = "Přidat do playlistu" };
                foreach (var playlist in mainVM.NactiPlaylisty.Playlists)
                {
                    var mi = new MenuItem
                    {
                        Header = playlist.Name,
                        DataContext = playlist,
                        Tag = selectedSong
                    };
                    mi.Click += Click_AddSongToSpecificPlaylist;
                    addToPlaylistItem.Items.Add(mi);
                }
                contextMenu.Items.Add(addToPlaylistItem);

                // Odstranit z aktuálního playlistu - jen pokud jsme v playlist módu
                if (mainVM.IsPlaylist && !string.IsNullOrEmpty(mainVM.AktualniPlaylist))
                {
                    var removeItem = new MenuItem { Header = "Odstranit z playlistu", Tag = selectedSong };
                    removeItem.Click += Click_RemoveSongFromCurrentPlaylist;
                    contextMenu.Items.Add(removeItem);
                }

                btn.ContextMenu = contextMenu;
                contextMenu.IsOpen = true;
            }
        }

        // Odebrání písně z aktuálního playlistu
        private void Click_RemoveSongFromCurrentPlaylist(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is Song selectedSong)
            {
                string playlistFile = System.IO.Path.Combine(MusicFilePath, "playlisty", mainVM.AktualniPlaylist + ".txt");

                if (File.Exists(playlistFile))
                {
                    // Odstraníme song ze souboru
                    var lines = File.ReadAllLines(playlistFile).ToList();
                    lines.RemoveAll(l => l.Equals(selectedSong.FileName, StringComparison.OrdinalIgnoreCase));
                    File.WriteAllLines(playlistFile, lines);

                    // Najdi Playlist objekt podle názvu
                    var playlist = mainVM.NactiPlaylisty.Playlists.FirstOrDefault(p => p.Name == mainVM.AktualniPlaylist);
                    if (playlist != null)
                    {
                        // Aktualizace seznamu Songs v objektu Playlist
                        playlist.Songs = lines.Select(name => new Song { FileName = name }).ToList();

                        // Přiřadíme ItemsSource přímo k aktualizovanému seznamu
                        SongsDataGrid.ItemsSource = playlist.Songs;

                        // Pokud chceš, aby se aktualizoval aktuální song:
                        if (playlist.Songs.Any())
                        {
                            var firstSong = playlist.Songs[0];
                            player.SetSong(firstSong.FileName);
                            mainVM.AktualniPisnicka = firstSong.FileName;

                            Slider_SongTime.Minimum = 0;
                            Slider_SongTime.Maximum = (int)player.GetSongLengthSeconds();
                            Slider_SongTime.Value = 0;
                        }
                        else
                        {
                            // Playlist je teď prázdný
                            mainVM.AktualniPisnicka = string.Empty;
                            SongsDataGrid.ItemsSource = null;
                        }
                    }
                }
            }
        }



        private void Click_AddSongToSpecificPlaylist(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.DataContext is Playlist playlist && menuItem.Tag is Song selectedSong)
                {
                    string folderPath = System.IO.Path.Combine(MusicFilePath, "playlisty");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string playlistFile = System.IO.Path.Combine(folderPath, $"{playlist.Name}.txt");

                    // Přidáme jen název souboru
                    File.AppendAllText(playlistFile, selectedSong.FileName + Environment.NewLine);

                    //MessageBox.Show($"Song '{selectedSong.FileName}' byl přidán do playlistu '{playlist.Name}'.");
                    mainVM.NactiPlaylisty.NactiPlaylisty(MusicFilePath);
                }
            }
        }

        // Vrátí index aktuálně vybrané písničky
        private int GetSelectedSongIndex()
        {
            if (SongsDataGrid.SelectedItem is Song selectedSong)
            {
                return mainVM.NacteniSongu.Songs.IndexOf(selectedSong);
            }
            return -1;
        }

        // Rewind – předchozí píseň
        private void Click_Rewind(object sender, RoutedEventArgs e)
        {
            int index = GetSelectedSongIndex();
            if (index > 0)
            {
                var prevSong = mainVM.NacteniSongu.Songs[index - 1];
                SongsDataGrid.SelectedItem = prevSong;
                SongsDataGrid.ScrollIntoView(prevSong);
                player.SetSong(prevSong.FileName);
                mainVM.AktualniPisnicka = prevSong.FileName;

                Slider_SongTime.Minimum = 0;
                Slider_SongTime.Maximum = (int)player.GetSongLengthSeconds();
                Slider_SongTime.Value = 0;

                player.PlayStop();
            }
        }

        // Forward – další píseň
        private void Click_Forward(object sender, RoutedEventArgs e)
        {
            int index = GetSelectedSongIndex();
            if (index >= 0 && index < mainVM.NacteniSongu.Songs.Count - 1)
            {
                var nextSong = mainVM.NacteniSongu.Songs[index + 1];
                SongsDataGrid.SelectedItem = nextSong;
                SongsDataGrid.ScrollIntoView(nextSong);
                player.SetSong(nextSong.FileName);
                mainVM.AktualniPisnicka = nextSong.FileName;

                Slider_SongTime.Minimum = 0;
                Slider_SongTime.Maximum = (int)player.GetSongLengthSeconds();
                Slider_SongTime.Value = 0;

                player.PlayStop();
            }
        }

        private void Click_Opakovani(object sender, RoutedEventArgs e)
        {
            if (mainVM.RezimPrehravani == false)
            {
                mainVM.RezimPrehravani = true;
                IMG_RezimPrehravani.Source = new BitmapImage(new Uri("pack://application:,,,/IMG/circle-1.png", UriKind.Absolute));
            }
            else
            {
                mainVM.RezimPrehravani = false;
                IMG_RezimPrehravani.Source = new BitmapImage(new Uri("pack://application:,,,/IMG/circle.png", UriKind.Absolute));
            }
        }

        private void Click_NahodnyVyber(object sender, RoutedEventArgs e)
        {
            if (mainVM.NahodnePrehravani == false)
            {
                mainVM.NahodnePrehravani = true;
                IMG_Prehravani.Source = new BitmapImage(new Uri("pack://application:,,,/IMG/shuffle.png", UriKind.Absolute));
            }
            else
            {
                mainVM.NahodnePrehravani = false;
                IMG_Prehravani.Source = new BitmapImage(new Uri("pack://application:,,,/IMG/arrow-right.png", UriKind.Absolute));
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                // posune obsah o 3 řádky (můžeš upravit)
                double offsetChange = e.Delta > 0 ? -30 : 30;
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offsetChange);
                e.Handled = true; // zabrání dalšímu zpracování eventu
            }
        }

    }
}