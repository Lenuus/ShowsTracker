using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ShowsTracker.Application.Service.Show.Dtos;
using ShowsTracker.Common.Constants;
using ShowsTracker.Common.Dtos;
using ShowsTracker.Common.Enum;
using ShowsTracker.Common.Helpers;
using System.Linq;

namespace ShowsTracker.Application.Service.Show
{
    public class ShowService : IShowService
    {
        private readonly IRepository<Domain.Show> _showRepository;
        private readonly IMapper _mapper;
        private readonly IClaimManager _claimManager;
        private readonly IRepository<Domain.ShowLink> _showLinkRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public ShowService(
            IRepository<Domain.Show> showRepository,
            IMapper mapper,
            IClaimManager claimManager,
            IRepository<Domain.ShowLink> showLinkRepository,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _showRepository = showRepository;
            _mapper = mapper;
            _claimManager = claimManager;
            _showLinkRepository = showLinkRepository;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task<ServiceResponse> AddShow(AddShowDto request)
        {
            var entity = _mapper.Map<Domain.Show>(request);
            var userRole = _claimManager.GetRole();
            if (userRole == RoleConstants.Admin)
            {
                entity.ApproveStatus = true;
            }

            entity.InsertedUserId = _claimManager.GetUserId();
            await _showRepository.Create(entity);
            return new ServiceResponse(true, string.Empty);

        }

        public async Task<ServiceResponse> DeleteShow(Guid id)
        {
            try
            {
                await _showRepository.DeleteById(id).ConfigureAwait(false);
                return new ServiceResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, ex.Message);
            }
        }

        public async Task<ServiceResponse<PagedResponseDto<ShowListDto>>> GetAllShows(GetAllShowsDto request)
        {
            var loggedUserId = _claimManager.GetUserId();
            var query = await _showRepository.GetAll()
                .Include(i => i.Links)
                .Include(i => i.Users)
                .Include(i => i.Genres).ThenInclude(i => i.Genre)
                .Where(f => !f.IsDeleted &&
                            f.ApproveStatus &&
                            (!string.IsNullOrEmpty(request.Search) ? EF.Functions.Like(EF.Functions.Collate(f.Name, "SQL_Latin1_General_CP1_CI_AS"), $"%{request.Search}%") : true) &&
                            (request.Categories.Any() ? request.Categories.Contains(f.Category) : true) &&
                            (request.Statuses.Any() ? request.Statuses.Contains(f.Status) : true) &&
                            (request.Genres.Any() ? request.Genres.Any(y => f.Genres.Any(k => k.GenreId == y && !k.IsDeleted)) : true) &&
                            (request.StartRating.HasValue ? f.Rating >= request.StartRating : true) &&
                            (request.EndRating.HasValue ? f.Rating <= request.EndRating : true)).Select(f => new ShowListDto
                            {
                                Id = f.Id,
                                Name = f.Name,
                                TotalEpisode = f.TotalEpisode,
                                Category = f.Category,
                                Status = f.Status,
                                CoverImageUrl = f.CoverImageUrl,
                                Rating = f.Rating,
                                ReleaseGap = f.ReleaseGap,
                                ReleaseType = f.ReleaseType,
                                IsFollowedByCurrentUser = f.Users.Any(x => x.UserId == loggedUserId && !x.IsDeleted),
                                CurrentEpisode = f.Users.Any(x => x.UserId == loggedUserId && !x.IsDeleted) ? f.Users.FirstOrDefault(x => x.UserId == loggedUserId && !x.IsDeleted).CurrentEpisode : 0,
                                Genres = f.Genres.Where(f => !f.IsDeleted).Select(f => new ShowListGenreDto { Id = f.GenreId, Name = f.Genre.Name }).ToList(),
                            }).ToPagedListAsync(request.PageSize, request.PageIndex).ConfigureAwait(false);

            var showIds = query.Data.Select(x => x.Id);
            var links = await _showLinkRepository.GetAll()
                    .Where(f => !f.IsDeleted && showIds.Any(x => x == f.ShowId) && (f.UserId == loggedUserId || f.IsDefaultLink))
                    .Select(f => new
                    {
                        f.ShowId,
                        f.Name,
                        f.Link,
                        f.IsDefaultLink,
                        f.Id
                    }).ToListAsync().ConfigureAwait(false);
            foreach (var show in query.Data)
            {
                show.Links = links.Where(f => f.ShowId == show.Id).Select(f => new ShowListLinkDto { Name = f.Name, Link = f.Link, IsDefault = f.IsDefaultLink, Id = f.Id });
            }

            return new ServiceResponse<PagedResponseDto<ShowListDto>>(query, true, string.Empty);
        }

