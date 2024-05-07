using System.Text.Json.Serialization;

namespace ShowTracker.MyAnimeListService.Models.Manga
{
    public class MangaListGenreModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}