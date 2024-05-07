using ShowsTracker.Application.Service.ShowUser.Dtos;
using ShowsTracker.Application.Service.User.Dtos;
using ShowsTracker.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.ShowUser
{
    public interface IShowUserService
    {
        Task<ServiceResponse> AddShowUser(AddShowUserDto request);

        Task<ServiceResponse> DeleteShowUser(Guid id);

        Task<ServiceResponse<PagedResponseDto<ShowUserListDto>>> GetShowUsersByUserIdPaged(ShowUserPagedRequestDto request);

        Task<ServiceResponse> UpdateUserShow(UserUpdateShowDto request);

        Task<ServiceResponse<Guid>> AddLinkToShow(AddLinkToShowRequestDto request);

        Task<ServiceResponse> UpdateLink(UpdateLinkRequestDto request);
        
        Task<ServiceResponse> DeleteLink(Guid id);

        Task<ServiceResponse> DropShowUser(DropShowUserDto request);
    }
}
