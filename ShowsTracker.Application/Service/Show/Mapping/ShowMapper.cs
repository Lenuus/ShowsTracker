using AutoMapper;
using ShowsTracker.Application.Service.Show.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Show.Mapping
{
    public class ShowMapper : Profile
    {
        public ShowMapper()
        {
            CreateMap<Domain.Show, ShowDetailDto>();
            CreateMap<AddShowDto, Domain.Show>();
        }
    }
}
