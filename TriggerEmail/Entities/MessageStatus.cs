using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerEmail.Entities
{
    public class MessageStatus
    {
        public Guid MessageId { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string Status { get; set; }

        public string Detalis { get; set; }

        public string Subject { get; set; }

        public string TemplatePath { get; set; }

        public string EmailBlobPath { get; set; }

        public int Attempts { get; set; }

        public string EventId { get; set; }

        public string ExperimentId { get; set; }

        public string VariationId { get; set; }

        public string RequestedCulture { get; set; }

        public string RenderedCulture { get; set; }

        public Guid? BatchJobId { get; set; }

        public Guid? DeliveryId { get; set; }

        public string ViewOnlineUrl { get; set; }

        public string Token { get; set; }

        public string DeliveryStatus { get; set; }

        public IEnumerable<string> Attachments { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("============================");
            sb.AppendLine($"MessageId: {MessageId.ToString()}");
            sb.AppendLine($"LastUpdated: {LastUpdated?.ToString() ?? string.Empty}");
            sb.AppendLine($"Status: {Status}");
            sb.AppendLine($"Detalis: {Detalis}");
            sb.AppendLine($"Subject: {Subject}");
            sb.AppendLine($"TemplatePath: {TemplatePath}");
            sb.AppendLine($"EmailBlobPath: {EmailBlobPath}");
            sb.AppendLine($"Attempts : {Attempts}");
            sb.AppendLine($"EventId {EventId}");
            sb.AppendLine($"ExperimentId: {ExperimentId}");
            sb.AppendLine($"VariationId: {VariationId}");
            sb.AppendLine($"RequestedCulture: {RequestedCulture}");
            sb.AppendLine($"RenderedCulture: {RenderedCulture}");
            sb.AppendLine($"BatchJobId: {BatchJobId}");
            sb.AppendLine($"DeliveryId: {DeliveryId}");
            sb.AppendLine($"ViewOnlineUrl: {ViewOnlineUrl}");
            sb.AppendLine($"Token: {Token}");
            sb.AppendLine($"DeliveryStatus: {DeliveryStatus}");

            return sb.ToString();
        }

    }
}
