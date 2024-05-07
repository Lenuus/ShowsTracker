using Microsoft.AspNetCore.Mvc;
using Moq;
using ShowsTracker.Application;
using ShowsTracker.Application.Service.Show;
using ShowsTracker.Application.Service.Show.Dtos;
using ShowsTracker.Common.Dtos;
using ShowsTracker.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowTracker.WebApi.Tests.Controllers
{
    public class ShowControllerTests
    {
        private ShowController _showController;
        private readonly Mock<IShowService> _showServiceMock;
        public ShowControllerTests()
        {
            _showServiceMock = new Mock<IShowService>();
            _showController = new ShowController(_showServiceMock.Object);
        }

        [Fact]
        public async Task GetShows_RequestNull_ReturnsBadRequest_OnFailure()
        {
            // Arrange
            GetAllShowsDto request = null;
            ServiceResponse<PagedResponseDto<ShowListDto>> response = new ServiceResponse<PagedResponseDto<ShowListDto>>(null, false, "Request cannot be null");
            _showServiceMock.Setup(x => x.GetAllShows(request)).ReturnsAsync(response);

            // Act
            var result = await _showController.GetShows(request);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(badRequestResult!.Value, response);
        }

        [Fact]
        public async Task GetShows_RequestValid_ReturnsSuccessful_OnSuccess()
        {
            // Arrange
            GetAllShowsDto request = new GetAllShowsDto();
            ServiceResponse<PagedResponseDto<ShowListDto>> response = new ServiceResponse<PagedResponseDto<ShowListDto>>(
                new PagedResponseDto<ShowListDto> { CurrentPage = 0, Data = new List<ShowListDto> { new ShowListDto { Id = Guid.NewGuid(), Name = "Frieren" } }, PageSize = 1, TotalCount = 1, TotalPage = 1 },
                true,
                string.Empty);
            _showServiceMock.Setup(x => x.GetAllShows(request)).ReturnsAsync(response);

            // Act
            var result = await _showController.GetShows(request);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(okResult!.Value, response);
        }
    }
}
