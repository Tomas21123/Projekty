using System.Configuration;
using System.Data;
using System.Windows;

namespace Stopky
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            mainWindow.ulozitData();
        }
    }

}
