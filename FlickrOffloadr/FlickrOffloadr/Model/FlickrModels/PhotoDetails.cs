using FlickrOffloadr.Convert.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickrOffloadr.Model.FlickrModels
{
    public class PhotoDetails
    {
        [JsonProperty(PropertyName = "photo")]
        public ExtendedPhoto Photo { get; set; }

    }

    public class ExtendedPhoto
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "secret")]
        public string Secret { get; set; }

        [JsonProperty(PropertyName = "server")]
        public string Server { get; set; }

        [JsonProperty(PropertyName = "farm")]
        public string Farm { get; set; }

        [JsonProperty(PropertyName = "originalformat")]
        public string Format { get; set; }

        [JsonProperty(PropertyName = "dates")]
        public PhotoDates Dates { get; set; }

    }

    public class PhotoDates
    {
        [JsonConverter(typeof(UnixToDateTimeConverter))]
        [JsonProperty(PropertyName = "posted")]
        public DateTime DatePostedUnix { get; set; }

        [JsonProperty(PropertyName = "taken")]
        public DateTime DateTaken { get; set; }

        [JsonProperty(PropertyName = "takengranularity")]
        public int TakenGranularity { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty(PropertyName = "takeunknown")]
        public bool IsDateTakenKnown { get; set; }

        [JsonConverter(typeof(UnixToDateTimeConverter))]
        [JsonProperty(PropertyName = "lastupdate")]
        public DateTime LastUpdateUnix { get; set; }
    }
}
