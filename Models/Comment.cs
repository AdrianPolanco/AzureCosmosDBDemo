namespace AzureCosmosDBDemo.Models
{
    public class Comment
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
