using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ShowsTracker.Application.Service.Account.Dtos;
using ShowsTracker.Application.Service.Auth;
using ShowsTracker.Application.Service.Email;
using ShowsTracker.Common.Constants;
using ShowsTracker.Common.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using System.Security.Policy;

namespace ShowsTracker.Application.Service.Account
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<Domain.User> _userRepository;
        private readonly IAuthService _authService;
        private readonly PasswordHelper _passwordHelper;
        private readonly IRepository<Domain.Role> _roleRepository;
        private readonly SignInManager<Domain.User> _signInManager;
        private readonly UserManager<Domain.User> _userManager;
        private readonly IEmailService _emailService;

        public AccountService(
            IRepository<Domain.User> userRepository,
            IAuthService authService,
            PasswordHelper passwordHelper,
            IRepository<Domain.Role> roleRepository,
            SignInManager<Domain.User> signInManager,
            UserManager<Domain.User> userManager,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _authService = authService;
            _passwordHelper = passwordHelper;
            _roleRepository = roleRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<LoginResponseDto>> ExternalLogin(ExternalLoginRequestDto request)
        {
            var normalizedEmail = request.Email.ToNormalize();
            var userExists = await _userRepository.GetAll().FirstOrDefaultAsync(f => f.NormalizedEmail == normalizedEmail && !f.IsDeleted).ConfigureAwait(false);
            if (userExists != null)
            {
                await _signInManager.SignInAsync(userExists, true);
                return await CreateTokenForUser(userExists).ConfigureAwait(false);
            }

            var user = await _userManager.CreateAsync(new Domain.User { Email = request.Email, UserName = request.Email }).ConfigureAwait(false);
            if (user.Succeeded)
            {
                var createdUser = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
                await _userManager.AddToRoleAsync(createdUser, RoleConstants.User).ConfigureAwait(false);
                await _signInManager.SignInAsync(createdUser, true);
                return await CreateTokenForUser(createdUser).ConfigureAwait(false);
            }

            return new ServiceResponse<LoginResponseDto>(null, false, "Something wrong");
        }

        public async Task<ServiceResponse> ForgotPassword(ForgotPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
            if (user == null)
            {
                return new ServiceResponse(false, "User not found");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            string body = $"/auth/change-password?userId={user.Id}&code={Uri.EscapeDataString(code)}";
            var result = await _emailService.Send(new Email.Dtos.SendRequestDto { Body = body, Subject = "Change Password", To = new List<string> { request.Email } }).ConfigureAwait(false);
            if (result.IsSuccesfull)
            {
                return new ServiceResponse(true, string.Empty);
            }

            return new ServiceResponse(result.IsSuccesfull, result.ErrorMessage);
        }

        public async Task<ServiceResponse> CheckForgotPasswordCode(CheckPasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                return new ServiceResponse(false, "User not found");
            }

            var purpose = UserManager<Domain.User>.ResetPasswordTokenPurpose;
            var isValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, purpose, request.Code);
            return new ServiceResponse(isValid, isValid ? string.Empty : "Code is incorrect");
        }

        public async Task<ServiceResponse> ChangePassword(ChangePasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                return new ServiceResponse(false, "User not found");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);
            if (result.Succeeded)
            {
                return new ServiceResponse(true, string.Empty);
            }

            return new ServiceResponse(false, string.Join("\n", result.Errors.Select(f => f.Description)));
        }

        public async Task<ServiceResponse<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var user = await _userRepository.GetAll()
                .Include(f => f.Roles).ThenInclude(f => f.Role)
                .FirstOrDefaultAsync(f => f.Email == request.Email).ConfigureAwait(false);
            if (user == null)
            {
                return new ServiceResponse<LoginResponseDto>(null, false, "User not found");
            }

            if (user.IsDeleted)
            {
                return new ServiceResponse<LoginResponseDto>(null, false, "User not found");
            }

            if (!user.EmailConfirmed)
            {
                return new ServiceResponse<LoginResponseDto>(null, false, "Activate your email");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, true, true).ConfigureAwait(false);
            if (!signInResult.Succeeded)
            {
                return new ServiceResponse<LoginResponseDto>(null, false, "Email or password is wrong");
            }

            return await CreateTokenForUser(user).ConfigureAwait(false);
        }

        public async Task<ServiceResponse> Register(RegisterRequestDto request)
        {
            var normalizedEmail = request.Email.ToNormalize();
            var userExists = await _userRepository.GetAll().FirstOrDefaultAsync(f => f.NormalizedEmail == normalizedEmail && !f.IsDeleted).ConfigureAwait(false);
            if (userExists != null)
            {
                return new ServiceResponse(false, "User already exists");
            }

            var user = await _userManager.CreateAsync(new Domain.User { Email = request.Email, UserName = request.Email }, request.Password).ConfigureAwait(false);
            if (user.Succeeded)
            {
                var createdUser = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(createdUser).ConfigureAwait(false);
                var filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                filePath = string.Concat(filePath, "\\", "mail-templates", "\\", "activate-account.html");
                var html = File.ReadAllText(filePath);
                html = html.Replace("\r\n", string.Empty);
                html = html.Replace("[ActivationLink]", $"auth/confirm-email?userId={createdUser.Id}&code={Uri.EscapeDataString(code)}");
                await _emailService.Send(new Email.Dtos.SendRequestDto { To = new List<string> { request.Email }, Body = html, Subject = "Activate your account" }).ConfigureAwait(false);
                await _userManager.AddToRoleAsync(createdUser, RoleConstants.User).ConfigureAwait(false);
                return new ServiceResponse(true, string.Empty);
            }

            return new ServiceResponse(false, string.Join("\n", user.Errors.Select(f => f.Description)));
        }

        public async Task<ServiceResponse> ConfirmEmail(ConfirmMailRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                return new ServiceResponse(false, "User not found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Code).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return new ServiceResponse(true, string.Empty);
            }

            return new ServiceResponse(false, string.Join("\n", result.Errors.Select(f => f.Description)));
        }

        private async Task<ServiceResponse<LoginResponseDto>> CreateTokenForUser(Domain.User user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtTokenConstants.UserId, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.Email.ToString()));
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            claims.Add(new Claim(ClaimTypes.Role, string.Join(",", roles)));
            var tokenInfo = _authService.GenerateToken(claims);
            if (!tokenInfo.IsSuccesfull)
            {
                return new ServiceResponse<LoginResponseDto>(null, false, "Token cannot be created");
            }

            var loginInfo = new LoginResponseDto()
            {
                Expires = tokenInfo.Data.Expires,
                Token = tokenInfo.Data.Token,
            };
            return new ServiceResponse<LoginResponseDto>(loginInfo, true, string.Empty);
        }

        private bool EmailIsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
