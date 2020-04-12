using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BackgroundUpdater.Helpers
{
    public class UriCreator
    {
        private static readonly Uri Wallalphacoders = new Uri(Constant.API_URL);

        public static Uri GetNewestWallpaper(string apiKey, int page=1, int infoLevel=1, int width = 0, int height = 0, Enums.OperatorType? @operator =null, int checkLast=0)
        {
            if (!Uri.TryCreate(Wallalphacoders, string.Format(Constant.NEWEST, apiKey, page, infoLevel, width, height, @operator == null ? string.Empty : @operator.ToString().ToLower(), checkLast), out var uri))
                throw new Exception("Cant create URI for newest wallpaper");
            return uri;
        }


        public static Uri GetCategoriesList(string apiKey)
        {
            if (!Uri.TryCreate(Wallalphacoders, string.Format(Constant.CATEGORY_LIST, apiKey), out var uri))
                throw new Exception("Cant create URI for categories list");

            return uri;
        }

        public static Uri GetWallpapersByCategory(string apiKey, int categoryID, int page =1, int infoLevel =1, Enums.SortType sort = Enums.SortType.Newest, int width=0, int height=0, Enums.OperatorType @operator = Enums.OperatorType.Equal, int checkLast =0)
        {
            if (!Uri.TryCreate(Wallalphacoders, string.Format(Constant.GET_WALLPAPERS_BY_CATEGORY, apiKey, categoryID, page, infoLevel, sort.ToString().ToLower(), width, height, "", checkLast), out var uri))
                throw new Exception("Cant create URI for categories list");

            return uri;
        }
    }
}
