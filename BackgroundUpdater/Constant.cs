using System;
using System.Collections.Generic;
using System.Text;

namespace BackgroundUpdater
{
    /// <summary>
    /// API endpoints
    /// https://wall.alphacoders.com/api.php
    /// </summary>
    public class Constant
    {
        public const string API_URL = "https://wall.alphacoders.com/api2.0/";
        public const string API_AUTH = API_URL + "get.php?auth={0}";

        public const string NEWEST = API_AUTH + "&method=newest&page={1}&info_level={2}&width={3}&height={4}&operator={5}&check_last={6}";

        public const string CATEGORY_LIST = API_AUTH + "&method=category_list";

        public const string GET_WALLPAPERS_BY_CATEGORY = API_AUTH + "&method=category&id={1}&page={2}&info_level={3}&sort={4}&width={5}&height={6}&operator={7}&check_last={8}";
    }
}
