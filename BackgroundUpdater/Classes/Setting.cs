using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BackgroundUpdater.Classes
{
    /// <summary>
    /// Manage application settings and manage backup of activated categories
    /// </summary>
    public sealed class Setting
    {
        #region Singleton
        private static Setting instance = null;
        private static readonly object padlock = new object();

        Setting() { }

        public static Setting Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Setting();
                    }
                    return instance;
                }
            }
        }
        #endregion

        #region Values
        public bool Launch { get; set; } = false;

        public bool LaunchWindowsStarted { get; set;  } = false;

        public List<int> CategoriesActivate { get; set; } = new List<int>();

        public DateTime LastChange { get; set; }

        public Enums.SortType SortType { get; set; } = Enums.SortType.Newest;

        public bool DeleteOldWallaper { get; set; } = false;

        public WallpaperIdentity LastWallpaperIdentity { get; set; }

        public string APIKey { get; set; }

        public List<string> Favorites { get; set; }  =new List<string>();

        public bool DarkMode { get; set; } = false;
        #endregion

        [JsonIgnore]
        public bool IsWindowActif { get; set; }

        public async void SaveSetting()
        {
            if (this.LaunchWindowsStarted)
                AddStartup("BackgroundUpdater", AppDomain.CurrentDomain.BaseDirectory + "\\BackgroundUpdater.exe");
            else
                RemoveStartup("BackgroundUpdater");
            
            await File.WriteAllTextAsync(AppPath.AppSavePath + "/setting.json", JsonConvert.SerializeObject(this));
        }

        public async Task LoadSetting()
        {
            if (File.Exists(AppPath.AppSavePath + "\\setting.json"))
            {
                var json = JsonConvert.DeserializeObject<BackgroundUpdater.Classes.Setting>(await File.ReadAllTextAsync(AppPath.AppSavePath + "\\setting.json"));
                this.Launch = json.Launch;
                this.LaunchWindowsStarted = json.LaunchWindowsStarted;
                this.CategoriesActivate = json.CategoriesActivate;
                this.LastChange = json.LastChange;
                this.SortType = json.SortType;
                this.DeleteOldWallaper = json.DeleteOldWallaper;
                this.LastWallpaperIdentity = json.LastWallpaperIdentity;
                this.APIKey = json.APIKey;
                this.Favorites = json.Favorites;
                this.DarkMode = json.DarkMode;
            }
        }

        public void AddCategorieActivate(int id)
        {
            if (this.CategoriesActivate == null) CategoriesActivate = new List<int>();
            CategoriesActivate.Add(id);
            SaveSetting();
        }

        public void RemoveCategorieActivate(int id)
        {
            CategoriesActivate.Remove(id);
            SaveSetting();
        }

        public void RemoveAllCategorieActivate()
        {
            CategoriesActivate.Clear();
            SaveSetting();
        }

        /// <summary>
        /// Add application to Startup of windows
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="path"></param>
        private  void AddStartup(string appName, string path)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue(appName, "\"" + path + "\"");
            }
        }

        /// <summary>
        /// Remove application from Startup of windows
        /// </summary>
        /// <param name="appName"></param>
        private  void RemoveStartup(string appName)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue(appName, false);
            }
        }
    }
}
