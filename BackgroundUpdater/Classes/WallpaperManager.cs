using MaterialDesignThemes.Wpf;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BackgroundUpdater.Classes
{
    /// <summary>
    /// Manages everything related to setting the wallpaper and managing the day change
    /// </summary>
    public class WallpaperManager
    {
        private readonly Timer timer;
        private DayOfWeek actualDay;
        private DateTime actualDate;

        public DayOfWeek ActualDay
        {
            get => actualDay;
            set
            {
                if (value != actualDay)
                {
                    actualDay = value;
                    DayChanged();
                }
            }
        }


        public DateTime ActualDate
        {
            get => actualDate;
            set
            {
                if (value != actualDate)
                {
                    actualDate = value;
                }
            }
        }

        public WallpaperManager()
        {
            //First use
            ActualDate = DateTime.Now;
            DayChanged();

            //Every 1 hours
            timer = new Timer((state) => ActualDate = DateTime.Now, null, 0, 3600000);
        }

        private void DayChanged()
        {
            Log.Information("Day changed");

            if (string.IsNullOrEmpty(Setting.Instance.APIKey))
            {
                Log.Information("No API Key");
                StaticProps.SnackbarMessageQueue.Enqueue("No API Key");
                return;
            }

            Log.Information("Last date:{LastDate} -- Actual date:{TodayDate}", Classes.Setting.Instance.LastChange.Date, ActualDate.Date);

            if (Classes.Setting.Instance.LastChange.Date != ActualDate.Date)
            {
                Log.Information("The dates are different");
                LaunchChangeWallpaper();
            }
        }

        public void LaunchChangeWallpaper()
        {
            Log.Information("Launch change wallpaper process");

            StaticProps.SnackbarMessageQueue.Enqueue("Update wallpaper in progress");
            StaticProps.ShowProgressBar = Visibility.Visible;


            if (string.IsNullOrEmpty(Setting.Instance.APIKey))
            {
                Log.Information("No API Key");
                StaticProps.ShowProgressBar = Visibility.Hidden;

                StaticProps.SnackbarMessageQueue.Enqueue("No API Key");
                return;
            }

            SearchWallpaperAsync();
            Classes.Setting.Instance.LastChange = actualDate;
            Classes.Setting.Instance.SaveSetting();
        }


        /// <summary>
        /// Search wallpaper , select wallaper and save it on Setting.Instance.LastWallpaperIdentity
        /// <see cref="Setting.Instance.LastWallpaperIdentity"/>
        /// </summary>
        private async void SearchWallpaperAsync()
        {
            if (Classes.Setting.Instance.CategoriesActivate != null && Classes.Setting.Instance.CategoriesActivate.Count != 0)
            {

                Log.Information("Activated categories are available");

                Log.Information("Sort: {SortType}", Setting.Instance.SortType);

                int randomID = Helpers.RandomEngine.GetRandom<int>(Classes.Setting.Instance.CategoriesActivate);
                var getWallpapers = await Classes.Client.GetWallpapersByCategory(randomID, Setting.Instance.SortType);
                if (!getWallpapers.Success)
                {
                    Log.Error("Error getting wallpaper, message: {Message}", getWallpapers.Error);
                    StaticProps.ShowProgressBar = Visibility.Hidden;

                    if (Setting.Instance.IsWindowActif)
                    {
                        await DialogHost.Show(new Views.Dialogs.DialogMessage("Error getting wallpaper, message: " + getWallpapers.Error), dialogIdentifier: "MainDialogHost");
                    }
                    return;
                }
                if (getWallpapers.Wallpapers ==null || getWallpapers.Wallpapers.Count == 0)
                {
                    Log.Error("No wallpaper available");
                    StaticProps.ShowProgressBar = Visibility.Hidden;

                    if (Setting.Instance.IsWindowActif)
                    {
                        await DialogHost.Show(new Views.Dialogs.DialogMessage("No wallpaper available"), dialogIdentifier: "MainDialogHost");
                    }
                    return;
                }
                var wallpaperUri = Helpers.RandomEngine.GetRandom<Classes.Response.Items.WallpaperItem>(getWallpapers.Wallpapers);
                string newWallpaperFile = AppPath.AppWallpaperFolder + $"\\[{wallpaperUri.CategoryId}]" + wallpaperUri.Id + ".jpg";

                Setting.Instance.LastWallpaperIdentity = new WallpaperIdentity { CategoryId = wallpaperUri.CategoryId, WallpaperId = wallpaperUri.Id };
                Setting.Instance.SaveSetting();

                Log.Information("Download in progress, wallpaper id: {WallpaperID}, category id: {CategoryId}", wallpaperUri.Id, randomID);

                using (var webClient = new WebClient())
                {
                    try
                    {
                        await webClient.DownloadFileTaskAsync(wallpaperUri.UrlImage, newWallpaperFile);

                    }
                    catch (Exception ex) 
                    {
                        if (Setting.Instance.IsWindowActif)
                        {
                            await DialogHost.Show(new Views.Dialogs.DialogMessage("Error when download wallpaper, uri: " + wallpaperUri.UrlImage + Environment.NewLine + ex.Message), dialogIdentifier: "MainDialogHost");
                        }
                        Log.Error(ex, "Error when download wallpaper, uri: " + wallpaperUri.UrlImage);
                    }
                }
                SetNewWallpaper(newWallpaperFile);
            }
            else
            {
                StaticProps.ShowProgressBar = Visibility.Hidden;
                Log.Information("No activated categories are available");
                if (Setting.Instance.IsWindowActif)
                {
                    if (DialogHost.CloseDialogCommand.CanExecute(null, null))
                        DialogHost.CloseDialogCommand.Execute(null, null);
                    await DialogHost.Show(new Views.Dialogs.DialogMessage("No activated categories are available"), dialogIdentifier: "MainDialogHost");
                }
            }
        }


        /// <summary>
        /// Use Windows API for set wallaper and delete other wallpaper if option is enabled
        /// </summary>
        /// <param name="newWallpaperFile">Path of new wallpaper</param>
        private async void SetNewWallpaper(string newWallpaperFile)
        {
            await Task.Run(async () =>
            {
                Log.Information("Download complete");

                if (!File.Exists(newWallpaperFile))
                {
                    Log.Error("No file available");
                    if (Setting.Instance.IsWindowActif)
                    {
                        await DialogHost.Show(new Views.Dialogs.DialogMessage("No file available for set in wallpaper"), dialogIdentifier: "MainDialogHost");
                    }
                    StaticProps.ShowProgressBar = Visibility.Hidden;

                    return;
                }

                if (Setting.Instance.DeleteOldWallaper)
                {
                    try
                    {
                        var oldFiles = Helpers.FastDirectoryEnumerator.GetFileList("*.jpg", AppPath.AppWallpaperFolder).ToList();
                        if (oldFiles != null && oldFiles.Count != 0)
                        {
                            Parallel.ForEach(oldFiles, oldWallpaper => {
                                if (oldWallpaper.FullName != newWallpaperFile)
                                {
                                    if (File.Exists(oldWallpaper.FullName))
                                        File.Delete(oldWallpaper.FullName);
                                }
                            });
                        }
                    }
                    catch { }
                }
                

                WindowsAPI.SetBackgroud(newWallpaperFile);
                StaticProps.ShowProgressBar = Visibility.Hidden;
                StaticProps.SnackbarMessageQueue.Enqueue("New wallpaper set!!");
            });
        }

    }
}
