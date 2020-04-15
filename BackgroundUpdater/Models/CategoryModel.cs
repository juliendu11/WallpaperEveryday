using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WallpapersEveryday.Models
{
    public class CategoryModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int id;
        private string name;


        private bool activate;

        public bool Activate
        {
            get => activate;
            set
            {
                if (value != activate)
                {
                    activate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Activate"));
                }
            }
        }

        public int Id
        {
            get => id;
            set
            {
                if (value != id)
                {
                    id = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
                }
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (value != name)
                {
                    name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
    }
}
