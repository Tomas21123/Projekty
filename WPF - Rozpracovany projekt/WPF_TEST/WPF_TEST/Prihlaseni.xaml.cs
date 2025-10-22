using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using WPF_TEST.User;

namespace WPF_TEST
{
    /// <summary>
    /// Interakční logika pro Prihlaseni.xaml
    /// </summary>
    public partial class Prihlaseni : Page
    {
        public Prihlaseni()
        {
            InitializeComponent();

            // načtení posledního emailu
            if (!string.IsNullOrWhiteSpace(LocalStorage.LastEmail))
            {
                textEmail.Text = LocalStorage.LastEmail;
                emailLabel.Content = string.Empty;
            }

            // načtení hesla (pokud je uloženo)
            var stored = LocalStorage.SavedPasswordEncrypted;
            if (!string.IsNullOrEmpty(stored))
            {
                textHeslo.Text = stored;
                rememberCheckBox.IsChecked = true;
                hesloLabel.Content = string.Empty;
            }
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

        // Přihlašovací funkce
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

            // Upozornění uživatele že už se něco děje
            errorTextBox.Text = "Kontrola Přihlašování.";
            errorTextBox.Foreground = System.Windows.Media.Brushes.Green;

            try
            {
                using (Databaze db = new Databaze())
                {
                    bool loginOk = await db.kontrolaLogin(email, heslo);
                    if (!loginOk)
                    {
                        errorTextBox.Text = "Špatný email nebo heslo!";
                        errorTextBox.Foreground = Brushes.Red;
                        return;
                    }

                    var uss = await db.UserSessionSetData(email, heslo);
                    UserSession.Instance.Set(uss);

                    // uložení do LocalStorage
                    LocalStorage.LastEmail = uss.Email;
                    LocalStorage.LastUserId = uss.Id;

                    if (rememberCheckBox.IsChecked == true)
                    {
                        LocalStorage.SavedPasswordEncrypted = heslo;
                    }
                    else
                    {
                        LocalStorage.SavedPasswordEncrypted = string.Empty;
                    }

                    ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Menu());
                }
            }
            catch (Exception ex)
            {
                errorTextBox.Text = ex.Message;
                errorTextBox.Foreground = Brushes.Red;
            }
        }

    }
}
