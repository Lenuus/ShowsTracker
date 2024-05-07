using System.Text.Json.Serialization;

namespace ShowTracker.MyAnimeListService.Models.Anime
{
    public class AnimeListBroadcastModel
    {
        [JsonPropertyName("day_of_the_week")]
        public string DayOfWeek { get; set; }

        [JsonPropertyName("start_time")]
        public string StartTime { get; set; }
    }
}