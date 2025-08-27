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

namespace WPF_TEST
{
    /// <summary>
    /// Interakční logika pro Uvod.xaml
    /// </summary>
    public partial class Uvod : Page
    {
        public Uvod()
        {
            InitializeComponent();
        }

        private void btnPrihlaseni_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new Prihlaseni());
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Prihlaseni());
        }

        private void btnRegistrace_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new Registrace());
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Registrace());
        }
    }
}
