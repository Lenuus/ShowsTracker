using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowsTracker.Application.Service.Voting;
using ShowsTracker.Application.Service.Voting.Dtos;

namespace ShowsTracker.WebApi.Controllers
{
    [ApiController]
    [Route("votings")]
    public class VotingController : Controller
    {
        private readonly IVotingService _votingService;

        public VotingController(
            IVotingService votingService)
        {
            _votingService = votingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentVotingSeason()
        {
            var response = await _votingService.GetCurrentVoting().ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("vote-for-show")]
        [Authorize]
        public async Task<IActionResult> VoteForShow([FromBody] VoteForShowRequestDto request)
        {
            var response = await _votingService.VoteForShow(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllVotingSeasons([FromBody] GetAllVotingSeasonsRequestDto request)
        {
            var response = await _votingService.GetAllVotingSeasons(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create-voting-season")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVotingSeason([FromBody] CreateVotingSeasonRequestDto request)
        {
            var response = await _votingService.CreateVotingSeason(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("delete-voting-season/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVotingSeason(Guid id)
        {
            var response = await _votingService.DeleteVotingSeason(id).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
