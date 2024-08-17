using Newtonsoft.Json;

namespace AzureCosmosDBDemo.Models
{
    public class Item
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime DueDate { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "_rid")]
        public string? Rid { get; set; }
        public string? Self { get; set; }
        public string? Etag { get; set; }
        public string? Attachments { get; set; }
        public long Ts { get; set; }
    }
}
