using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WallpapersEveryday.Helpers
{
    public static class CheckInternetConnection
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
