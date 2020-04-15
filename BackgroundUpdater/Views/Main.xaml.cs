using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WallpapersEveryday.Views
{
    /// <summary>
    /// Logique d'interaction pour Main.xaml
    /// </summary>
    public partial class Main : Window
    {

        public Main()
        {
            InitializeComponent();
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console(outputTemplate:
               "[{Timestamp:dd/MM/yyyy HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
               .WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + "logs/myapp.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();

            Log.Information("Launch");
        }
    }
}
