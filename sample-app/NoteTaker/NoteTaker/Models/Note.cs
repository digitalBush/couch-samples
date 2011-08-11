using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace NoteTaker.Models
{
    public class Note
    {
        [JsonProperty("_id")]
        public Guid Id { get; set; }

        [JsonProperty("_rev",NullValueHandling=NullValueHandling.Ignore)]
        public string Revision { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }
        public string Tags { get; set; }
    }
}