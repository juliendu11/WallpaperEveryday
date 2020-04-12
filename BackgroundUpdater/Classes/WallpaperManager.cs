using Serilog;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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

            if (string.IsNullOrEmpty(Client.Instance.API_KEY))
            {
                Log.Information("No API Key");
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

            if (string.IsNullOrEmpty(Client.Instance.API_KEY))
            {
                Log.Information("No API Key");
                return;
            }

            SearchWallpaperAsync();
            Classes.Setting.Instance.LastChange = actualDate;
            Classes.Setting.Instance.SaveSetting();
        }


        private async void SearchWallpaperAsync()
        {
            if (Classes.Setting.Instance.CategoriesActivate != null && Classes.Setting.Instance.CategoriesActivate.Count != 0)
            {

                Log.Information("Activated categories are available");

                Log.Information("Sort: {SortType}", Setting.Instance.SortType);

                int randomID = Helpers.RandomEngine.GetRandom<int>(Classes.Setting.Instance.CategoriesActivate);
                var getWallpapers = await Classes.Client.Instance.GetWallpapersByCategory(randomID, Setting.Instance.SortType);
                if (!getWallpapers.Success)
                {
                    Log.Error("Error getting wallpaper, message: {Message}", getWallpapers.Error);
                    return;
                }
                if (getWallpapers.Wallpapers ==null || getWallpapers.Wallpapers.Count == 0)
                {
                    Log.Error("No wallpaper available");
                    return;
                }
                var wallpaperUri = Helpers.RandomEngine.GetRandom<Classes.Response.Items.WallpaperItem>(getWallpapers.Wallpapers);
                string newWallpaperFile = AppPath.AppWallpaperFolder + $"\\[{wallpaperUri.CategoryId}]" + wallpaperUri.Id + ".jpg";

                Setting.Instance.LastWallpaperIdentity = new WallpaperIdentity { CategoryId = wallpaperUri.CategoryId, WallpaperId = wallpaperUri.Id };
                Setting.Instance.SaveSetting();

                Log.Information("Download in progress, wallpaper id: {WallpaperID}, category id: {CategoryId}", wallpaperUri.Id, randomID);

                using (var webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(wallpaperUri.UrlImage, newWallpaperFile);
                }
                SetNewWallpaper(newWallpaperFile);
            }
            else
                Log.Information("No activated categories are available");
        }

        private async void SetNewWallpaper(string newWallpaperFile)
        {
            await Task.Run(() =>
            {
                Log.Information("Download complete");

                if (!File.Exists(newWallpaperFile))
                {
                    Log.Error("No file available");
                    return;
                }

                if (Setting.Instance.DeleteOldWallaper)
                {
                    var oldFiles = Helpers.FastDirectoryEnumerator.GetFileList("*.jpg", AppPath.AppWallpaperFolder);
                    if (oldFiles != null)
                    {
                        Parallel.ForEach(oldFiles, oldWallpaper => {
                            if (File.Exists(oldWallpaper.FullName))
                                File.Delete(oldWallpaper.FullName);
                        });
                    }
                }
                

                WindowsAPI.SetBackgroud(newWallpaperFile);

            });
        }

    }
}
