using ShowsTracker.Application;
using ShowTracker.MyAnimeListService.Models.Anime;
using ShowTracker.MyAnimeListService.Models.Manga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowTracker.MyAnimeListService.Services
{
    public interface IMyAnimeListService
    {
        Task<ServiceResponse<List<AnimeListModel>>> GetAnimesByRanking(int limit = 100, int offset = 0);

        Task<ServiceResponse<List<MangaListModel>>> GetMangasByRanking(int limit = 100, int offset = 0);
    }
}
