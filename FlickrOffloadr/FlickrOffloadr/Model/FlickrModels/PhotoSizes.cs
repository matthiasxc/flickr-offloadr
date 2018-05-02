using FlickrOffloadr.Convert.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickrOffloadr.Model.FlickrModels
{
    public class PhotoSizes
    {
        [JsonProperty(PropertyName = "sizes")]
        public SizeDetails Sizes { get; set; }

        [JsonProperty(PropertyName = "stat")]
        public string stat { get; set; }
    }

    public class SizeDetails
    {
        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty(PropertyName = "canblog")]
        public bool CanBlog { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty(PropertyName = "canprint")]
        public bool CanPrint { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty(PropertyName = "candownload")]
        public bool CanDownload { get; set; }

        [JsonProperty(PropertyName = "size")]
        public List<SizeInfo> Sizes { get; set; }

    }

    public class SizeInfo
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "width")]
        public string Width { get; set; }

        [JsonProperty(PropertyName = "height")]
        public string Height { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "media")]
        public string Media { get; set; }

    }

}
