using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media.Imaging;

namespace WallpapersEveryday.Models
{
    public class FavoriteModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string path;
        private bool isActual = false;
        private BitmapImage image;

        public BitmapImage Image
        {
            get => image;
            set
            {
                if (value != image)
                {
                    image = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Image"));
                }
            }
        }

        public string Path
        {
            get => path;
            set
            {
                if (value != path)
                {
                    path = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Path"));
                }
            }
        }

        public bool IsActual
        {
            get => isActual;
            set
            {
                if (value != isActual)
                {
                    isActual = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsActual"));
                }
            }
        }
    }
}
