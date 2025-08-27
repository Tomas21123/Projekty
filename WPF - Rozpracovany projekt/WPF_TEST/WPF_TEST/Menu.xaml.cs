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
using WPF_TEST.Knihovna;

namespace WPF_TEST
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void HeaderText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Kliknul jsi na horní text!");
            // Tady můžeš přidat jinou logiku podle potřeby
        }

        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                MessageBox.Show($"Kliknuto na tlačítko {btn.Content}");
            }
        }

        private void BtnPrepnutiNaUcet(object sender, RoutedEventArgs e)
        {

        }

        private void BtnVypisKnih(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new VypisKnih();
        }
    }
}
