using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlickrOffloadr.Convert.Json;
using Newtonsoft.Json;

namespace FlickrOffloadr.Model.FlickrModels
{
    public class PublicPhotos
    {
        [JsonProperty(PropertyName = "photos")]
        public Photos Photos { get; set; }

        [JsonProperty(PropertyName = "stat")]
        public string Stat { get; set; }
    }

    public class Photos
    {
        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; }

        [JsonProperty(PropertyName = "pages")]
        public int Pages { get; set; }

        [JsonProperty(PropertyName = "perpage")]
        public int PerPage { get; set; }

        [JsonProperty(PropertyName = "total")]
        public string Total { get; set; }

        [JsonProperty(PropertyName = "photo")]
        public List<Photo> PhotoList { get; set; }
    }

    public class Photo
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public string Owner { get; set; }

        [JsonProperty(PropertyName = "secret")]
        public string Secret { get; set; }

        [JsonProperty(PropertyName = "server")]
        public string Server { get; set; }

        [JsonProperty(PropertyName = "farm")]
        public string Farm { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "originalformat")]
        public string Format { get; set; }

        [JsonProperty(PropertyName = "dates")]
        public PhotoDates Dates { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty(PropertyName = "ispublic")]
        public bool IsPublic { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty(PropertyName = "isfriend")]
        public bool IsFriend { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty(PropertyName = "isfamily")]
        public bool IsFamily { get; set; }

        public SizeDetails Details { get; set; }
        public string PhotoThumb { get; set; }
        public string PhotoDetailsUrl { get; set; }

    }
}
