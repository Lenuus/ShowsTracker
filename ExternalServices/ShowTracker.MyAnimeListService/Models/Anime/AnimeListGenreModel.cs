using System.Text.Json.Serialization;

namespace ShowTracker.MyAnimeListService.Models.Anime
{
    public class AnimeListGenreModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}