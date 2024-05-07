using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShowTracker.MyAnimeListService.Models.Anime
{
    public class AnimeListModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("main_picture")]
        public MangaListPictureModel Picture { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("genres")]
        public List<AnimeListGenreModel> Genres { get; set; }

        [JsonPropertyName("mean")]
        public double? Rating { get; set; }

        [JsonPropertyName("rank")]
        public int? Rank { get; set; }

        [JsonPropertyName("popularity")]
        public int? Popularity { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("num_episodes")]
        public int? EpisodeCount { get; set; }

        [JsonPropertyName("start_season")]
        public AnimeListSeasonModel Season { get; set; }

        [JsonPropertyName("broadcast")]
        public AnimeListBroadcastModel Broadcast { get; set; }
    }
}
