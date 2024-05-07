using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShowsTracker.Application.Service.Show.Dtos;
using ShowsTracker.Application.Service.User.Dtos;
using ShowsTracker.Common.Dtos;
using ShowsTracker.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.User
{
    public class UserService : IUserService
    {
        private readonly IRepository<Domain.User> _userRepository;
        private readonly IMapper _mapper;

        public UserService(IRepository<Domain.User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse> AddUser(AddUserDto request)
        {
            if (request == null) return new ServiceResponse(true, "Not Found");
            var check =await  _userRepository.GetAll().Where(f => f.Name == request.Name).AnyAsync().ConfigureAwait(false);
            if (check)
            {
                return new ServiceResponse(false, "Already Taken Password");
            }

            var response = _mapper.Map<Domain.User>(request);
            await _userRepository.Create(response);
            return new ServiceResponse(true, string.Empty);

        }

        public async Task<ServiceResponse> DeleteUser(Guid id)
        {
            try
            {
                await _userRepository.DeleteById(id).ConfigureAwait(false);
                return new ServiceResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, ex.Message);
            }
        }

        public async Task<ServiceResponse<PagedResponseDto<UserListDto>>> GetAllUser(GetAllUsersDto request)
        {
            var check = await _userRepository.GetAll().Where(f => !f.IsDeleted && (!string.IsNullOrEmpty(request.Search) ? (f.Name + " " + f.Surname)
            .Contains(request.Search) : true))
            .Select(f => new UserListDto
            {
                Id = f.Id,
                Surname = f.Surname,
                Name = f.Name,
                Email = f.Email,
            }).ToPagedListAsync(request.PageSize, request.PageIndex).ConfigureAwait(false);

            return new ServiceResponse<PagedResponseDto<UserListDto>>(check, true, string.Empty);
        }

        public async Task<ServiceResponse<UserDetailDto>> GetUserById(Guid id)
        {
            var check = await _userRepository.GetById(id).ConfigureAwait(false);
            if (check == null) { return new ServiceResponse<UserDetailDto>(null, true, "Not Found"); }
            var response = _mapper.Map<UserDetailDto>(check);

            return new ServiceResponse<UserDetailDto>(response, true, string.Empty);
        }

        public async Task<ServiceResponse> UpdateUser(UpdateUserDto request)
        {
            var check = await _userRepository.GetById(request.Id).ConfigureAwait(false);
            if (check == null)
            {
                return new ServiceResponse(true, "Not Found");
            }

            check.Surname = request.Surname;
            check.Email = request.Email;
            check.Name = request.Name;
            await _userRepository.Update(check);
            return new ServiceResponse(true, string.Empty);
        }
    }
}
