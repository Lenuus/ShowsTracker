using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShowsTracker.Application;
using ShowsTracker.Common.Helpers;
using ShowTracker.MyAnimeListService.Models.Anime;
using ShowTracker.MyAnimeListService.Services;
using System.Net.Sockets;
using ShowsTracker.Domain;
using System.Xml.Linq;
using ShowTracker.MyAnimeListService.Models.Manga;
using ShowsTracker.Persistence;

#region Initalize Configuration and Services
IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var myAnimelistService = new MyAnimeListService(Configuration);

var serviceProvider = new ServiceCollection();
serviceProvider.AddDbContext<MainDbContext>(
    option => option.UseSqlServer(Configuration.GetConnectionString("MainDbContext")));
serviceProvider.AddTransient(typeof(IRepository<>), typeof(Repository<>));
var services = serviceProvider.BuildServiceProvider();
#endregion

var showRepository = services.GetRequiredService<IRepository<Show>>();
var genreRepository = services.GetRequiredService<IRepository<Genre>>();
var dbShows = showRepository.GetAll().ToList();
var dbGenres = genreRepository.GetAll().ToList();

#region Animes

for (int i = 20; i < 40; i++)
{
    Console.WriteLine($"{(i + 1)}. sayfa başlatıldı");
    var animes = myAnimelistService.GetAnimesByRanking(limit: 500, offset: (i * 500)).Result;

    var willInsertAnimes = animes.Data.Where(f => !dbShows.Exists(x => x.MyAnimeListId == f.Id));
    var existedAnimes = animes.Data.Where(f => dbShows.Exists(x => x.MyAnimeListId == f.Id));
    foreach (var anime in willInsertAnimes)
    {
        InsertAnimesToDb(anime);
    }

    foreach (var anime in existedAnimes)
    {
        UpdateAnimes(anime);
    }
    Console.WriteLine($"{(i + 1)}. sayfa yüklendi");
}

void InsertAnimesToDb(AnimeListModel model)
{
    var newShow = new Show()
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

    IEnumerable<Genre> existedGenres;
    IEnumerable<AnimeListGenreModel> willAddGenres;
    if (model.Genres != null)
    {
        existedGenres = dbGenres.Where(f => model.Genres.Exists(x => x.Id == f.MyAnimeListId));
        willAddGenres = model.Genres.Where(f => !dbGenres.Exists(x => x.MyAnimeListId == f.Id));
    }
    else
    {
        existedGenres = new List<Genre>();
        willAddGenres = new List<AnimeListGenreModel>();
    }

    var addGenres = new List<ShowGenre>();
    addGenres.AddRange(existedGenres.Select(f => new ShowGenre
    {
        GenreId = f.Id,
    }));
    foreach (var genre in willAddGenres)
    {
        var addedData = genreRepository.Create(new Genre { MyAnimeListId = genre.Id, Name = genre.Name }).Result;
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
    showRepository.Create(newShow).Wait();
}

void UpdateAnimes(AnimeListModel model)
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
        showRepository.Update(dbShow).Wait();
    }
}
#endregion

#region Manga
for (int i = 20; i < 40; i++)
{
    Console.WriteLine($"{(i + 1)}. sayfa başlatıldı");
    var mangas = myAnimelistService.GetMangasByRanking(limit: 500, offset: (i * 500)).Result;

    var willInsertMangas = mangas.Data.Where(f => !dbShows.Exists(x => x.MyAnimeListId == f.Id));
    var existedMangas = mangas.Data.Where(f => dbShows.Exists(x => x.MyAnimeListId == f.Id));
    foreach (var manga in willInsertMangas)
    {
        InsertMangasToDb(manga);
    }

    foreach (var manga in existedMangas)
    {
        UpdateMangas(manga);
    }
    Console.WriteLine($"{(i + 1)}. sayfa yüklendi");
}

void InsertMangasToDb(MangaListModel model)
{
    var newShow = new Show()
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

    IEnumerable<Genre> existedGenres;
    IEnumerable<MangaListGenreModel> willAddGenres;
    if (model.Genres != null)
    {
        existedGenres = dbGenres.Where(f => model.Genres.Exists(x => x.Id == f.MyAnimeListId));
        willAddGenres = model.Genres.Where(f => !dbGenres.Exists(x => x.MyAnimeListId == f.Id));
    }
    else
    {
        existedGenres = new List<Genre>();
        willAddGenres = new List<MangaListGenreModel>();
    }

    var addGenres = new List<ShowGenre>();
    addGenres.AddRange(existedGenres.Select(f => new ShowGenre
    {
        GenreId = f.Id,
    }));
    foreach (var genre in willAddGenres)
    {
        var addedData = genreRepository.Create(new Genre { MyAnimeListId = genre.Id, Name = genre.Name }).Result;
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

    showRepository.Create(newShow).Wait();
}

void UpdateMangas(MangaListModel model)
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
        showRepository.Update(dbShow).Wait();
    }
}
#endregion
return 0;