        public async Task<ServiceResponse<List<ShowListDto>>> GetRandomShows(Category category)
        {
            var randomShowIds = GetRandomShowIdsOrCreate();
            var loggedUserId = _claimManager.GetUserId();
            var query = await _showRepository.GetAll()
                .Include(i => i.Links)
                .Include(i => i.Users)
                .Include(i => i.Genres).ThenInclude(i => i.Genre)
                .Where(f => randomShowIds.Contains(f.Id) && f.Category == category)
                .Select(f => new ShowListDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    TotalEpisode = f.TotalEpisode,
                    Category = f.Category,
                    Status = f.Status,
                    CoverImageUrl = f.CoverImageUrl,
                    Rating = f.Rating,
                    ReleaseGap = f.ReleaseGap,
                    ReleaseType = f.ReleaseType,
                    IsFollowedByCurrentUser = f.Users.Any(x => x.UserId == loggedUserId && !x.IsDeleted),
                    CurrentEpisode = f.Users.Any(x => x.UserId == loggedUserId && !x.IsDeleted) ? f.Users.FirstOrDefault(x => x.UserId == loggedUserId && !x.IsDeleted).CurrentEpisode : 0,
                    Genres = f.Genres.Where(f => !f.IsDeleted).Select(f => new ShowListGenreDto { Id = f.GenreId, Name = f.Genre.Name }).ToList(),
                }).ToListAsync().ConfigureAwait(false);

            var links = await _showLinkRepository.GetAll()
                    .Where(f => !f.IsDeleted && randomShowIds.Any(x => x == f.ShowId) && (f.UserId == loggedUserId || f.IsDefaultLink))
                    .Select(f => new
                    {
                        f.ShowId,
                        f.Name,
                        f.Link,
                        f.IsDefaultLink,
                        f.Id
                    }).ToListAsync().ConfigureAwait(false);
            foreach (var show in query)
            {
                show.Links = links.Where(f => f.ShowId == show.Id).Select(f => new ShowListLinkDto { Name = f.Name, Link = f.Link, IsDefault = f.IsDefaultLink, Id = f.Id });
            }

            return new ServiceResponse<List<ShowListDto>>(query, true, string.Empty);
        }

        public async Task<ServiceResponse<List<ShowListDto>>> GetOnGoingShows()
        {
            var loggedUserId = _claimManager.GetUserId();
            var query = await _showRepository.GetAll()
                .Include(i => i.Links)
                .Include(i => i.Users)
                .Include(i => i.Genres).ThenInclude(i => i.Genre)
                .Where(f => !f.IsDeleted && f.ApproveStatus && f.Category == Category.Anime && f.Status == Status.Ongoing)
                .Select(f => new ShowListDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    TotalEpisode = f.TotalEpisode,
                    Category = f.Category,
                    Status = f.Status,
                    CoverImageUrl = f.CoverImageUrl,
                    Rating = f.Rating,
                    ReleaseGap = f.ReleaseGap,
                    ReleaseType = f.ReleaseType,
                    IsFollowedByCurrentUser = f.Users.Any(x => x.UserId == loggedUserId && !x.IsDeleted),
                    CurrentEpisode = f.Users.Any(x => x.UserId == loggedUserId && !x.IsDeleted) ? f.Users.FirstOrDefault(x => x.UserId == loggedUserId && !x.IsDeleted).CurrentEpisode : 0,
                    Genres = f.Genres.Where(f => !f.IsDeleted).Select(f => new ShowListGenreDto { Id = f.GenreId, Name = f.Genre.Name }).ToList(),
                }).ToListAsync().ConfigureAwait(false);
            var showIds = query.Select(x => x.Id);
            var links = await _showLinkRepository.GetAll()
                    .Where(f => !f.IsDeleted && showIds.Any(x => x == f.ShowId) && (f.UserId == loggedUserId || f.IsDefaultLink))
                    .Select(f => new
                    {
                        f.ShowId,
                        f.Name,
                        f.Link,
                        f.IsDefaultLink,
                        f.Id
                    }).ToListAsync().ConfigureAwait(false);
            foreach (var show in query)
            {
                show.Links = links.Where(f => f.ShowId == show.Id).Select(f => new ShowListLinkDto { Name = f.Name, Link = f.Link, IsDefault = f.IsDefaultLink, Id = f.Id });
            }

