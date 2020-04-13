using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BackgroundUpdater.Views.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour DialogMessage.xaml
    /// </summary>
    public partial class DialogMessage : UserControl
    {
        public DialogMessage(string message)
        {
            InitializeComponent();
            this.textBlock.Text = message;
        }
    }
}
