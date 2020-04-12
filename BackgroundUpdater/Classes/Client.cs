using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BackgroundUpdater.Classes
{
    /// <summary>
    /// Handles requests to send to the API
    /// </summary>
    public sealed class Client
    {
        private static Client instance = null;
        private static readonly object padlock = new object();

        Client() { }

        public static Client Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Client();
                    }
                    return instance;
                }
            }
        }

        private readonly HttpClient client = new HttpClient();

        public string API_KEY { get; set; }

        public async Task<Classes.Response.WallpapersList> GetNewestWallpaper(int page, int infoLevel =1)
        {
            var getNewestWallpaper = await client.GetAsync(Helpers.UriCreator.GetNewestWallpaper(API_KEY, page, infoLevel));
            if (getNewestWallpaper.IsSuccessStatusCode)
            {
                string responseBody = await getNewestWallpaper.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<Classes.Response.WallpapersList>(responseBody);

                return json;
            }
            else
            {
                return new Classes.Response.WallpapersList{ 
                     Success = false, Error = $"Bad status code: {getNewestWallpaper.StatusCode}", Wallpapers =  null }
                ;
            }
        }

        public async Task<Classes.Response.CategoriesList> GetCategoriesList()
        {
            var getCategoriesList = await client.GetAsync(Helpers.UriCreator.GetCategoriesList(API_KEY));
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

        public async Task<Classes.Response.WallpapersList> GetWallpapersByCategory(int categoryID, Enums.SortType sort)
        {
            var getWallpapersByCategory = await client.GetAsync(Helpers.UriCreator.GetWallpapersByCategory(API_KEY, categoryID, sort: sort));
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
    }
}
