using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowsTracker.Application.Service.ShowUser;
using ShowsTracker.Application.Service.ShowUser.Dtos;

namespace ShowsTracker.WebApi.Controllers
{
	[ApiController]
	[Route("user-show")]
	[Authorize]
	public class UserShowController : Controller
	{
		private readonly IShowUserService _showUserService;

		public UserShowController(IShowUserService showUserService)
		{
			_showUserService = showUserService;
		}

		[HttpPost("add-show")]
		public async Task<IActionResult> AddShow([FromBody] AddShowUserDto request)
		{
			var response = await _showUserService.AddShowUser(request).ConfigureAwait(false);
			if (!response.IsSuccesfull)
			{
				return BadRequest(response);
			}

			return Ok(response);
        }

        [HttpPost("update-show")]
        public async Task<IActionResult> UpdateShow([FromBody] UserUpdateShowDto request)
        {
            var response = await _showUserService.UpdateUserShow(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("drop-show")]
        public async Task<IActionResult> DropShow([FromBody] DropShowUserDto request)
        {
            var response = await _showUserService.DropShowUser(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("get-my-shows")]
		public async Task<IActionResult> GetMyShows([FromBody] ShowUserPagedRequestDto request)
		{
			var response = await _showUserService.GetShowUsersByUserIdPaged(request).ConfigureAwait(false);
			if (!response.IsSuccesfull)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}

		[HttpPost("add-link-to-show")]
		public async Task<IActionResult> AddLinkToShow([FromBody] AddLinkToShowRequestDto request)
		{
			var response = await _showUserService.AddLinkToShow(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update-link")]
        public async Task<IActionResult> UpdateLink([FromBody] UpdateLinkRequestDto request)
        {
            var response = await _showUserService.UpdateLink(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("delete-link")]
        public async Task<IActionResult> DeleteLink(Guid id)
        {
            var response = await _showUserService.DeleteLink(id).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
