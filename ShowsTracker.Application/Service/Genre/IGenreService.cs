using ShowsTracker.Application.Service.Genre.Dtos;
using ShowsTracker.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Genre
{
    public interface IGenreService
    {
        Task<ServiceResponse<PagedResponseDto<GetAllGenresResponseDto>>> GetAllGenres(GetAllGenresRequestDto request);
    }
}
