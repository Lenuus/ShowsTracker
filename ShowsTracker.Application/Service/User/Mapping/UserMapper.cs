using AutoMapper;
using ShowsTracker.Application.Service.Show.Dtos;
using ShowsTracker.Application.Service.User.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.User.Mapping
{
    public class UserMapper : Profile
    {
        public UserMapper() 
        {
            CreateMap<AddUserDto,Domain.User>();
            CreateMap<Domain.User,UserDetailDto>();
        }
    }
}
