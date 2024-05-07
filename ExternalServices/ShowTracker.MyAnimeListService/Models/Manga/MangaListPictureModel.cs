using System.Text.Json.Serialization;

namespace ShowTracker.MyAnimeListService.Models.Manga
{
    public class MangaListPictureModel
    {
        [JsonPropertyName("medium")]
        public string Medium { get; set; }

        [JsonPropertyName("large")]
        public string Large { get; set; }
    }
}