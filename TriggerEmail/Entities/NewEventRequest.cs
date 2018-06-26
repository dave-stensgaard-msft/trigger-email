using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TriggerEmail.Entities
{
    public class NewEventRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("cc")]
        public string Cc { get; set; }

        [JsonProperty("subscriptionId")]
        public Guid? SubscriptionId { get; set; }

        [JsonProperty("eventId")]
        public string EventId { get; set; }

        [JsonProperty("culture")]
        public string Culture { get; set; }

        [JsonProperty("messageId")]
        public Guid? MessageId { get; set; }

        [JsonProperty("values")]
        public Dictionary<string, object> Values { get; set; }
    }
}
