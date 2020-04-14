using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BackgroundUpdater.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string str = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

        private PaletteHelper paletteHelper = new PaletteHelper();
        

        private int categoriesEnabled = 0;
        public int CategoriesEnabled
        {
            get => categoriesEnabled;
            set
            {
                if (value != categoriesEnabled)
                {
                    categoriesEnabled = value;
                    NotifyPropertyChanged();
                }
            }
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

        private ObservableCollection<Models.FavoriteModel> favoriteModels;
        public ObservableCollection<Models.FavoriteModel> FavoriteModels
        {
            get => favoriteModels;
            set
            {
                if (value != favoriteModels)
                {
                    favoriteModels = value;
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

        public IEnumerable<Enums.OperatorType> OperatorType
        {
            get
            {
                return Enum.GetValues(typeof(Enums.OperatorType)).Cast<Enums.OperatorType>();
            }
        }

        public string actualImagePath { get; set; }
        private BitmapImage actualWallpaper;
        /// <summary>
        /// Set in Windows API
        /// <see cref="Classes.WindowsAPI.SetBackgroud(string)"/>
        /// </summary>
        public BitmapImage ActualWallpaper
        {
            get => actualWallpaper;
            set
            {
                if (value != actualWallpaper)
                {
                    actualWallpaper = value;

                    //In case
                    actualImagePath = value.UriSource.LocalPath;

                    if (FavoriteModels != null && FavoriteModels.Count != 0 && FavoriteModels.Any(x => x.Path == actualImagePath))
                    {
                        FavoriteModels.First(x => x.Path == actualImagePath).IsActual = true;
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        private Visibility categoriesTab = Visibility.Visible;
        public Visibility CategoriesTab
        {
            get => categoriesTab;
            set
            {
                if (value != categoriesTab)
                {
                    categoriesTab = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Visibility favoritesTab = Visibility.Collapsed;
        public Visibility FavoritesTab
        {
            get => favoritesTab;
            set
            {
                if (value != favoritesTab)
                {
                    favoritesTab = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool darkMode = false;
        public bool DarkMode
        {
            get => darkMode;
            set
            {
                if (value != darkMode)
                {
                    darkMode = value;

                    ITheme theme = paletteHelper.GetTheme();
                    theme.SetBaseTheme(value ? Theme.Dark :Theme.Light);
                    paletteHelper.SetTheme(theme);

                    Classes.Setting.Instance.DarkMode = value;
                    Classes.Setting.Instance.SaveSetting();
                    NotifyPropertyChanged();
                }
            }
        }

        private Classes.WallpaperManager wallpaper;
        private TaskScheduler uiContext = null;

        public MainViewModel()
        {
        }

        private ICommand windowLoaded;
        public ICommand WindowLoaded
        {
            get
            {
                if (windowLoaded == null)
                {
                    windowLoaded = new RelayCommand<object>((obj) =>
                    {
            LoadConfig();
                    });
                }

                return windowLoaded;
            }
        }

        private async void LoadConfig()
        {
            StaticProps.SnackbarMessageQueue.Enqueue("Hello world!");

            
            Classes.Setting.Instance.IsWindowActif = true;

            LaunchAtStartup = Classes.Setting.Instance.LaunchWindowsStarted;
            SortTypeSelected = Classes.Setting.Instance.SortType;
            DeleteOldWallpapers = Classes.Setting.Instance.DeleteOldWallaper;
            DarkMode = Classes.Setting.Instance.DarkMode;
            ApiKey = Classes.Setting.Instance.APIKey;
            Width = Classes.Setting.Instance.Width;
            Height = Classes.Setting.Instance.Height;
            SizeOperatorSelected = Classes.Setting.Instance.Operator;

            if(Height ==500 && Width == 500)
            {
                if(LoadDefaultSizeSetting.CanExecute(null))
                    LoadDefaultSizeSetting.Execute(null);
            }

            string actual = Classes.WindowsAPI.GetBackgroud();
            actualImagePath = actual;
            ActualWallpaper = await Helpers.BitmapCreator.CreateBitmapAsync(actual);

            await SearchCategories(true);

            SetFavoritesListAsync();
        }

        private async void SetFavoritesListAsync()
        {
            uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            await Task.Factory.StartNew(async () =>
            {
                if (Classes.Setting.Instance.Favorites != null && Classes.Setting.Instance.Favorites.Count != 0)
                {
                    foreach (var favorite in Classes.Setting.Instance.Favorites)
                    {
                        if (!File.Exists(favorite)) continue;
                        if (FavoriteModels == null) 
                        {
                            FavoriteModels = new ObservableCollection<Models.FavoriteModel>();
                        }
                        FavoriteModels.Add(new Models.FavoriteModel
                        {
                            Path = favorite,
                            IsActual = favorite == actualImagePath,
                            Image = await Helpers.BitmapCreator.CreateBitmapAsync(favorite)
                        });
                    }
                }
            }, CancellationToken.None, TaskCreationOptions.None, uiContext);
           
        }



        private async Task SearchCategories(bool forceReload = false)
        {
            if (forceReload || CategoryModels == null || CategoryModels.Count == 0)
            {
                if (string.IsNullOrEmpty(ApiKey)) return;

                var getCategories = await Classes.Client.GetCategoriesList();
                if (!getCategories.Success)
                {
                    if (Classes.Setting.Instance.IsWindowActif)
                        await DialogHost.Show(new Views.Dialogs.DialogMessage("Error retrieving categories, message: " + getCategories.Error), dialogIdentifier: "MainDialogHost");
                    return;
                }
                if (getCategories.Categories == null && getCategories.Categories.Count == 0) { return; }

                if (CategoryModels == null)
                {
                    CategoryModels = new ObservableCollection<Models.CategoryModel>();
                    CategoryModels.CollectionChanged += CategoryModelsSource_CollectionChanged;
                }

                foreach (var categorie in getCategories.Categories)
                {
                    CategoryModels.Add(new Models.CategoryModel
                    {
                        Activate = Classes.Setting.Instance.CategoriesActivate != null && Classes.Setting.Instance.CategoriesActivate.Count != 0 ? Classes.Setting.Instance.CategoriesActivate.Exists(x => x == categorie.Id) : false,
                        Id = categorie.Id,
                        Name = categorie.Name
                    });
                }
                CategoriesEnabled = CategoryModels.Count(x => x.Activate == true);
                if (wallpaper == null) wallpaper = new Classes.WallpaperManager();
            }
        }

        private void CategoryModelsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Models.CategoryModel item in e.NewItems)
                    item.PropertyChanged += Category_PropertyChanged;

            if (e.OldItems != null)
                foreach (Models.CategoryModel item in e.OldItems)
                    item.PropertyChanged -= Category_PropertyChanged;
        }

        private void Category_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Activate")
                CategoriesEnabled = ((Models.CategoryModel)sender).Activate ? CategoriesEnabled +1: CategoriesEnabled -1;

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
                    Classes.Setting.Instance.DeleteOldWallaper = value;
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

        private  int height;
        public int Height
        {
            get => height;
            set
            {
                if (value != height)
                {
                    height = value;
                    NotifyPropertyChanged();
                    Classes.Setting.Instance.Height = value;
                    Classes.Setting.Instance.SaveSetting();
                }
            }
        }

        private int width;
        public int Width
        {
            get => width;
            set
            {
                if (value != width)
                {
                    width = value;
                    NotifyPropertyChanged();
                    Classes.Setting.Instance.Width = value;
                    Classes.Setting.Instance.SaveSetting();
                }
            }
        }

        private Enums.OperatorType sizeOperatorSelected;
        public Enums.OperatorType SizeOperatorSelected
        {
            get => sizeOperatorSelected;
            set
            {
                if (value != sizeOperatorSelected)
                {
                    sizeOperatorSelected = value;
                    NotifyPropertyChanged();
                    Classes.Setting.Instance.Operator = value;
                    Classes.Setting.Instance.SaveSetting();
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
                            if (wallpaper == null) wallpaper = new Classes.WallpaperManager();
                            wallpaper.LaunchChangeWallpaper();
                        }
                        else
                        {
                            StaticProps.SnackbarMessageQueue.Enqueue("No API Key!");
                        }
                    });
                }

                return loadAPIKey;
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
                        if (wallpaper == null) wallpaper = new Classes.WallpaperManager();
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

        private ICommand addToFavorite;
        public ICommand AddToFavorite
        {
            get
            {
                if (addToFavorite == null)
                {
                    addToFavorite = new RelayCommand<object>((obj) =>
                    {
                        Task.Run(() =>
                        {
                            if (!string.IsNullOrEmpty(actualImagePath) && File.Exists(actualImagePath))
                            {
                                FileInfo info = new FileInfo(actualImagePath);
                                string newFilePath = AppPath.AppFavoritesFolder + "\\" + info.Name;

                                if (!Classes.Setting.Instance.Favorites.Contains(newFilePath))
                                {
                                    File.Copy(actualImagePath, newFilePath, true);
                                    Classes.Setting.Instance.Favorites.Add(newFilePath);
                                    Classes.Setting.Instance.SaveSetting();
                                    StaticProps.SnackbarMessageQueue.Enqueue("The current wallpaper has been placed in the favorites");


                                    App.Current.Dispatcher.Invoke(async () =>
                                    {
                                        if (this.FavoriteModels == null) this.FavoriteModels = new ObservableCollection<Models.FavoriteModel>();
                                        this.FavoriteModels.Add(new Models.FavoriteModel
                                        {
                                            IsActual = true,
                                            Path = newFilePath,
                                            Image = await Helpers.BitmapCreator.CreateBitmapAsync(newFilePath)
                                        });
                                    });
                                }
                                else
                                {
                                    StaticProps.SnackbarMessageQueue.Enqueue("This wallpaper is already in the favorites");
                                }
                            }
                            else
                            {
                                StaticProps.SnackbarMessageQueue.Enqueue("No current wallpaper detected or missing file");
                            }

                        });
                      
                    });
                }

                return addToFavorite;
            }
        }

        private ICommand openActualWallpaper;
        public ICommand OpenActualWallpaper
        {
            get
            {
                if (openActualWallpaper == null)
                {
                    openActualWallpaper = new RelayCommand<object>((obj) =>
                    {
                        if (!string.IsNullOrEmpty(actualImagePath))
                        {
                            ProcessStartInfo psi = new ProcessStartInfo
                            {
                                FileName = "cmd",
                                Arguments = "/c start " + ActualWallpaper,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true
                            };
                            Process.Start(psi);
                        }
                    });
                }

                return openActualWallpaper;
            }
        }

        private ICommand tabMenuChanged;
        public ICommand TabMenuChanged
        {
            get
            {
                if (tabMenuChanged == null)
                {
                    tabMenuChanged = new RelayCommand<string>((obj) =>
                    {
                        CategoriesTab = obj == "CATEGORIES" ? Visibility.Visible : Visibility.Collapsed;
                        FavoritesTab = obj == "FAVORITES" ? Visibility.Visible : Visibility.Collapsed;
                    });
                }

                return tabMenuChanged;
            }
        }

        private ICommand useThisWallpaper;
        public ICommand UseThisWallpaper
        {
            get
            {
                if (useThisWallpaper == null)
                {
                    useThisWallpaper = new RelayCommand<Models.FavoriteModel>((obj) =>
                    {
                        if (!string.IsNullOrEmpty(actualImagePath) && actualImagePath == obj.Path || obj.IsActual)
                        {
                            return;
                        }

                        Classes.WindowsAPI.SetBackgroud(obj.Path);
                    });
                }

                return useThisWallpaper;
            }
        }

        private ICommand deleteThisWallpaper;
        public ICommand DeleteThisWallpaper
        {
            get
            {
                if (deleteThisWallpaper == null)
                {
                    deleteThisWallpaper = new RelayCommand<Models.FavoriteModel>((obj) =>
                    {
                        string oldPath = obj.Path;

                        //To avoid exceptions
                        //string newUri = doGetImageSourceFromResource("BackgroundUpdater", "61SDXtp8G4L.png");
                        //ActualWallpaper = newUri;
                        //obj.Path = newUri;

                        FavoriteModels.Remove(obj);
                        if (Classes.Setting.Instance.Favorites !=null && Classes.Setting.Instance.Favorites.Count != 0)
                        {
                            if (File.Exists(oldPath))
                            {
                                File.Delete(oldPath);
                            }
                            Classes.Setting.Instance.Favorites.Remove(oldPath);
                            Classes.Setting.Instance.SaveSetting();
                        }

                       
                    });
                }

                return deleteThisWallpaper;
            }
        }

        private ICommand loadDefaultSizeSetting;
        public ICommand LoadDefaultSizeSetting
        {
            get
            {
                if (loadDefaultSizeSetting == null)
                {
                    loadDefaultSizeSetting = new RelayCommand<object>((obj) =>
                    {
                        this.Height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
                        this.Width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
                        this.SizeOperatorSelected = Enums.OperatorType.Equal;
                    });
                }

                return loadDefaultSizeSetting;
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
                            Classes.Setting.Instance.IsWindowActif = false;
                        }
                        else
                        {
                            App.Current.MainWindow.Show();
                            CurWindowState = WindowState.Normal;
                            Classes.Setting.Instance.IsWindowActif = true;
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