            return new ServiceResponse<List<ShowListDto>>(query, true, string.Empty);
        }

        public async Task<ServiceResponse<PagedResponseDto<UnapprovedShowListDto>>> GetUnapprovedShows(GetAllShowsDto request)
        {
            var query = await _showRepository.GetAll()
                .Include(f => f.InsertedUser)
                .Where(f => !f.IsDeleted && !f.ApproveStatus && (!string.IsNullOrEmpty(request.Search) ? (f.Name).Contains(request.Search) : true)).
                Select(f => new UnapprovedShowListDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    TotalEpisode = f.TotalEpisode,
                    Category = f.Category,
                    Status = f.Status,
                    CreatedUserMail = f.InsertedUser.Email
                }).ToPagedListAsync(request.PageSize, request.PageIndex).ConfigureAwait(false);
            return new ServiceResponse<PagedResponseDto<UnapprovedShowListDto>>(query, true, string.Empty);
        }

        public async Task<ServiceResponse> UpdateShow(UpdateShowDto request)
        {
            var show = await _showRepository.GetById(request.Id).ConfigureAwait(false);
            if (show == null)
            {
                return new ServiceResponse(false, "Not Found");
            }

            show.TotalEpisode = request.TotalEpisode;
            show.Category = request.Category;
            show.Name = request.Name;
            show.Status = request.Status;
            await _showRepository.Update(show);
            return new ServiceResponse(true, string.Empty);
        }

        private List<Guid> GetRandomShowIdsOrCreate()
        {
            List<Guid> randomListIds;
            var randomListCreated = _memoryCache.TryGetValue(ApplicationConstants.RandomShowIdKey, out randomListIds);
            if (!randomListCreated)
            {
                randomListIds = new List<Guid>();
                var randomShowRatingThreshold = Convert.ToInt32(_configuration["RandomShowRatingThreshold"]);
                var randomShowCount = Convert.ToInt32(_configuration["RandomShowCount"]);
                var randomShowRenewDayCount = Convert.ToInt32(_configuration["RandomShowRenewDayCount"]);
                var rnd = new Random();
                var groupedShows = _showRepository.GetAll().Where(f => !f.IsDeleted && f.Rating.HasValue && f.Rating.Value > randomShowRatingThreshold).GroupBy(f => f.Category);
                Guid selectedId;
                foreach (var group in groupedShows)
                {
                    for (int i = 0; i < randomShowCount; i++)
                    {
                        selectedId = group.ElementAt(rnd.Next(0, group.Count())).Id;
                        if (!randomListIds.Contains(selectedId))
                        {
                            randomListIds.Add(selectedId);
                        }
                        else
                        {
                            while (randomListIds.Contains(selectedId))
                                selectedId = group.ElementAt(rnd.Next(0, group.Count())).Id;
                            randomListIds.Add(selectedId);
                        }
                    }
                }

                _memoryCache.Set(ApplicationConstants.RandomShowIdKey, randomListIds, DateTimeOffset.UtcNow.AddDays(randomShowRenewDayCount));
            }

            return randomListIds;
        }

        public async Task<ServiceResponse<List<ShowListDto>>> FindYourTaste(FindYourTasteRequestDto request)
        {
            var loggedUserId = _claimManager.GetUserId();
            var minimumRating = Convert.ToInt32(_configuration["TasteFinderMinimumRating"]);
            var query = await _showRepository.GetAll()
                .Include(i => i.Links)
                .Include(i => i.Users)
                .Include(i => i.Genres).ThenInclude(i => i.Genre)
                .Where(f => !f.IsDeleted && f.Category == request.Category && request.Genres.Any(x => f.Genres.Any(y => y.GenreId == x)) && f.Rating >= minimumRating)
                .Select(f => new ShowListDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    TotalEpisode = f.TotalEpisode,
                    Category = f.Category,
                    Status = f.Status,
                    CoverImageUrl = f.CoverImageUrl,
                    Rating = f.Rating,
                    ReleaseGap = f.ReleaseGap,
                    ReleaseType = f.ReleaseType,
                    IsFollowedByCurrentUser = f.Users.Any(x => x.UserId == loggedUserId && !x.IsDeleted),
                    CurrentEpisode = f.Users.Any(x => x.UserId == loggedUserId && !x.IsDeleted) ? f.Users.FirstOrDefault(x => x.UserId == loggedUserId && !x.IsDeleted).CurrentEpisode : 0,
                    Genres = f.Genres.Where(x => !x.IsDeleted).Select(x => new ShowListGenreDto { Id = x.GenreId, Name = x.Genre.Name }).ToList(),
                    Links = f.Links.Where(x => !x.IsDeleted).Select(x => new ShowListLinkDto { Id = x.Id, IsDefault = x.IsDefaultLink, Link = x.Link, Name = x.Name })
                }).ToListAsync().ConfigureAwait(false);

            var tasteCount = Convert.ToInt32(_configuration["TasteFinderCount"]);
            var randomIndexes = new int[tasteCount > query.Count ? query.Count : tasteCount];
            var rnd = new Random();
            var data = new List<ShowListDto>();

            Enumerable.Range(0, randomIndexes.Length).ToList().ForEach((i) =>
            {
                int randomIndex;
                do
                {
                    randomIndex = rnd.Next(0, query.Count);
                }
                while (randomIndexes.Contains(randomIndex));
                randomIndexes[i] = randomIndex;
                data.Add(query.ElementAt(randomIndex));
            });
            return new ServiceResponse<List<ShowListDto>>(data, true, string.Empty);
        }
    }
}
