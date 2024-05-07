using Microsoft.EntityFrameworkCore;
using ShowsTracker.Application.Service.Voting.Dtos;
using ShowsTracker.Common.Dtos;
using ShowsTracker.Common.Helpers;
using ShowsTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Voting
{
    public class VotingService : IVotingService
    {
        private readonly IRepository<Domain.VoteSeason> _voteSeasonRepository;
        private readonly IRepository<Domain.Show> _showRepository;
        private readonly IRepository<Domain.UserVote> _userVoteRepository;
        private readonly IClaimManager _claimManager;

        public VotingService(
            IRepository<VoteSeason> voteSeasonRepository,
            IRepository<Domain.Show> showRepository,
            IRepository<UserVote> userVoteRepository,
            IClaimManager claimManager)
        {
            _voteSeasonRepository = voteSeasonRepository;
            _showRepository = showRepository;
            _userVoteRepository = userVoteRepository;
            _claimManager = claimManager;
        }

        public async Task<ServiceResponse> CreateVotingSeason(CreateVotingSeasonRequestDto request)
        {
            var selectedShowIds = request.Shows.Select(f => f.Id);
            var showCount = await _showRepository.GetAll().Where(f => selectedShowIds.Any(x => x == f.Id)).CountAsync();
            if (showCount != selectedShowIds.Count())
            {
                return new ServiceResponse(false, "Not Found");
            }

            await _voteSeasonRepository.Create(new VoteSeason
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Name = request.Name,
                Shows = request.Shows.Select(f => new VoteShow
                {
                    ShowId = f.Id,
                    DisplayOrder = f.DisplayOrder
                }).ToList()
            });
            return new ServiceResponse(true, string.Empty);
        }

        public async Task<ServiceResponse> DeleteVotingSeason(Guid id)
        {
            var existedVoting = await _voteSeasonRepository.GetById(id).ConfigureAwait(false);
            if (existedVoting == null)
            {
                return new ServiceResponse(false, "Not Found");
            }

            await _voteSeasonRepository.Delete(existedVoting).ConfigureAwait(false);
            return new ServiceResponse(true, string.Empty);
        }

        public async Task<ServiceResponse<PagedResponseDto<VoteSeasonDetailDto>>> GetAllVotingSeasons(GetAllVotingSeasonsRequestDto request)
        {
            var query = await _voteSeasonRepository.GetAll()
                .Include(i => i.Shows).ThenInclude(i => i.Show)
                .Include(i => i.UserVotes)
                .Where(f => !f.IsDeleted &&
                            (request.StartDate.HasValue ? request.StartDate <= f.StartDate : true) &&
                            (request.EndDate.HasValue ? request.EndDate >= f.EndDate : true) &&
                            (request.IsFinished == null || f.IsFinished == request.IsFinished))
                .Select(f => new VoteSeasonDetailDto
                {
                    Id = f.Id,
                    EndDate = f.EndDate,
                    IsFinished = f.IsFinished,
                    Name = f.Name,
                    Shows = f.Shows.Where(s => !s.IsDeleted).Select(s => new VoteSeasonDetailShowDto
                    {
                        Id = s.Id,
                        DisplayOrder = s.DisplayOrder,
                        IsWinner = s.IsWinner,
                        Name = s.Show.Name,
                        TotalVote = f.UserVotes.Count(x => x.ShowId == s.ShowId && !x.IsDeleted),
                        CoverImageUrl = s.Show.CoverImageUrl
                    }).ToList(),
                    StartDate = f.StartDate,
                    TotalVote = f.UserVotes.Count(x => !x.IsDeleted)
                }).ToPagedListAsync(request.PageSize, request.PageIndex).ConfigureAwait(false);

            return new ServiceResponse<PagedResponseDto<VoteSeasonDetailDto>>(query, true, string.Empty);
        }

        public async Task<ServiceResponse<List<VoteSeasonDetailDto>>> GetCurrentVoting()
        {
            var now = DateTime.Now;
            var loggedUserId = _claimManager.GetUserId();
            var votingSeasons = await _voteSeasonRepository.GetAll()
                .Include(i => i.Shows).ThenInclude(i => i.Show)
                .Include(i => i.UserVotes)
                .Where(f => !f.IsDeleted)
                .OrderByDescending(o => o.EndDate)
                .Select(f => new VoteSeasonDetailDto
                {
                    Id = f.Id,
                    EndDate = f.EndDate,
                    IsFinished = f.IsFinished,
                    Name = f.Name,
                    Shows = f.Shows.Where(s => !s.IsDeleted).Select(s => new VoteSeasonDetailShowDto
                    {
                        Id = s.Id,
                        ShowId = s.ShowId,
                        DisplayOrder = s.DisplayOrder,
                        IsWinner = s.IsWinner,
                        Name = s.Show.Name,
                        TotalVote = f.IsFinished ? f.UserVotes.Count(x => x.ShowId == s.ShowId && !x.IsDeleted) : 0,
                        CoverImageUrl = s.Show.CoverImageUrl,
                        SelectedByCurrentUser = loggedUserId != Guid.Empty ? f.UserVotes.Any(x => x.UserId == loggedUserId && x.ShowId == s.ShowId && !x.IsDeleted) : false
                    }).OrderByDescending(s => f.IsFinished ? s.TotalVote : 1).OrderBy(s => f.IsFinished ? 1 : s.DisplayOrder).ToList(),
                    StartDate = f.StartDate,
                    TotalVote = f.UserVotes.Count(x => !x.IsDeleted)
                }).ToListAsync().ConfigureAwait(false);

            return new ServiceResponse<List<VoteSeasonDetailDto>>(votingSeasons, true, string.Empty);
        }

        public async Task<ServiceResponse> VoteForShow(VoteForShowRequestDto request)
        {
            var loggedUserId = _claimManager.GetUserId();
            var votingSeason = await _voteSeasonRepository.GetAll().Include(f => f.Shows).FirstOrDefaultAsync(f => f.Id == request.VotingSeasonId && !f.IsDeleted).ConfigureAwait(false);
            if (votingSeason == null)
            {
                return new ServiceResponse(false, "Not Found");
            }

            if (!votingSeason.Shows.Any(f => f.ShowId == request.ShowId && !f.IsDeleted))
            {
                return new ServiceResponse(false, "Show Not Found");
            }

            var userVote = await _userVoteRepository.GetAll().FirstOrDefaultAsync(f => f.UserId == loggedUserId && f.VoteSeasonId == request.VotingSeasonId && !f.IsDeleted).ConfigureAwait(false);
            if (userVote == null)
            {
                await _userVoteRepository.Create(new UserVote { ShowId = request.ShowId, UserId = loggedUserId, VoteSeasonId = request.VotingSeasonId, VoteDateTime = DateTime.UtcNow }).ConfigureAwait(false);
            }
            else
            {
                userVote.ShowId = request.ShowId;
                await _userVoteRepository.Update(userVote).ConfigureAwait(false);
            }

            return new ServiceResponse(true, string.Empty);
        }
    }
}
