using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BackgroundUpdater.Classes
{
    /// <summary>
    /// Use windows API for set wallpaper
    /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/ab83d0c3-0b82-4353-b447-38ad297dfece/how-to-change-the-wallpaper-programmatically?forum=csharpgeneral
    /// </summary>
    public class WindowsAPI
    {
        /// <summary>
        /// set the parameter of system
        /// </summary>
        /// <param name="uAction"></param>
        /// <param name="uParam"></param>
        /// <param name="lpvParam"></param>
        /// <param name="fuWinIni"></param>
        /// <example></example>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(UAction uAction, int uParam, StringBuilder lpvParam, int fuWinIni);
        public enum UAction
        {
            /// <summary>
            /// set the desktop background image
            /// </summary>
            SPI_SETDESKWALLPAPER = 0x0014,
            /// <summary>
            /// set the desktop background image
            /// </summary>
            SPI_GETDESKWALLPAPER = 0x0073,
        }
        public static string GetBackgroud()
        {
            StringBuilder s = new StringBuilder(300);
            SystemParametersInfo(UAction.SPI_GETDESKWALLPAPER, 300, s, 0);
            return s.ToString();
        }
        /// <summary>
        /// set the desktop background image
        /// </summary>
        /// <param name="fileName">the path of image</param>
        /// <returns></returns>
        public static int SetBackgroud(string fileName)
        {
            int result = 0;
            if (File.Exists(fileName))
            {
                StringBuilder s = new StringBuilder(fileName);
                result = SystemParametersInfo(UAction.SPI_SETDESKWALLPAPER, 0, s, 0x2);

                Log.Information("Wallpaper placement complete");
            }
            return result;
        }
        /// <summary>
        /// set the option of registry
        /// </summary>
        /// <param name="optionsName">the name of registry</param>
        /// <param name="optionsData">set the data of registry</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool SetOptions(string optionsName, string optionsData, ref string msg)
        {
            bool returnBool = true;
            RegistryKey classesRoot = Registry.CurrentUser;
            RegistryKey registryKey = classesRoot.OpenSubKey(@"Control Panel\Desktop", true);
            try
            {
                if (registryKey != null)
                {
                    registryKey.SetValue(optionsName.ToUpper(), optionsData);
                }
                else
                {
                    returnBool = false;
                }
            }
            catch
            {
                returnBool = false;
                msg = "Error when read the registry";
            }
            finally
            {
                classesRoot.Close();
                registryKey.Close();
            }
            return returnBool;
        }
    }
}
