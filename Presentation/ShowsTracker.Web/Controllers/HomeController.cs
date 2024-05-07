using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShowsTracker.Application;
using ShowsTracker.Web.Constants;
using ShowsTracker.Web.Models;
using ShowsTracker.Web.Models.Show;
using System.Diagnostics;
using System.Text.Json;

namespace ShowsTracker.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(GetAllShowsRequestModel model)
        {
            ServiceResponse<PagedResponseModel<GetAllShowsResponseModel>> responseMapped = await PostAsync<ServiceResponse<PagedResponseModel<GetAllShowsResponseModel>>>(RouteConstants.GetShows, model).ConfigureAwait(false);
            if (!responseMapped.IsSuccesfull)
            {
                return NotFound();
            }

            return View(responseMapped.Data);
        }
    }
}
