using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace WallpapersEveryday
{
    public static class StaticProps
    {
        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static void NotifyStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        private static SnackbarMessageQueue snackbarMessageQueue = new SnackbarMessageQueue();
        public static SnackbarMessageQueue SnackbarMessageQueue
        {
            get => snackbarMessageQueue;
        }

        private static Visibility showProgressBar = Visibility.Hidden;
        public static Visibility ShowProgressBar
        {
            get => showProgressBar;
            set
            {
                if (value != showProgressBar)
                {
                    showProgressBar = value;
                    NotifyStaticPropertyChanged();
                }
            }
        }


    }
}
