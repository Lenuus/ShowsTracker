using ShowsTracker.Application.Service.Show.Dtos;
using ShowsTracker.Common.Dtos;
using ShowsTracker.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Show
{
    public interface IShowService
    {
        /// <summary>
        /// Delete Show
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceResponse> DeleteShow(Guid id);

        /// <summary>
        /// Show All Shows
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ServiceResponse<PagedResponseDto<ShowListDto>>> GetAllShows(GetAllShowsDto request);

        /// <summary>
        /// Add Show
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ServiceResponse> AddShow(AddShowDto request);

        /// <summary>
        /// Change Valid Values
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ServiceResponse> UpdateShow(UpdateShowDto request);

        /// <summary>
        /// GetUnapprovedShows
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ServiceResponse<PagedResponseDto<UnapprovedShowListDto>>> GetUnapprovedShows(GetAllShowsDto request);

        /// <summary>
        /// GetThisSeasonShows
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<ShowListDto>>> GetOnGoingShows();

        /// <summary>
        /// GetRandomShows
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<ShowListDto>>> GetRandomShows(Category category);

        /// <summary>
        /// FindYourTaste
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<ShowListDto>>> FindYourTaste(FindYourTasteRequestDto request);
    }
}
