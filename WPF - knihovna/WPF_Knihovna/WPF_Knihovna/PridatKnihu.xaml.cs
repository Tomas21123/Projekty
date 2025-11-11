using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Knihovna
{
    /// <summary>
    /// Interaction logic for PridatKnihu.xaml
    /// </summary>
    public partial class PridatKnihu : UserControl
    {
        private string connectionString = "Data Source=Databaze.db";
        public event EventHandler KnihaPridana;

        public PridatKnihu()
        {
            InitializeComponent();
        }

        private void PridatKnihu_Click(object sender, RoutedEventArgs e)
        {
            string nazev = NazevBox.Text;
            string autor = AutorBox.Text;
            string rokText = RokBox.Text;
            string zanr = ZanrBox.Text;
            string stav = (StavBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            int hodnoceni = (int)HodnoceniSlider.Value;

            // Povinné pole Název a Autor
            if (string.IsNullOrWhiteSpace(nazev) || string.IsNullOrWhiteSpace(autor))
            {
                MessageBox.Show("Název a Autor jsou povinné.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kontrola, zda je Rok vydání číslo
            if (!int.TryParse(rokText, out int rok))
            {
                MessageBox.Show("Rok vydání musí být číslo.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO SeznamKnih (Nazev, Autor, RokVydani, Zanr, StavPrecteni, Hodnoceni)
                VALUES ($nazev, $autor, $rok, $zanr, $stav, $hodnoceni)";

            command.Parameters.AddWithValue("$nazev", nazev);
            command.Parameters.AddWithValue("$autor", autor);
            command.Parameters.AddWithValue("$rok", rok);
            command.Parameters.AddWithValue("$zanr", zanr);
            command.Parameters.AddWithValue("$stav", stav);
            command.Parameters.AddWithValue("$hodnoceni", hodnoceni);

            command.ExecuteNonQuery();

            MessageBox.Show("Kniha byla přidána!");

            // Vymazání formuláře
            NazevBox.Text = "";
            AutorBox.Text = "";
            RokBox.Text = "";
            ZanrBox.Text = "";
            StavBox.SelectedIndex = -1;
            HodnoceniSlider.Value = 0;

            KnihaPridana?.Invoke(this, EventArgs.Empty);
        }


        private void HodnoceniSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (HodnoceniText != null)
                HodnoceniText.Text = ((int)HodnoceniSlider.Value).ToString();
        }

    }
}
