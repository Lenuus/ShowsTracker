using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using ShowsTracker.Domain;
using ShowTracker.MyAnimeListService.Models.Anime;
using ShowTracker.MyAnimeListService.Models.Manga;
using ShowTracker.MyAnimeListService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Jobs
{
    public class ShowCreationJob : IJob
    {
        private readonly IRepository<Domain.Show> _showRepository;
        private readonly IRepository<Domain.Genre> _genreRepository;
        private readonly IMyAnimeListService _myAnimeListService;
        private readonly ILogger<ShowCreationJob> _logger;

        public ShowCreationJob(
            IRepository<Show> showRepository,
            IRepository<Genre> genreRepository,
            IMyAnimeListService myAnimeListService,
            ILogger<ShowCreationJob> logger)
        {
            _showRepository = showRepository;
            _genreRepository = genreRepository;
            _myAnimeListService = myAnimeListService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dbShows = await _showRepository.GetAll().ToListAsync().ConfigureAwait(false);
            var dbGenres = await _genreRepository.GetAll().ToListAsync().ConfigureAwait(false);

            for (int i = 0; i < 40; i++)
            {
                _logger.LogInformation($"Anime:{i}");
                var animes = await _myAnimeListService.GetAnimesByRanking(limit: 500, offset: (i * 500)).ConfigureAwait(false);

                var willInsertAnimes = animes.Data.Where(f => !dbShows.Exists(x => x.MyAnimeListId == f.Id));
                var existedAnimes = animes.Data.Where(f => dbShows.Exists(x => x.MyAnimeListId == f.Id));
                foreach (var anime in willInsertAnimes)
                {
                    await InsertAnimesToDb(anime, dbShows, dbGenres).ConfigureAwait(false);
                }

                foreach (var anime in existedAnimes)
                {
                    await UpdateAnimes(anime, dbShows, dbGenres).ConfigureAwait(false);
                }
            }

            for (int i = 0; i < 40; i++)
            {
                _logger.LogInformation($"Manga:{i}");
                var mangas = await _myAnimeListService.GetMangasByRanking(limit: 500, offset: (i * 500)).ConfigureAwait(false);

                var willInsertMangas = mangas.Data.Where(f => !dbShows.Exists(x => x.MyAnimeListId == f.Id));
                var existedMangas = mangas.Data.Where(f => dbShows.Exists(x => x.MyAnimeListId == f.Id));
                foreach (var manga in willInsertMangas)
                {
                    await InsertMangasToDb(manga, dbShows, dbGenres).ConfigureAwait(false);
                }

                foreach (var manga in existedMangas)
                {
                    await UpdateMangas(manga, dbShows, dbGenres).ConfigureAwait(false);
                }
            }
        }


        #region Animes
        private async Task InsertAnimesToDb(AnimeListModel model, List<Domain.Show> dbShows, List<Domain.Genre> dbGenres)
        {
            var newShow = new Domain.Show()
            {
                ApproveStatus = true,
                Category = ShowsTracker.Common.Enum.Category.Anime,
                CoverImageUrl = model.Picture != null ? model.Picture.Large : null,
                MyAnimeListId = model.Id,
                Name = model.Title,
                Popularity = model.Popularity,
                Rank = model.Rank,
                Rating = model.Rating,
                TotalEpisode = model.EpisodeCount ?? 0,
                Description = string.Empty,
                InsertedUserId = new Guid("eec6c155-2f08-4428-6464-08dc45c9fe75"),
                ReleaseGap = 1,
                ReleaseType = ShowsTracker.Common.Enum.ReleaseType.Week,
            };

            if (!string.IsNullOrEmpty(model.StartDate))
            {
                if (DateTime.TryParse(model.StartDate, out var startDate))
                {
                    newShow.StartDate = startDate;
                }
                else
                {
                    if (model.StartDate.Contains("-"))
                    {
                        newShow.StartDate = DateTime.Parse(model.StartDate + "-01");
                    }
                    else
                    {
                        newShow.StartDate = DateTime.Parse(model.StartDate + "-01" + "-01");
                    }
                }
            }

            if (!string.IsNullOrEmpty(model.EndDate))
            {
                if (DateTime.TryParse(model.EndDate, out var endDate))
                {
                    newShow.EndDate = endDate;
                }
                else
                {
                    if (model.EndDate.Contains("-"))
                    {
                        newShow.EndDate = DateTime.Parse(model.EndDate + "-31");
                    }
                    else
                    {
                        newShow.EndDate = DateTime.Parse(model.EndDate + "-12" + "-31");
                    }
                }
            }

            if (model.Broadcast != null)
            {
                switch (model.Broadcast.DayOfWeek)
                {
                    case "monday":
                        newShow.DayOfWeek = DayOfWeek.Monday; break;
                    case "tuesday":
                        newShow.DayOfWeek = DayOfWeek.Tuesday; break;
                    case "wednesday":
                        newShow.DayOfWeek = DayOfWeek.Wednesday; break;
                    case "thursday":
                        newShow.DayOfWeek = DayOfWeek.Thursday; break;
                    case "friday":
                        newShow.DayOfWeek = DayOfWeek.Friday; break;
                    case "saturday":
                        newShow.DayOfWeek = DayOfWeek.Saturday; break;
                    case "sunday":
                        newShow.DayOfWeek = DayOfWeek.Sunday; break;
                    default:
                        newShow.DayOfWeek = DayOfWeek.Monday;
                        break;
                }
            }

            IEnumerable<Domain.Genre> existedGenres;
            IEnumerable<AnimeListGenreModel> willAddGenres;
            if (model.Genres != null)
            {
                existedGenres = dbGenres.Where(f => model.Genres.Exists(x => x.Id == f.MyAnimeListId));
                willAddGenres = model.Genres.Where(f => !dbGenres.Exists(x => x.MyAnimeListId == f.Id));
            }
            else
            {
                existedGenres = new List<Domain.Genre>();
                willAddGenres = new List<AnimeListGenreModel>();
            }

            var addGenres = new List<ShowGenre>();
            addGenres.AddRange(existedGenres.Select(f => new ShowGenre
            {
                GenreId = f.Id,
            }));
            foreach (var genre in willAddGenres)
            {
                var addedData = await _genreRepository.Create(new Domain.Genre { MyAnimeListId = genre.Id, Name = genre.Name }).ConfigureAwait(false);
                dbGenres.Add(addedData);
                addGenres.Add(new ShowGenre { GenreId = addedData.Id });
            }
            newShow.Genres = addGenres;

            switch (model.Status)
            {
                case "finished_airing":
                    newShow.Status = ShowsTracker.Common.Enum.Status.Done; break;
                case "currently_airing":
                    newShow.Status = ShowsTracker.Common.Enum.Status.Ongoing; break;
                case "not_yet_aired":
                    newShow.Status = ShowsTracker.Common.Enum.Status.NotYetAired; break;
                default:
                    newShow.Status = ShowsTracker.Common.Enum.Status.None;
                    break;
            }

            if (model.Season != null)
            {
                switch (model.Season.Season)
                {
                    case "winter":
                        newShow.StartSeason = ShowsTracker.Common.Enum.StartSeason.Winter; break;
                    case "spring":
                        newShow.StartSeason = ShowsTracker.Common.Enum.StartSeason.Spring; break;
                    case "summer":
                        newShow.StartSeason = ShowsTracker.Common.Enum.StartSeason.Summer; break;
                    case "fall":
                        newShow.StartSeason = ShowsTracker.Common.Enum.StartSeason.Fall; break;
                    default:
                        newShow.StartSeason = ShowsTracker.Common.Enum.StartSeason.Winter;
                        break;
                }
            }
            await _showRepository.Create(newShow).ConfigureAwait(false);
        }

        private async Task UpdateAnimes(AnimeListModel model, List<Domain.Show> dbShows, List<Domain.Genre> dbGenres)
        {
            var dbShow = dbShows.FirstOrDefault(f => f.MyAnimeListId == model.Id);
            ShowsTracker.Common.Enum.Status newStatus;
            switch (model.Status)
            {
                case "finished_airing":
                    newStatus = ShowsTracker.Common.Enum.Status.Done; break;
                case "currently_airing":
                    newStatus = ShowsTracker.Common.Enum.Status.Ongoing; break;
                case "not_yet_aired":
                    newStatus = ShowsTracker.Common.Enum.Status.NotYetAired; break;
                default:
                    newStatus = ShowsTracker.Common.Enum.Status.None;
                    break;
            }

            bool willUpdate = false;
            if (dbShow.Status != newStatus)
            {
                dbShow.Status = newStatus;
                willUpdate = true;
            }

            if (dbShow.TotalEpisode != model.EpisodeCount)
            {
                dbShow.TotalEpisode = model.EpisodeCount ?? 0;
                willUpdate = true;
            }

            if (dbShow.Rank != model.Rank)
            {
                dbShow.Rank = model.Rank;
                willUpdate = true;
            }

            if (dbShow.Rating != model.Rating)
            {
                dbShow.Rating = model.Rating;
                willUpdate = true;
            }

            if (willUpdate)
            {
                await _showRepository.Update(dbShow).ConfigureAwait(false);
            }
        }
        #endregion

        #region Manga

        private async Task InsertMangasToDb(MangaListModel model, List<Domain.Show> dbShows, List<Domain.Genre> dbGenres)
        {
            var newShow = new Domain.Show()
            {
                ApproveStatus = true,
                CoverImageUrl = model.Picture != null ? model.Picture.Large : null,
                MyAnimeListId = model.Id,
                Name = model.Title,
                Popularity = model.Popularity,
                Rank = model.Rank,
                Rating = model.Rating,
                TotalEpisode = model.ChapterCount ?? 0,
                Description = string.Empty,
                InsertedUserId = new Guid("eec6c155-2f08-4428-6464-08dc45c9fe75"),
                ReleaseGap = 0,
                ReleaseType = ShowsTracker.Common.Enum.ReleaseType.None,
            };

            if (!string.IsNullOrEmpty(model.StartDate))
            {
                if (DateTime.TryParse(model.StartDate, out var startDate))
                {
                    newShow.StartDate = startDate;
                }
                else
                {
                    if (model.StartDate.Contains("-"))
                    {
                        newShow.StartDate = DateTime.Parse(model.StartDate + "-01");
                    }
                    else
                    {
                        newShow.StartDate = DateTime.Parse(model.StartDate + "-01" + "-01");
                    }
                }
            }

            if (!string.IsNullOrEmpty(model.EndDate))
            {
                if (DateTime.TryParse(model.EndDate, out var endDate))
                {
                    newShow.EndDate = endDate;
                }
                else
                {
                    if (model.EndDate.Contains("-"))
                    {
                        newShow.EndDate = DateTime.Parse(model.EndDate + "-31");
                    }
                    else
                    {
                        newShow.EndDate = DateTime.Parse(model.EndDate + "-12" + "-31");
                    }
                }
            }

            IEnumerable<Domain.Genre> existedGenres;
            IEnumerable<MangaListGenreModel> willAddGenres;
            if (model.Genres != null)
            {
                existedGenres = dbGenres.Where(f => model.Genres.Exists(x => x.Id == f.MyAnimeListId));
                willAddGenres = model.Genres.Where(f => !dbGenres.Exists(x => x.MyAnimeListId == f.Id));
            }
            else
            {
                existedGenres = new List<Domain.Genre>();
                willAddGenres = new List<MangaListGenreModel>();
            }

            var addGenres = new List<ShowGenre>();
            addGenres.AddRange(existedGenres.Select(f => new ShowGenre
            {
                GenreId = f.Id,
            }));
            foreach (var genre in willAddGenres)
            {
                var addedData = await _genreRepository.Create(new Domain.Genre { MyAnimeListId = genre.Id, Name = genre.Name }).ConfigureAwait(false);
                dbGenres.Add(addedData);
                addGenres.Add(new ShowGenre { GenreId = addedData.Id });
            }
            newShow.Genres = addGenres;

            switch (model.Status)
            {
                case "finished":
                    newShow.Status = ShowsTracker.Common.Enum.Status.Done; break;
                case "currently_publishing":
                    newShow.Status = ShowsTracker.Common.Enum.Status.Ongoing; break;
                case "not_yet_published":
                    newShow.Status = ShowsTracker.Common.Enum.Status.NotYetAired; break;
                default:
                    newShow.Status = ShowsTracker.Common.Enum.Status.None;
                    break;
            }

            switch (model.MediaType)
            {
                case "manga":
                case "doujinshi":
                case "one_shot":
                case "oel":
                    newShow.Category = ShowsTracker.Common.Enum.Category.Manga; break;
                case "manhwa":
                    newShow.Category = ShowsTracker.Common.Enum.Category.Manhwa; break;
                case "manhua":
                    newShow.Category = ShowsTracker.Common.Enum.Category.Manhua; break;
                case "novel":
                    newShow.Category = ShowsTracker.Common.Enum.Category.Novel; break;
                case "unknown":
                default:
                    newShow.Category = ShowsTracker.Common.Enum.Category.None;
                    break;
            }

            await _showRepository.Create(newShow).ConfigureAwait(false);
        }

        private async Task UpdateMangas(MangaListModel model, List<Domain.Show> dbShows, List<Domain.Genre> dbGenres)
        {
            var dbShow = dbShows.FirstOrDefault(f => f.MyAnimeListId == model.Id);
            ShowsTracker.Common.Enum.Status newStatus;
            switch (model.Status)
            {
                case "finished":
                    newStatus = ShowsTracker.Common.Enum.Status.Done; break;
                case "currently_publishing":
                    newStatus = ShowsTracker.Common.Enum.Status.Ongoing; break;
                case "not_yet_published":
                    newStatus = ShowsTracker.Common.Enum.Status.NotYetAired; break;
                default:
                    newStatus = ShowsTracker.Common.Enum.Status.None;
                    break;
            }

            bool willUpdate = false;
            if (dbShow.Status != newStatus)
            {
                dbShow.Status = newStatus;
                willUpdate = true;
            }

            if (dbShow.TotalEpisode != model.ChapterCount)
            {
                dbShow.TotalEpisode = model.ChapterCount ?? 0;
                willUpdate = true;
            }

            if (dbShow.Rank != model.Rank)
            {
                dbShow.Rank = model.Rank;
                willUpdate = true;
            }

            if (dbShow.Rating != model.Rating)
            {
                dbShow.Rating = model.Rating;
                willUpdate = true;
            }

            if (willUpdate)
            {
                await _showRepository.Update(dbShow).ConfigureAwait(false);
            }
        }
        #endregion
    }
}
