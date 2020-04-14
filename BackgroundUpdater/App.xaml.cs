using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BackgroundUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        async void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            bool startMinimized = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "/StartMinimized")
                {
                    startMinimized = true;
                }
            }

            await Classes.Setting.Instance.LoadSetting();

            // Create main application window, starting minimized if specified
            Views.Main mainWindow = new Views.Main();
            if (startMinimized)
            {
                var v = (ViewModels.MainViewModel)mainWindow.DataContext;

                mainWindow.Hide();
                v.CurWindowState = WindowState.Minimized;
                Classes.Setting.Instance.IsWindowActif = false;
            }
            else
            {
                mainWindow.Show();
            }
        }
    }
}
