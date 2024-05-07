using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowsTracker.Application.Service.Show;
using ShowsTracker.Application.Service.Show.Dtos;
using ShowsTracker.Common.Constants;
using ShowsTracker.Common.Enum;

namespace ShowsTracker.WebApi.Controllers
{
    [ApiController]
    [Route("show")]
    public class ShowController : Controller
    {
        private readonly IShowService _showService;

        public ShowController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpPost("get-shows")]
        public async Task<IActionResult> GetShows([FromBody] GetAllShowsDto request)
        {
            var response = await _showService.GetAllShows(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("get-ongoing-shows")]
        public async Task<IActionResult> GetOnGoingShows()
        {
            var response = await _showService.GetOnGoingShows().ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("get-random-shows")]
        public async Task<IActionResult> GetRandomShows(Category category)
        {
            var response = await _showService.GetRandomShows(category).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("find-your-taste")]
        public async Task<IActionResult> FindYourTaste([FromBody] FindYourTasteRequestDto request)
        {
            var response = await _showService.FindYourTaste(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create-show")]
        [Authorize]
        public async Task<IActionResult> CreateShow([FromBody] AddShowDto request)
        {
            var response = await _showService.AddShow(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("get-unapproved-shows")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUnapprovedShows([FromBody] GetAllShowsDto request)
        {
            var response = await _showService.GetUnapprovedShows(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
