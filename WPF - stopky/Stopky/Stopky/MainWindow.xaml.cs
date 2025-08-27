using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using System.IO.MemoryMappedFiles;
using System.IO;

namespace Stopky
{
    // Třída kde uchovávám záznam
    public class CasovyZaznam
    {
        public int Poradi {  get; set; }
        public string Cas { get; set; }
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Seznam záznamů který se aktualizuje v realném čase
        public ObservableCollection<CasovyZaznam> CasoveZaznamy { get; set; } = new ObservableCollection<CasovyZaznam>();

        // Proěnné pro ovládání stopek
        private OvladaniStopek cas = new OvladaniStopek();
        private readonly DispatcherTimer dispatcherTimer;

        private double aktualniCas;
        public double AktualniCas
        {
            get => aktualniCas;
            set
            {
                if (value != aktualniCas)
                {
                    aktualniCas = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AktualniCas)));
                }
            }
        }

        // Cesty k souborům
        private string path1 = "numbers.txt";
        private string path2 = "numbersOrder.txt";

        // Počáteční nastavení
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            nacistData();

            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            dispatcherTimer.Tick += (s, e) => AktualniCas = cas.NavratCas();
        }



        // Tlačítko pro odstranění časového záznamu
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CasovyZaznam record)
                CasoveZaznamy.Remove(record);
        }



        // Tlačítko pro přidání časového záznamu
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var newEntry = new CasovyZaznam
            {
                Poradi = CasoveZaznamy.Count + 1,
                Cas = AktualniCas.ToString("F2") + "s"
            };
            CasoveZaznamy.Add(newEntry);
        }



        // Zapnutí stopek
        private void btn_start(object sender, RoutedEventArgs e)
        {
            cas.Start();
            dispatcherTimer.Start();
        }



        // Stopnutí stopek
        private void btn_stop(object sender, RoutedEventArgs e)
        {
            cas.Stop();
        }



        // Vyresetuje stopky
        private void btn_restart(object sender, RoutedEventArgs e)
        {
            cas.Reset();
            AktualniCas = 0;
            dispatcherTimer.Stop();
        }



        // Ukládání dat do souborů
        public void ulozitData()
        {
            var lines = CasoveZaznamy.Select(z => $"{z.Poradi};{z.Cas}");
            File.WriteAllLines(path1, lines);
        }



        // Načítání dat ze souborů
        private void nacistData()
        {
            if (File.Exists(path1))
            {
                foreach (var line in File.ReadAllLines(path1))
                {
                    var parts = line.Split(';');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int poradi))
                    {
                        CasoveZaznamy.Add(new CasovyZaznam { Poradi = poradi, Cas = parts[1] });
                    }
                }
            }
        }



        // Funkce pro posun levého sloupce myší
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

    }


    // Třída pro měření času
    public class OvladaniStopek()
    {
        private readonly Stopwatch cas = new Stopwatch();

        public void Start() => cas.Start();

        public void Stop() => cas.Stop();

        public void Reset() => cas.Reset();

        public double NavratCas() => cas.Elapsed.TotalSeconds;
    }
}