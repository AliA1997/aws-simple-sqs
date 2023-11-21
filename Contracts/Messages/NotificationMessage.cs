using Newtonsoft.Json;

namespace Contracts.Messages
{
    public class NotificationMessage : IMessage
    {
        [JsonProperty("messageType")]
        public string MessageType { get; set; } = String.Empty;
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; } 
        [JsonProperty("notification")]
        public string Notification { get; set; } = string.Empty;
        [JsonIgnore]
        public string MessageTypeName => nameof(NotificationMessage);
    }
}
