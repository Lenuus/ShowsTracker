using System.Text.Json.Serialization;

namespace ShowTracker.MyAnimeListService.Models.Anime
{
    public class AnimeListSeasonModel
    {
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("season")]
        public string Season { get; set; }
    }
}