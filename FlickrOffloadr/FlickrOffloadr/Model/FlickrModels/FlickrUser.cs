using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickrOffloadr.Model.FlickrModels
{
    public class UserCall
    {
        [JsonProperty(PropertyName = "user")]
        public FlickrUser User { get; set; }
        [JsonProperty(PropertyName = "stat")]
        public string Stat { get; set; }
    }

    public class FlickrUser
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "username")]
        public Username UserName { get; set; }

    }

    public class Username
    {
        [JsonProperty(PropertyName = "_content")]
        public string Name { get; set; }
    }
}
