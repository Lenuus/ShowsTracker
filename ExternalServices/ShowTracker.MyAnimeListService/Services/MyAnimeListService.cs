using Microsoft.Extensions.Configuration;
using ShowsTracker.Application;
using ShowTracker.MyAnimeListService.Models;
using ShowTracker.MyAnimeListService.Models.Anime;
using ShowTracker.MyAnimeListService.Models.Manga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShowTracker.MyAnimeListService.Services
{
    public class MyAnimeListService : IMyAnimeListService
    {
        private readonly IConfiguration _configuration;

        public MyAnimeListService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ServiceResponse<List<AnimeListModel>>> GetAnimesByRanking(int limit = 100, int offset = 0)
        {
            var data = new List<AnimeListModel>();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"https://api.myanimelist.net/v2/anime/ranking?ranking_type=all&fields=id,title,main_picture,start_date,end_date,genres,mean,rank,popularity,status,num_episodes,start_season,broadcast&limit={limit}&offset={offset}"))
                {
                    request.Headers.Add("X-MAL-CLIENT-ID", _configuration["MyAnimeListClientId"]);
                    var response = await client.SendAsync(request);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return new ServiceResponse<List<AnimeListModel>>(null, false, "Error when getting datas");
                    }

                    var responseJson = await response.Content.ReadAsStringAsync();
                    var responseMapped = JsonSerializer.Deserialize<PagedResponseModel<AnimeListModel>>(responseJson);
                    data.AddRange(responseMapped.Data.Select(f => f.Node));
                }
            }

            return new ServiceResponse<List<AnimeListModel>>(data, true, string.Empty);
        }

        public async Task<ServiceResponse<List<MangaListModel>>> GetMangasByRanking(int limit = 100, int offset = 0)
        {
            var data = new List<MangaListModel>();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"https://api.myanimelist.net/v2/manga/ranking?ranking_type=all&fields=id,title,main_picture,start_date,end_date,genres,mean,rank,popularity,status,num_chapters,num_volumes,media_type&limit={limit}&offset={offset}"))
                {
                    request.Headers.Add("X-MAL-CLIENT-ID", _configuration["MyAnimeListClientId"]);
                    var response = await client.SendAsync(request);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return new ServiceResponse<List<MangaListModel>>(null, false, "Error when getting datas");
                    }

                    var responseJson = await response.Content.ReadAsStringAsync();
                    var responseMapped = JsonSerializer.Deserialize<PagedResponseModel<MangaListModel>>(responseJson);
                    data.AddRange(responseMapped.Data.Select(f => f.Node));
                }
            }

            return new ServiceResponse<List<MangaListModel>>(data, true, string.Empty);
        }
    }
}
