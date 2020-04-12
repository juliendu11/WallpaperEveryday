using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackgroundUpdater.Classes.Response.Items
{
    public class CategoryItem
    {
        /// <summary>
        /// ID of the category.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Name of the category.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Number of wallpaper in the category.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// URL to the category's page on Wallpaper Abyss.
        /// </summary>
        [JsonProperty("string")]
        public int url { get; set; }
    }
}
