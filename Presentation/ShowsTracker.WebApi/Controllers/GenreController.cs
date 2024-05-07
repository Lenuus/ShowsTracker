using Microsoft.AspNetCore.Mvc;
using ShowsTracker.Application.Service.Genre;
using ShowsTracker.Application.Service.Genre.Dtos;
using ShowsTracker.Application.Service.Show.Dtos;

namespace ShowsTracker.WebApi.Controllers
{
    [ApiController]
    [Route("genres")]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpPost("get-genres")]
        public async Task<IActionResult> GetAllGenres([FromBody] GetAllGenresRequestDto request)
        {
            var response = await _genreService.GetAllGenres(request).ConfigureAwait(false);
            if (!response.IsSuccesfull)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
