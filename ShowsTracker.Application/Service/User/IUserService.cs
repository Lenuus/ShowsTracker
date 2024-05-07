using ShowsTracker.Application.Service.Show.Dtos;
using ShowsTracker.Application.Service.User.Dtos;
using ShowsTracker.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.User
{
    public interface IUserService
    {
        Task<ServiceResponse> DeleteUser(Guid id);

        Task<ServiceResponse<PagedResponseDto<UserListDto>>> GetAllUser(GetAllUsersDto request);

        Task<ServiceResponse<UserDetailDto>> GetUserById(Guid id);

        Task<ServiceResponse> AddUser(AddUserDto request);

        Task<ServiceResponse> UpdateUser(UpdateUserDto request);

    }
}
