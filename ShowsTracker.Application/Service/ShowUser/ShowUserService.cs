using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ShowsTracker.Application.Service.Show.Dtos;
using ShowsTracker.Application.Service.ShowUser.Dtos;
using ShowsTracker.Common.Dtos;
using ShowsTracker.Common.Helpers;
using ShowsTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.ShowUser
{
    public class ShowUserService : IShowUserService
    {
        private readonly IRepository<Domain.Show> _showRepository;
        private readonly IRepository<Domain.User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<Domain.ShowUser> _showUserRepository;
        private readonly IClaimManager _claimManager;
        private readonly IRepository<Domain.ShowLink> _showLinkRepository;

        public ShowUserService(
            IMapper mapper,
            IRepository<Domain.User> userRepository,
            IRepository<Domain.Show> showRepository,
            IRepository<Domain.ShowUser> showUserRepository,
            IClaimManager claimManager,
            IRepository<Domain.ShowLink> showLinkRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _showRepository = showRepository;
            _showUserRepository = showUserRepository;
            _claimManager = claimManager;
            _showLinkRepository = showLinkRepository;
        }

        public async Task<ServiceResponse<Guid>> AddLinkToShow(AddLinkToShowRequestDto request)
        {
            var existedShow = await _showRepository.GetById(request.ShowId).ConfigureAwait(false);
            if (existedShow == null)
            {
                return new ServiceResponse<Guid>(Guid.Empty, false, "Not Found");
            }

            var loggedUserId = _claimManager.GetUserId();
            var countOfLinksOfUser = await _showLinkRepository.GetAll().CountAsync(f => f.UserId == loggedUserId && f.ShowId == request.ShowId && !f.IsDeleted).ConfigureAwait(false);
            if (countOfLinksOfUser > 7)
            {
                return new ServiceResponse<Guid>(Guid.Empty, false, "Cannot add more links. You can add maximum 7 links at the same time.");
            }

            if (!request.Link.IsUrlValid())
            {
                return new ServiceResponse<Guid>(Guid.Empty, false, "Link is not valid.");
            }

            var link = await _showLinkRepository.Create(new ShowLink
            {
                ShowId = request.ShowId,
                Link = request.Link,
                Name = request.Name,
                IsDefaultLink = false,
                UserId = loggedUserId
            }).ConfigureAwait(false);
            return new ServiceResponse<Guid>(link.Id, true, string.Empty);
        }

        public async Task<ServiceResponse> DeleteLink(Guid id)
        {
            var loggedUser = _claimManager.GetUserId();
            var existedLink = await _showLinkRepository.GetAll().FirstOrDefaultAsync(f => f.UserId == loggedUser && f.Id == id && !f.IsDeleted).ConfigureAwait(false);
            if (existedLink == null)
            {
                return new ServiceResponse(false, "Not Found");
            }

            await _showLinkRepository.Delete(existedLink).ConfigureAwait(false);
            return new ServiceResponse(true, string.Empty);
        }

        public async Task<ServiceResponse> UpdateLink(UpdateLinkRequestDto request)
        {
            var loggedUser = _claimManager.GetUserId();
            var existedLink = await _showLinkRepository.GetAll().FirstOrDefaultAsync(f => f.UserId == loggedUser && f.Id == request.Id && !f.IsDeleted).ConfigureAwait(false);
            if (existedLink == null)
            {
                return new ServiceResponse(false, "Not Found");
            }

            existedLink.Name = request.Name;
            existedLink.Link = request.Link;
            await _showLinkRepository.Update(existedLink).ConfigureAwait(false);
            return new ServiceResponse(true, string.Empty);
        }

        public async Task<ServiceResponse> AddShowUser(AddShowUserDto request)
        {
            if (request.ShowId == Guid.Empty)
            {
                return new ServiceResponse(false, "Not Found");
            }

            var loggedUserId = _claimManager.GetUserId();
            var exist = await _showUserRepository.GetAll().Where(f => f.UserId == loggedUserId &&
                                                                      f.ShowId == request.ShowId &&
                                                                      !f.IsDeleted).AnyAsync().ConfigureAwait(false);
            if (exist)
            {
                return new ServiceResponse(false, "Already Added");
            }

            var show = await _showRepository.GetById(request.ShowId).ConfigureAwait(false);
            if (show == null)
            {
                return new ServiceResponse(false, "Not Found");
            }

            var entity = _mapper.Map<Domain.ShowUser>(request);
            entity.CurrentEpisode = 1;
            entity.UpdateDate = DateTime.UtcNow;
            if (show.TotalEpisode == 1)
            {
                entity.Status = Common.Enum.WatchStatus.Finished;
            }
            else
            {
                entity.Status = Common.Enum.WatchStatus.OnGoing;
            }
            entity.UserId = loggedUserId;
            await _showUserRepository.Create(entity).ConfigureAwait(false);
            return new ServiceResponse(true, string.Empty);
        }

        public async Task<ServiceResponse> DeleteShowUser(Guid showId)
        {
            try
            {
                var loggedUserId = _claimManager.GetUserId();
                var existedUserShow = await _showUserRepository.GetAll().Where(f => f.UserId == loggedUserId &&
                                                                          f.ShowId == showId &&
                                                                          !f.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                if (existedUserShow == null)
                {
                    return new ServiceResponse(false, "Not Found");
                }

                existedUserShow.UpdateDate = DateTime.UtcNow;
                existedUserShow.Status = Common.Enum.WatchStatus.Dropped;
                await _showUserRepository.Update(existedUserShow).ConfigureAwait(false);
                return new ServiceResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, ex.Message);
            }
        }

        public async Task<ServiceResponse<PagedResponseDto<ShowUserListDto>>> GetShowUsersByUserIdPaged(ShowUserPagedRequestDto request)
        {
            var loggedUserId = _claimManager.GetUserId();
            var query = await _showUserRepository.GetAll()
                .Include(i => i.User)
                .Include(i => i.Show).ThenInclude(i => i.Links)
                .Include(i => i.Show).ThenInclude(i => i.Genres).ThenInclude(i => i.Genre)
                .Where(f => !f.IsDeleted &&
                            f.Show.ApproveStatus &&
                            f.UserId == loggedUserId &&
                            (!string.IsNullOrEmpty(request.Search) ? EF.Functions.Like(EF.Functions.Collate(f.Show.Name, "SQL_Latin1_General_CP1_CI_AS"), $"%{request.Search}%") : true) &&
                            (request.Categories.Any() ? request.Categories.Contains(f.Show.Category) : true) &&
                            (request.Statuses.Any() ? request.Statuses.Contains(f.Show.Status) : true) &&
                            (request.TrackStatuses.Any() ? request.TrackStatuses.Contains(f.Status) : true) &&
                            (request.Genres.Any() ? request.Genres.Any(y => f.Show.Genres.Any(k => k.GenreId == y && !k.IsDeleted)) : true) &&
                            (request.StartRating.HasValue ? f.Show.Rating >= request.StartRating : true) &&
                            (request.EndRating.HasValue ? f.Show.Rating <= request.EndRating : true))
                .OrderBy(f => f.Status).ThenByDescending(f => f.UpdateDate)
                .Select(f => new ShowUserListDto
                {
                    Id = f.Id,
                    Name = f.Show.Name,
                    TotalEpisode = f.Show.TotalEpisode,
                    Category = f.Show.Category,
                    Status = f.Show.Status,
                    CoverImageUrl = f.Show.CoverImageUrl,
                    Rating = f.Show.Rating,
                    ReleaseGap = f.Show.ReleaseGap,
                    ReleaseType = f.Show.ReleaseType,
                    CurrentEpisode = f.CurrentEpisode,
                    LastUpdateDate = f.UpdateDate,
                    TrackStatus = f.Status,
                    ShowId = f.ShowId,
                    Genres = f.Show.Genres.Where(f => !f.IsDeleted).Select(f => new ShowUserListGenreDto { Id = f.GenreId, Name = f.Genre.Name }).ToList()
                }).ToPagedListAsync(request.PageSize, request.PageIndex).ConfigureAwait(false);

            var showIds = query.Data.Select(x => x.ShowId);
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
                show.Links = links.Where(f => f.ShowId == show.ShowId).Select(f => new ShowListLinkDto { Name = f.Name, Link = f.Link, IsDefault = f.IsDefaultLink, Id = f.Id });
            }

            return new ServiceResponse<PagedResponseDto<ShowUserListDto>>(query, true, string.Empty);
        }

        public async Task<ServiceResponse> UpdateUserShow(UserUpdateShowDto request)
        {
            var userId = _claimManager.GetUserId();
            var userShow = await _showUserRepository.GetAll()
                .Include(i => i.Show)
                .FirstOrDefaultAsync(f => f.Id == request.Id && f.UserId == userId && !f.IsDeleted).ConfigureAwait(false);
            if (userShow == null)
            {
                return new ServiceResponse(false, "You are not following this show");
            }

            if (request.CurrentEpisode <= 0)
            {
                return new ServiceResponse(false, "Episode count is not correct");
            }

            if (userShow.Show.TotalEpisode > 0 && request.CurrentEpisode > userShow.Show.TotalEpisode)
            {
                return new ServiceResponse(false, "Episode count is not correct");
            }

            if (userShow.Show.TotalEpisode == request.CurrentEpisode)
            {
                userShow.Status = Common.Enum.WatchStatus.Finished;
            }
            else
            {
                userShow.Status = Common.Enum.WatchStatus.OnGoing;
            }

            userShow.CurrentEpisode = request.CurrentEpisode;
            userShow.UpdateDate = DateTime.UtcNow;
            await _showUserRepository.Update(userShow).ConfigureAwait(false);
            return new ServiceResponse(true, string.Empty);
        }

        public async Task<ServiceResponse> DropShowUser(DropShowUserDto request)
        {
            var userId = _claimManager.GetUserId();
            var userShow = await _showUserRepository.GetAll()
                .Include(i => i.Show)
                .FirstOrDefaultAsync(f => f.ShowId == request.ShowId && f.UserId == userId && !f.IsDeleted).ConfigureAwait(false);
            if (userShow == null)
            {
                return new ServiceResponse(false, "You are not following this show");
            }

            if (userShow.Show.Name.Contains("Gintama"))
            {
                return new ServiceResponse(false, "Are you insane? How can someone drop Gintama. I think you misclicked. So I will help you and not gonna unfollow.");
            }

            userShow.Status = Common.Enum.WatchStatus.Dropped;
            userShow.UpdateDate = DateTime.UtcNow;
            await _showUserRepository.Delete(userShow).ConfigureAwait(false);
            return new ServiceResponse(true, string.Empty);
        }
    }
}