using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShowTracker.MyAnimeListService.Models.Manga
{
    public class MangaListModel
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
        public List<MangaListGenreModel> Genres { get; set; }

        [JsonPropertyName("mean")]
        public double? Rating { get; set; }

        [JsonPropertyName("rank")]
        public int? Rank { get; set; }

        [JsonPropertyName("popularity")]
        public int? Popularity { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("num_volumes")]
        public int? VolumeCount { get; set; }

        [JsonPropertyName("num_chapters")]
        public int? ChapterCount { get; set; }

        [JsonPropertyName("media_type")]
        public string MediaType { get; set; }
    }
}
