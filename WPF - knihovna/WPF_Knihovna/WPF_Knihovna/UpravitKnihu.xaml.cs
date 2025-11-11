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
using WPF_Knihovna.Models;

namespace WPF_Knihovna
{
    /// <summary>
    /// Interaction logic for UpravitKnihu.xaml
    /// </summary>
    public partial class UpravitKnihu : UserControl
    {
        public event EventHandler KnihaUpravena;

        private Kniha upravovanaKniha;

        public UpravitKnihu(Kniha kniha)
        {
            InitializeComponent();
            upravovanaKniha = kniha;

            NazevBox.Text = kniha.Nazev;
            AutorBox.Text = kniha.Autor;
            RokBox.Text = kniha.RokVydani?.ToString() ?? "";
            ZanrBox.Text = kniha.Zanr;
            StavBox.SelectedItem = StavBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => (string)i.Content == kniha.Stav);
            HodnoceniSlider.Value = kniha.Hodnoceni ?? 1;
        }

        private void Ulozit_Click(object sender, RoutedEventArgs e)
        {
            upravovanaKniha.Nazev = NazevBox.Text;
            upravovanaKniha.Autor = AutorBox.Text;
            upravovanaKniha.RokVydani = int.TryParse(RokBox.Text, out int r) ? r : null;
            upravovanaKniha.Zanr = ZanrBox.Text;
            upravovanaKniha.Stav = (StavBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            upravovanaKniha.Hodnoceni = (int)HodnoceniSlider.Value;

            using var connection = new SqliteConnection("Data Source=Databaze.db");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
            UPDATE SeznamKnih
            SET Nazev=$nazev, Autor=$autor, RokVydani=$rok, Zanr=$zanr, StavPrecteni=$stav, Hodnoceni=$hodnoceni
            WHERE Id=$id";
            command.Parameters.AddWithValue("$nazev", upravovanaKniha.Nazev);
            command.Parameters.AddWithValue("$autor", upravovanaKniha.Autor);
            command.Parameters.AddWithValue("$rok", upravovanaKniha.RokVydani.HasValue ? (object)upravovanaKniha.RokVydani.Value : DBNull.Value);
            command.Parameters.AddWithValue("$zanr", upravovanaKniha.Zanr);
            command.Parameters.AddWithValue("$stav", upravovanaKniha.Stav);
            command.Parameters.AddWithValue("$hodnoceni", upravovanaKniha.Hodnoceni ?? 1);
            command.Parameters.AddWithValue("$id", upravovanaKniha.Id);
            command.ExecuteNonQuery();

            KnihaUpravena?.Invoke(this, EventArgs.Empty);
        }

        private void HodnoceniSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (HodnoceniText != null)
                HodnoceniText.Text = ((int)HodnoceniSlider.Value).ToString();
        }
    }

}
