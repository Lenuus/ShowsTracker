using ShowsTracker.Application.Service.Voting.Dtos;
using ShowsTracker.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Voting
{
    public interface IVotingService : IApplicationService
    {
        Task<ServiceResponse> CreateVotingSeason(CreateVotingSeasonRequestDto request);

        Task<ServiceResponse> DeleteVotingSeason(Guid id);

        Task<ServiceResponse<List<VoteSeasonDetailDto>>> GetCurrentVoting();

        Task<ServiceResponse<PagedResponseDto<VoteSeasonDetailDto>>> GetAllVotingSeasons(GetAllVotingSeasonsRequestDto request);

        Task<ServiceResponse> VoteForShow(VoteForShowRequestDto request);
    }
}
