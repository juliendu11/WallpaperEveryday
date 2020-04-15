using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WallpapersEveryday.Helpers
{
    public class BitmapCreator
    {
        public static async Task<BitmapImage> CreateBitmapAsync(string path, bool useAwait = false)
        {
            return await Task.Run(async () =>
            {
                if (useAwait)
                    await Task.Delay(TimeSpan.FromSeconds(2));

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(path);
                image.EndInit();
                image.Freeze();
                return image;

            });
        }

        public static async Task<BitmapImage> CreateBitmapAsync(Uri path)
        {
            return await Task.Run(() =>
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = path;
                image.EndInit();
                image.Freeze();
                return image;
            });
        }
    }
}
