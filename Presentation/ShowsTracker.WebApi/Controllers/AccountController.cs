using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowsTracker.Application.Service.Account;
using ShowsTracker.Application.Service.Account.Dtos;
using ShowsTracker.Common.Helpers;

namespace ShowsTracker.WebApi.Controllers
{
	[ApiController]
	[Route("account")]
	public class AccountController : Controller
	{
		private readonly IAccountService _accountService;
		private readonly IClaimManager _claimManager;

		public AccountController(
			IAccountService accountService,
			IClaimManager claimManager)
		{
			_accountService = accountService;
			_claimManager = claimManager;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
		{
			var response = await _accountService.Login(request).ConfigureAwait(false);
			if (!response.IsSuccesfull)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
		{
			var response = await _accountService.Register(request).ConfigureAwait(false);
			if (!response.IsSuccesfull)
			{
				return BadRequest(response);
			}

			return Ok(response);
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginRequestDto request)
        {
            var response = await _accountService.ExternalLogin(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var response = await _accountService.ForgotPassword(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("check-forgot-password-code")]
        public async Task<IActionResult> CheckForgotPasswordCode([FromBody] CheckPasswordRequestDto request)
        {
            var response = await _accountService.CheckForgotPasswordCode(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var response = await _accountService.ChangePassword(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmMailRequestDto request)
        {
            var response = await _accountService.ConfirmEmail(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("get-claims")]
		[Authorize]
		public IActionResult GetClaims()
		{
			var email = _claimManager.GetEmail();
			var role = _claimManager.GetRole();
			var id = _claimManager.GetUserId();
			return Ok(new
			{
				Data = new
				{
					Id = id,
					Email = email,
					Role = role,
				}
			});
		}
	}
}
