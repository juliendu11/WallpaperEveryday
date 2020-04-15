using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WallpapersEveryday.Classes.Response.Items
{
    public class WallpaperItem
    {
        /// <summary>
        /// ID of the wallpaper.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Width of the wallpaper.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        /// Height of the wallpaper.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// Format of the wallpaper.
        /// </summary>
        [JsonProperty("file_type")]
        public string FileType { get; set; }

        /// <summary>
        /// Size of the wallpaper in (octets).
        /// </summary>
        [JsonProperty("file_size")]
        public int FileSize { get; set; }

        /// <summary>
        /// URL to the wallpaper.
        /// </summary>
        [JsonProperty("url_image")]
        public Uri UrlImage { get; set; }

        /// <summary>
        /// URL to the wallpaper's thumbnail.
        /// </summary>
        [JsonProperty("url_thumb")]
        public Uri UrlThumb { get; set; }

        /// <summary>
        /// URL to the wallpaper's page on Wallpaper Abyss.
        /// </summary>
        [JsonProperty("url_page")]
        public Uri UrlPage { get; set; }

        /// <summary>
        /// [LV2]Name of the category.
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// [LV2]ID of the category.
        /// </summary>
        [JsonProperty("category_id")]
        public int CategoryId { get; set; }

        /// <summary>
        /// [LV2]Name of the sub-category.
        /// </summary>
        [JsonProperty("sub_category")]
        public string SubCategory { get; set; }

        /// <summary>
        /// [LV2]ID of ths sub-category.
        /// </summary>
        [JsonProperty("sub_category_id")]
        public int SubCategoryId { get; set; }

        /// <summary>
        /// [LV2]Submitter of the wallpaper.
        /// </summary>
        [JsonProperty("user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// [LV2]Submitter ID of the wallpaper.
        /// </summary>
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        /// <summary>
        /// [LV3]Name of the collection.
        /// </summary>
        [JsonProperty("collection")]
        public string Collection { get; set; }

        /// <summary>
        /// [LV3]ID of the collection.
        /// </summary>
        [JsonProperty("collection_id")]
        public int CollectionId { get; set; }

        /// <summary>
        /// [LV3]Name of the group.
        /// </summary>
        [JsonProperty("group")]
        public string Group { get; set; }

        /// <summary>
        /// [LV3]ID of the group.
        /// </summary>
        [JsonProperty("group_id")]
        public int GroupId { get; set; }
    }
}
