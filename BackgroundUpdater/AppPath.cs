using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BackgroundUpdater
{
    /// <summary>
    /// Manage application paths, save files and backgrounds
    /// </summary>
    public class AppPath
    {
        private static readonly string appSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BackgroundUpdater";
        public static string AppSavePath
        {
            get
            {
                if (!Directory.Exists(appSavePath))
                    Directory.CreateDirectory(appSavePath);

                return appSavePath;
            }
        }

        private static readonly string appWallpaperFolder = appSavePath + "\\Wallpapers";
        public static string AppWallpaperFolder
        {
            get
            {
                if (!Directory.Exists(appWallpaperFolder))
                    Directory.CreateDirectory(appWallpaperFolder);

                return appWallpaperFolder;
            }
        }
    }
}
