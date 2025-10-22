using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WPF_TEST.Sklad
{
    /// <summary>
    /// Interaction logic for VypisSkladu.xaml
    /// </summary>
    public partial class VypisSkladu : UserControl
    {
        public ObservableCollection<Produkt> Produkty { get; set; } = new ObservableCollection<Produkt>();

        const string zaklad_ID = "---";
        const string zaklad_Nazev = "N/A";
        const string zaklad_pozice = "N/A";
        const string zaklad_MnozstviMin = "MIN";
        const string zaklad_MnozstviMax = "MAX";

        public VypisSkladu()
        {
            InitializeComponent();
            DataContext = this;
            NaplnSkladAsync();
        }

        private async void NaplnSkladAsync()
        {
            Databaze db = new Databaze();
            var vypis = await db.NavratVypisSkladuAsync();
            Produkty.Clear();
            foreach (var item in vypis)
            {
                Produkty.Add(item);
            }
        }

        private void btn_Filtrovat_Click(object sender, RoutedEventArgs e)
        {
            var FiltrovaneProdukty = new ObservableCollection<Produkt>();

            foreach (var item in Produkty)
            {
                if (TextBox_ID.Text != zaklad_ID)
                {
                    if(item.Id != int.Parse(TextBox_ID.Text))
                    {
                        continue;
                    }
                }

                if (TextBox_Nazev.Text != zaklad_Nazev)
                {
                    if(item.Nazev != TextBox_Nazev.Text)
                    {
                        string query = (TextBox_Nazev.Text ?? "").Trim();
                        if (item?.Nazev?.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            // našlo to
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                if(TextBox_Pozice.Text != zaklad_pozice)
                {
                    if (!string.Equals(item.PoziceNaSklade, TextBox_Pozice.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                if (TextBox_MnozstviMin.Text != zaklad_MnozstviMin)
                {
                    if (item.Mnozstvi < int.Parse(TextBox_MnozstviMin.Text))
                    {
                        continue;
                    }
                }

                if(TextBox_MnozstviMax.Text != "MAX")
                {
                    if (item.Mnozstvi > int.Parse(TextBox_MnozstviMax.Text))
                    {
                        continue;
                    }
                }

                FiltrovaneProdukty.Add(item);
            }

            Produkty.Clear();
            foreach(var item in FiltrovaneProdukty)
            {
                Produkty.Add(item);
            }
        }

        private void btn_ZrusitFiltr_Click(object sender, RoutedEventArgs e)
        {
            NaplnSkladAsync();

            TextBox_ID.Text = zaklad_ID;
            TextBox_Nazev.Text = zaklad_Nazev;
            TextBox_Pozice.Text = zaklad_pozice;
            TextBox_MnozstviMin.Text = zaklad_MnozstviMin;
            TextBox_MnozstviMax.Text = zaklad_MnozstviMax;
        }
    }
}
