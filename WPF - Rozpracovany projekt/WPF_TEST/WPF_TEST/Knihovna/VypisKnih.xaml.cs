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

namespace WPF_TEST.Knihovna
{
    /// <summary>
    /// Interaction logic for VypisKnih.xaml
    /// </summary>
    public partial class VypisKnih : UserControl
    {
        public ObservableCollection<Kniha> Knihy { get; } = new ObservableCollection<Kniha>();

        public VypisKnih()
        {
            InitializeComponent();

            DataContext = this;
            NaplnitKnihyAsync();
        }

        private async void NaplnitKnihyAsync()
        {
            Databaze db = new Databaze();
            var seznam = await db.NavratSeznamKnihAsync();
            Knihy.Clear();
            foreach (var k in seznam)
                Knihy.Add(k);
        }

    }
}
