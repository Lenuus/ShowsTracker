using ShowsTracker.Application.Service.Genre.Dtos;
using ShowsTracker.Common.Dtos;
using ShowsTracker.Common.Helpers;

namespace ShowsTracker.Application.Service.Genre
{
    public class GenreService : IGenreService
    {
        private readonly IRepository<Domain.Genre> _genreRepository;

        public GenreService(IRepository<Domain.Genre> genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<ServiceResponse<PagedResponseDto<GetAllGenresResponseDto>>> GetAllGenres(GetAllGenresRequestDto request)
        {
            var query = await _genreRepository.GetAll()
                                                        .Where(f => !f.IsDeleted && 
                                                                    (!string.IsNullOrEmpty(request.Search) ? f.Name.Contains(request.Search) : true))
                                                        .Select(f => new GetAllGenresResponseDto
                                                        {
                                                            Id = f.Id,
                                                            Name = f.Name,
                                                        })
                                                        .OrderBy(f => f.Name)
                                                        .ToPagedListAsync(request.PageSize, request.PageIndex);

            return new ServiceResponse<PagedResponseDto<GetAllGenresResponseDto>>(query, true, string.Empty);
        }
    }
}
