using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Serilog;

namespace BackgroundUpdater.Classes
{
    /// <summary>
    /// Handles requests to send to the API
    /// </summary>
    public static class Client
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<Classes.Response.WallpapersList> GetNewestWallpaper(int page, int infoLevel =1)
        {
            try
            {
                var getNewestWallpaper = await client.GetAsync(Helpers.UriCreator.GetNewestWallpaper(Setting.Instance.APIKey, page, infoLevel));
                if (getNewestWallpaper.IsSuccessStatusCode)
                {
                    string responseBody = await getNewestWallpaper.Content.ReadAsStringAsync();
                    var json = JsonConvert.DeserializeObject<Classes.Response.WallpapersList>(responseBody);

                    return json;
                }
                else
                {
                    return new Classes.Response.WallpapersList
                    {
                        Success = false,
                        Error = $"Bad status code: {getNewestWallpaper.StatusCode}",
                        Wallpapers = null
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting newest wallpaper with API, message: " + ex.Message);
                return  new Classes.Response.WallpapersList
                {
                    Success = false,
                    Error = "Error newest wallpaper with API, message: " + ex.Message,
                    Wallpapers = null
                };
            }
           
        }

        public static async Task<Classes.Response.CategoriesList> GetCategoriesList()
        {
            try
            {
                var getCategoriesList = await client.GetAsync(Helpers.UriCreator.GetCategoriesList(Setting.Instance.APIKey));
                if (getCategoriesList.IsSuccessStatusCode)
                {
                    string responseBody = await getCategoriesList.Content.ReadAsStringAsync();
                    var json = JsonConvert.DeserializeObject<Classes.Response.CategoriesList>(responseBody);

                    return json;
                }
                else
                {
                    return new Classes.Response.CategoriesList
                    {
                        Success = false,
                        Error = $"Bad status code: {getCategoriesList.StatusCode}"
                    };
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error getting categories with API, message: " + ex.Message);
                return new Classes.Response.CategoriesList
                {
                    Success = false,
                    Error = "Error getting categories with API, message: " + ex.Message,
                };
            }
          
        }

        public static async Task<Classes.Response.WallpapersList> GetWallpapersByCategory(int categoryID, Enums.SortType sort)
        {
            try
            {
                var getWallpapersByCategory = await client.GetAsync(Helpers.UriCreator.GetWallpapersByCategory(Setting.Instance.APIKey, categoryID, sort: sort));
                if (getWallpapersByCategory.IsSuccessStatusCode)
                {
                    string responseBody = await getWallpapersByCategory.Content.ReadAsStringAsync();
                    var json = JsonConvert.DeserializeObject<Classes.Response.WallpapersList>(responseBody);

                    return json;
                }
                else
                {
                    return new Classes.Response.WallpapersList
                    {
                        Success = false,
                        Error = $"Bad status code: {getWallpapersByCategory.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting wallpaper with specific cateogry with API, message: " + ex.Message);
                return new Classes.Response.WallpapersList
                {
                    Success = false,
                    Error = "Error getting wallpaper with specific cateogry with API, message: " + ex.Message,
                };
            }
            
        }
    }
}
