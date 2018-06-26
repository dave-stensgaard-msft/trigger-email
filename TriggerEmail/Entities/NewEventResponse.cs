using Newtonsoft.Json;
using System;

namespace TriggerEmail.Entities
{
    public class NewEventResponse
    {
        [JsonProperty("messageIds")]
        public Guid[] MessageIds { get; set; }
    }
}
