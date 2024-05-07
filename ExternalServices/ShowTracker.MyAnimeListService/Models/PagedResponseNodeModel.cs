using System.Text.Json.Serialization;

namespace ShowTracker.MyAnimeListService.Models
{
    public class PagedResponseNodeModel<T>
    {
        [JsonPropertyName("node")]
        public T Node { get; set; }
    }
}