using BackgroundUpdater.Classes.Response.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackgroundUpdater.Classes.Response
{
    public class CategoriesList : Result
    {
        [JsonProperty("categories")]
        public List<CategoryItem> Categories { get; set; }
    }

    
}
