using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptionMeApp
{
    public class photocaptioninformation
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        public DateTime DateUtc { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string DateDisplay { get { return DateUtc.ToLocalTime().ToString("d"); } }

        [Newtonsoft.Json.JsonIgnore]
        public string TimeDisplay { get { return DateUtc.ToLocalTime().ToString("t"); } }

        [JsonProperty(PropertyName = "Caption")]
        public string Caption { get; set; }
    }
}
