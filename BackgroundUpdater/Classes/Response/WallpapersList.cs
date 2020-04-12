using BackgroundUpdater.Classes.Response.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackgroundUpdater.Classes.Response
{
    public class WallpapersList : Result
    {
        [JsonProperty("wallpapers")]
        public List<WallpaperItem> Wallpapers { get; set; }
    }
}
