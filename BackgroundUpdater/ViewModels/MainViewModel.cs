using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BackgroundUpdater.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string str = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }


        private WindowState curWindowState = WindowState.Normal;
        public WindowState CurWindowState
        {
            get => curWindowState;
            set
            {
                if (value != curWindowState) { curWindowState = value; NotifyPropertyChanged(); }
            }
        }


        private ObservableCollection<Models.CategoryModel> categoryModels;
        public ObservableCollection<Models.CategoryModel> CategoryModels
        {
            get => categoryModels;
            set
            {
                if (value != categoryModels)
                {
                    categoryModels = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public IEnumerable<Enums.SortType> SortType
        {
            get
            {
                return Enum.GetValues(typeof(Enums.SortType)).Cast<Enums.SortType>();
            }
        }

        private Classes.WallpaperManager wallpaper;

        public MainViewModel()
        {
            LoadConfig();
        }

        private async void LoadConfig()
        {
            await Classes.Setting.Instance.LoadSetting();

            LaunchAtStartup = Classes.Setting.Instance.LaunchWindowsStarted;
            SortTypeSelected = Classes.Setting.Instance.SortType;
            DeleteOldWallpapers = Classes.Setting.Instance.DeleteOldWallaper;
            ApiKey = Classes.Setting.Instance.APIKey;

            await SearchCategories(true);
        }


        private async Task SearchCategories(bool forceReload = false)
        {
            if (forceReload || CategoryModels == null || CategoryModels.Count == 0)
            {
                var getCategories = await Classes.Client.Instance.GetCategoriesList();
                if (!getCategories.Success)
                {
                    //Error
                    return;
                }
                if (getCategories.Categories == null && getCategories.Categories.Count == 0) { return; }

                if (CategoryModels == null) CategoryModels = new ObservableCollection<Models.CategoryModel>();

                foreach (var categorie in getCategories.Categories)
                {
                    CategoryModels.Add(new Models.CategoryModel
                    {
                        Activate = Classes.Setting.Instance.CategoriesActivate != null && Classes.Setting.Instance.CategoriesActivate.Count != 0 ? Classes.Setting.Instance.CategoriesActivate.Exists(x => x == categorie.Id) : false,
                        Id = categorie.Id,
                        Name = categorie.Name
                    });
                }
                wallpaper = new Classes.WallpaperManager();
            }
        }

        private string apiKey;
        public string ApiKey
        {
            get => apiKey;
            set
            {
                if (value != apiKey)
                {
                    apiKey = value;
                    NotifyPropertyChanged();
                   
                }
            }
        }

        private ICommand loadAPIKey;
        public ICommand LoadAPIKey
        {
            get
            {
                if (loadAPIKey == null)
                {
                    loadAPIKey = new RelayCommand<object>(async (obj) =>
                    {
                        Classes.Setting.Instance.APIKey = ApiKey;
                        Classes.Setting.Instance.SaveSetting();
                        if (!string.IsNullOrEmpty(ApiKey))
                        {
                            await SearchCategories();
                            wallpaper.LaunchChangeWallpaper();
                        }
                    });
                }

                return loadAPIKey;
            }
        }

        private bool launchAtStartup;
        public bool LaunchAtStartup
        {
            get => launchAtStartup;
            set
            {
                if (value != launchAtStartup)
                {
                    launchAtStartup = value;
                    NotifyPropertyChanged();
                    Classes.Setting.Instance.LaunchWindowsStarted = value;
                    Classes.Setting.Instance.SaveSetting();
                }
            }
        }

        private bool deleteOldWallpapers;
        public bool DeleteOldWallpapers
        {
            get => deleteOldWallpapers;
            set
            {
                if (value != deleteOldWallpapers)
                {
                    deleteOldWallpapers = value;
                    NotifyPropertyChanged();
                    Classes.Setting.Instance.LaunchWindowsStarted = value;
                    Classes.Setting.Instance.SaveSetting();
                }
            }
        }

        private Enums.SortType sortTypeSelected;
        public Enums.SortType SortTypeSelected
        {
            get => sortTypeSelected;
            set
            {
                if(value != sortTypeSelected)
                {
                    sortTypeSelected = value;
                    NotifyPropertyChanged();
                    Classes.Setting.Instance.SortType = value;
                    Classes.Setting.Instance.SaveSetting();
                }
            }
        }

        private ICommand disableAllCategory;
        public ICommand DisableAllCategory
        {
            get
            {
                if (disableAllCategory == null)
                {
                    disableAllCategory = new RelayCommand<object>((obj) =>
                    {
                       if (CategoryModels !=null && CategoryModels.Count != 0)
                        {
                            foreach (var category in CategoryModels)
                                category.Activate = false;

                            Classes.Setting.Instance.RemoveAllCategorieActivate();
                        }
                    });
                }

                return disableAllCategory;
            }
        }

        private ICommand forceLoadNewWallpaper;
        public ICommand ForceLoadNewWallpaper
        {
            get
            {
                if (forceLoadNewWallpaper == null)
                {
                    forceLoadNewWallpaper = new RelayCommand<Models.CategoryModel>(async (obj) =>
                    {
                        await SearchCategories();
                        wallpaper.LaunchChangeWallpaper();
                    });
                }

                return forceLoadNewWallpaper;
            }
        }

        private ICommand categoryCheckedChanged;
        public ICommand CategoryCheckedChanged
        {
            get
            {
                if (categoryCheckedChanged == null)
                {
                    categoryCheckedChanged = new RelayCommand<Models.CategoryModel>((obj) =>
                    {
                        if (obj.Activate)
                        {
                            if (Classes.Setting.Instance.CategoriesActivate == null) Classes.Setting.Instance.CategoriesActivate = new List<int>();
                            Classes.Setting.Instance.AddCategorieActivate(obj.Id);
                        }
                        else
                        {
                            Classes.Setting.Instance.RemoveCategorieActivate(obj.Id);
                        }
                    });
                }

                return categoryCheckedChanged;
            }
        }

        private ICommand editWindow;
        public ICommand EditWindow
        {
            get
            {
                if (editWindow == null)
                {
                    editWindow = new RelayCommand<object>( (obj) =>
                    {
                        if (CurWindowState == WindowState.Normal || CurWindowState == WindowState.Maximized)
                        {
                            App.Current.MainWindow.Hide();
                            CurWindowState = WindowState.Minimized;
                        }
                        else
                        {
                            App.Current.MainWindow.Show();
                            CurWindowState = WindowState.Normal;
                        }
                      
                    });
                }

                return editWindow;
            }
        }

        private ICommand exit;
        public ICommand Exit
        {
            get
            {
                if (exit == null)
                {
                    exit = new RelayCommand<object>((obj) =>
                    {
                        App.Current.Shutdown(0);
                    });
                }

                return exit;
            }
        }

    }
}
