using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WallpapersEveryday.Classes.Response
{
    public abstract class Result
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
