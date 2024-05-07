using AutoMapper;
using ShowsTracker.Application.Service.ShowUser.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.ShowUser.Mapping
{
    public class ShowUserMapper:Profile
    {
        public ShowUserMapper() 
        {
            CreateMap<AddShowUserDto, Domain.ShowUser>();
         //   CreateMap<Domain.ShowUser, ShowUserDetailDto>();
        }
    }
}
