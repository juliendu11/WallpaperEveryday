using WallpapersEveryday.Classes.Response.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WallpapersEveryday.Classes.Response
{
    public class WallpapersList : Result
    {
        [JsonProperty("wallpapers")]
        public List<WallpaperItem> Wallpapers { get; set; }
    }
}
