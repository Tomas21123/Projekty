using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SlovníFotbal
{
    public partial class Hra : Page
    {
        // Proměnné
        public string posledniSlovo = "Auto";
        private int kdo = 1;

        private DispatcherTimer countdownTimer;
        private const int StartSeconds = 15;
        private int remainingSeconds = StartSeconds;

        // Počáteční nastavení
        public Hra()
        {
            InitializeComponent();

            txt_minuleSlovo.Text = posledniSlovo;
            lbl_kteryHrac.Content = "Hráč: " + kdo;
            lbl_kteryHrac.Foreground = Brushes.Blue;

            countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            countdownTimer.Tick += CountdownTimer_Tick;

            ResetCountdown();
        }

        // Počitadlo času
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            remainingSeconds--;
            txt_zbivajiciCas.Text = remainingSeconds.ToString();

            // Pokud odpočet skončí (konec hry)
            if (remainingSeconds <= 0)
            {
                countdownTimer.Stop();
                txt_zbivajiciCas.Text = "0";
                MessageBox.Show("Vypršel čas!\nHráč: " + kdo + " prohrál.", "Konec hry", MessageBoxButton.OK, MessageBoxImage.Information);
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Uvod());
            }
        }

        // Začátek odpočtu
        private void StartCountdown()
        {
            if (!countdownTimer.IsEnabled)
                countdownTimer.Start();
        }

        // Zastavení odpočtu
        private void StopCountdown()
        {
            if (countdownTimer.IsEnabled)
                countdownTimer.Stop();
        }

        // Reset odpočtu
        private void ResetCountdown()
        {
            remainingSeconds = StartSeconds;
            txt_zbivajiciCas.Text = remainingSeconds.ToString();
            StopCountdown();
            StartCountdown();
        }

        // Tlačítko pro odeslání a ověření odpovědi
        private async void btn_odpoved(object sender, RoutedEventArgs e)
        {
            // Pokud hraješ proti pc a je na řadě tak nemůžeš odpovídat
            if (Globals.jePcNaRade)
                return;

            // Kontrola odpovědi
            var vstup = txt_zadaneSlovo.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(vstup) || vstup.StartsWith(" Zadej slovo ...") || vstup.StartsWith("Hrac:"))
            {
                // Pokud chyba tak vyhodí okénko s upozorněním
                MessageBox.Show("Zadej prosím platné slovo.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kontrola zda je poslední odpověděné slovo v pořádku
            if (string.IsNullOrEmpty(posledniSlovo))
            {
                MessageBox.Show("Interní chyba: chybí poslední slovo.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kontrola zda sedí nové slovo s minulým slovem
            if (posledniSlovo[^1] != vstup[0])
            {
                MessageBox.Show("Zadané slovo nesedí.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Nastavení noveho posledního slova
            txt_minuleSlovo.Text = vstup;
            posledniSlovo = vstup;
            txt_zadaneSlovo.Text = string.Empty;

            // Určuje kdo bude odpovídat příště (pokud hraješ proti PC tak --> PC = hrac 2)
            kdo = (kdo == 1) ? 2 : 1;

            Globals.jePcNaRade = Globals.hrajePc && (kdo == 2);

            // Rozhoduje se kdo bude odpovídat
            if (Globals.hrajePc && Globals.jePcNaRade)
            {
                lbl_kteryHrac.Content = "Počítač";
                lbl_kteryHrac.Foreground = Brushes.Red;
                ResetCountdown();
                await pcHledaOdpoved(posledniSlovo[^1]);
            }
            else
            {
                lbl_kteryHrac.Content = "Hráč: " + kdo;
                lbl_kteryHrac.Foreground = (kdo == 1) ? Brushes.Blue : Brushes.Red;
                ResetCountdown();
            }
        }

        // Pomocná funkce pro TextBox
        private void TxtZadaneSlovo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_zadaneSlovo.Text))
            {
                txt_zadaneSlovo.Text = " Zadej slovo ...";
                txt_zadaneSlovo.Foreground = Brushes.Gray;
            }
        }

        // Pomocná funkce pro TextBox
        private void TxtZadaneSlovo_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_zadaneSlovo.Text == " Zadej slovo ...")
            {
                txt_zadaneSlovo.Text = "";
                txt_zadaneSlovo.Foreground = Brushes.Black;
            }
        }

        // Funkce kde počítač hledá vhodnou odpověď z databáze slov
        private async Task pcHledaOdpoved(char z)
        {
            StopCountdown();

            var db = new Databaze();
            int pokusy = 0;
            const int maxPokusy = 3;
            string? slovo = null;

            while (pokusy < maxPokusy)
            {
                slovo = await db.NajdiSlovoZacinajiciNaAsync(z);
                if (!string.IsNullOrEmpty(slovo))
                    break;

                pokusy++;
                await Task.Delay(200);
            }

            if (string.IsNullOrEmpty(slovo))
            {
                MessageBox.Show("Počítač nenašel odpověď.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                Globals.jePcNaRade = false;
                lbl_kteryHrac.Content = "Hráč: " + kdo;
                lbl_kteryHrac.Foreground = (kdo == 1) ? Brushes.Blue : Brushes.Red;
                ResetCountdown();
                return;
            }

            txt_minuleSlovo.Text = slovo;
            posledniSlovo = slovo;
            Globals.jePcNaRade = false;

            kdo = 1;
            lbl_kteryHrac.Content = "Hráč: " + kdo;
            lbl_kteryHrac.Foreground = Brushes.Blue;

            ResetCountdown();
        }
    }
}
