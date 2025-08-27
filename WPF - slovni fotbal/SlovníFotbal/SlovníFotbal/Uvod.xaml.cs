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

namespace SlovníFotbal
{
    public static class Globals
    {
        public static bool hrajePc = false;
        public static bool jePcNaRade = false;
    }

    public partial class Uvod : Page
    {
        public Uvod()
        {
            InitializeComponent();
            txt_nadpis.Text = "Zahraj si\n slovní fotbálek";
        }

        private void btn_hra(object sender, RoutedEventArgs e)
        {
            Globals.hrajePc = false;
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Hra());
        }

        private void btn_hraSPocitacem(object sender, RoutedEventArgs e)
        {
            Globals.hrajePc = true;
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Hra());
        }
    }
}
