using System.Text.Json.Serialization;

namespace ShowTracker.MyAnimeListService.Models.Anime
{
    public class MangaListPictureModel
    {
        [JsonPropertyName("medium")]
        public string Medium { get; set; }

        [JsonPropertyName("large")]
        public string Large { get; set; }
    }
}