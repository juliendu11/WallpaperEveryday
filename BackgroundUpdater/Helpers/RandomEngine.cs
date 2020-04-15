using System;
using System.Collections.Generic;
using System.Text;

namespace WallpapersEveryday.Helpers
{
    public static class RandomEngine
    {
        private static readonly Random random = new Random(DateTime.Now.Second);
        public static T GetRandom<T>(List<T> list)
        {
            int index = random.Next(list.Count);
            return list[index];
        }
    }
}
