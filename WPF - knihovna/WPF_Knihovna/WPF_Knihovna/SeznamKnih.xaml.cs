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
using Microsoft.Data.Sqlite;
using System.ComponentModel;

namespace WPF_Knihovna
{
    /// <summary>
    /// Interaction logic for SeznamKnih.xaml
    /// </summary>
    public partial class SeznamKnih : UserControl
    {
        private string connectionString = "Data Source=Databaze.db";
        private List<Kniha> seznam = new List<Kniha>();
        private ICollectionView view;

        public SeznamKnih()
        {
            InitializeComponent();
            NacistKnihy();

            view = CollectionViewSource.GetDefaultView(seznam);
            KnihyGrid.ItemsSource = view;
        }

        private void NacistKnihy()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS SeznamKnih (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nazev TEXT,
                    Autor TEXT,
                    RokVydani INTEGER,
                    Zanr TEXT,
                    StavPrecteni TEXT,
                    Hodnoceni INTEGER
                )";
            createTableCmd.ExecuteNonQuery();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SeznamKnih";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var kniha = new Kniha
                {
                    Id = reader.GetInt32(0),
                    Nazev = reader.GetString(1),
                    Autor = reader.GetString(2),
                    RokVydani = reader.GetInt32(3),
                    Zanr = reader.GetString(4),
                    Stav = reader.GetString(5),
                    Hodnoceni = reader.GetInt32(6)
                };
                seznam.Add(kniha);
            }

            KnihyGrid.ItemsSource = seznam;
        }

        private void Filter()
        {
            if (view == null) return;

            string filter = SearchBox.Text.Trim().ToLower();
            string stavFilter = (StavComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            view.Filter = obj =>
            {
                if (obj is Kniha k)
                {
                    bool textMatch = string.IsNullOrEmpty(filter) ||
                                     (k.Nazev?.ToLower().Contains(filter) ?? false) ||
                                     (k.Autor?.ToLower().Contains(filter) ?? false) ||
                                     (k.Zanr?.ToLower().Contains(filter) ?? false) ||
                                     (k.RokVydani?.ToString().Contains(filter) ?? false);

                    bool stavMatch = stavFilter == "-" || k.Stav == stavFilter;

                    return textMatch && stavMatch;
                }
                return false;
            };

            view.Refresh();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void StavComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void Odstranit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var result = MessageBox.Show("Opravdu chcete odstranit tuto knihu?",
                                             "Potvrzení", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;

                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM SeznamKnih WHERE Id = $id";
                cmd.Parameters.AddWithValue("$id", id);
                cmd.ExecuteNonQuery();

                var kniha = seznam.FirstOrDefault(k => k.Id == id);
                if (kniha != null)
                {
                    seznam.Remove(kniha);
                    view.Refresh();
                }
            }
        }


        private void Upravit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var kniha = seznam.FirstOrDefault(k => k.Id == id);
                if (kniha == null) return;

                var upravitControl = new UpravitKnihu(kniha);

                RootGrid.Children.Clear();
                RootGrid.Children.Add(upravitControl);

                upravitControl.KnihaUpravena += (s, args) =>
                {
                    NacistKnihy();
                };

            }
        }


    }
}
