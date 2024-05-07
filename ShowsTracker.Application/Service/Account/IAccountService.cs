using ShowsTracker.Application.Service.Account.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Account
{
    public interface IAccountService
    {
        Task<ServiceResponse<LoginResponseDto>> Login(LoginRequestDto request);

        Task<ServiceResponse> Register(RegisterRequestDto request);

        Task<ServiceResponse<LoginResponseDto>> ExternalLogin(ExternalLoginRequestDto request);

        Task<ServiceResponse> ForgotPassword(ForgotPasswordRequestDto request);

        Task<ServiceResponse> CheckForgotPasswordCode(CheckPasswordRequestDto request);

        Task<ServiceResponse> ChangePassword(ChangePasswordRequestDto request);

        Task<ServiceResponse> ConfirmEmail(ConfirmMailRequestDto request);
    }
}
