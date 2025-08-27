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
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace WPF_TEST
{
    /// <summary>
    /// Interakční logika pro Registrace.xaml
    /// </summary>
    public partial class Registrace : Page
    {
        public Registrace()
        {
            InitializeComponent();
        }



        // Navigace na stránku Uvod
        private void btnUvod_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Uvod());
        }



        // Placeholder funkce
        private void textEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textEmail.Text))
            {
                emailLabel.Content = "Email";
            }
            else
            {
                emailLabel.Content = string.Empty;
            }
        }



        // Placeholder funkce
        private void textHeslo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textHeslo.Text))
            {
                hesloLabel.Content = "Heslo";
            }
            else
            {
                hesloLabel.Content = string.Empty;
            }
        }



        // Funkce pro registraci
        private async void btnPrihlasit_Click(object sender, RoutedEventArgs e)
        {
            // ziskani obsahu pole vyplnění
            string email = textEmail.Text;
            string heslo = textHeslo.Text;

            // kontrola zda jsou pole vyplnena
            if (string.IsNullOrWhiteSpace(email))
            {
                emailLabel.Content = "Nevyplnil jsi email!";
                emailLabel.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }
            if (string.IsNullOrWhiteSpace(heslo))
            {
                hesloLabel.Content = "Nevyplnil jsi heslo!";
                hesloLabel.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            // Kontrola zda email a heslo splňují požadavky
            if(!new EmailAddressAttribute().IsValid(email))
            {
                errorTextBox.Text = "Neplatný e-mail.";
                errorTextBox.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }
            if (!IsValidPassword(heslo)) 
            {
                return;
            }

            // Upozornění uživatele že už se něco děje
            errorTextBox.Text = "Kontrola registrace.";
            errorTextBox.Foreground = System.Windows.Media.Brushes.Green;

            // Kontrola databáze
            try
            {
                using (Databaze db = new Databaze())
                {
                    if (await db.kontrolaRegistrace(email))
                    {
                    }
                    else
                    {
                        errorTextBox.Text = "Tento email se už používá!";
                        errorTextBox.Foreground = new SolidColorBrush(Colors.Red);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                errorTextBox.Text = ex.Message;
                errorTextBox.Foreground = new SolidColorBrush(Colors.Red);
            }

            // Upozornění uživatele že už se něco děje
            errorTextBox.Text = "Registrace.";
            errorTextBox.Foreground = System.Windows.Media.Brushes.Green;

            // Registrace do databáze
            try
            {
                using (Databaze db = new Databaze())
                {
                    db.registraceAsync(email, heslo);
                }
            }
            catch (Exception ex)
            {
                errorTextBox.Text = ex.Message;
                errorTextBox.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            // Upozornění uživatele že už se něco děje
            errorTextBox.Text = "Hotovo.";
            errorTextBox.Foreground = System.Windows.Media.Brushes.Green;

            // Navigace na stránku menu
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Menu());
        }



        // Funkce pro kontrolu hesla
        private bool IsValidPassword(string heslo)
        {
            if (heslo.Length < 6)
            {
                errorTextBox.Text = "Heslo je moc krátké! (MIN. 6)";
                errorTextBox.Foreground = System.Windows.Media.Brushes.Red;
                return false;
            }
            if (heslo.Length > 16)
            {
                errorTextBox.Text = "Heslo je moc dlouhé! (MAX. 16)";
                errorTextBox.Foreground = System.Windows.Media.Brushes.Red;
                return false;
            }

            bool jePismenoVelke = false;
            bool jePismenoMale = false;
            bool jeCislo = false;
            foreach(char znak in heslo)
            {
                for (char cislo = '0'; cislo <= '9'; cislo++)
                {
                    if (znak == cislo)
                    {
                        jeCislo = true;
                    }
                }
                for (char pismeno = 'A'; pismeno <= 'Z'; pismeno++)
                {
                    if (znak == pismeno)
                    {
                        jePismenoVelke = true;
                    }
                }
                for (char pismeno = 'a'; pismeno <= 'z'; pismeno++)
                {
                    if (znak == pismeno)
                    {
                        jePismenoMale = true;
                    }
                }
            }

            if (!jePismenoVelke)
            {
                errorTextBox.Text = "Heslo musí obsahovat velká písmena!";
                errorTextBox.Foreground = System.Windows.Media.Brushes.Red;
                return false;
            }
            if (!jePismenoMale)
            {
                errorTextBox.Text = "Heslo musí obsahovat malá písmena!";
                errorTextBox.Foreground = System.Windows.Media.Brushes.Red;
                return false;
            }
            if (!jeCislo)
            {
                errorTextBox.Text = "Heslo musí obahovat čísla!";
                errorTextBox.Foreground = System.Windows.Media.Brushes.Red;
                return false;
            }

            return true;
        }
    }
}
