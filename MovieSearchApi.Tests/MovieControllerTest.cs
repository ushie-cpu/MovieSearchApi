using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Services;
using Xunit;

namespace MovieSearchApi.Controllers
{
    public class MovieControllerTest
    {
        [Fact]
        public async Task SearchMovieById_ReturnsOkObjectResult()
        {
            // Arrange
            var omdbServiceMock = new Mock<OmdbService>(MockBehavior.Strict, new HttpClient());
            omdbServiceMock.Setup(s => s.SearchMovieByIdAsync(It.IsAny<string>()))
                           .ReturnsAsync(new Movie { Title = "Sample Movie" });

            var memoryCacheMock = new Mock<IMemoryCache>();
            var controller = new MovieController(omdbServiceMock.Object, memoryCacheMock.Object);

            // Act
            var result = await controller.SearchMovie("tt123456", null, null, null);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okObjectResult = (OkObjectResult)result.Result;
            Assert.IsType<Movie>(okObjectResult.Value);
            var movie = (Movie)okObjectResult.Value;
            Assert.Equal("Sample Movie", movie.Title);
        }

        [Fact]
        public async Task SearchMovieByTitle_ReturnsOkObjectResultAndSavesSearchQuery()
        {
            // Arrange
            var omdbServiceMock = new Mock<OmdbService>(MockBehavior.Strict, new HttpClient());
            omdbServiceMock.Setup(s => s.SearchMovieByTitleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync(new Movie { Title = "Sample Movie" });

            var memoryCacheMock = new Mock<IMemoryCache>();
            memoryCacheMock.Setup(c => c.Get<List<SearchQuery>>("LatestSearchQueries"))
                           .Returns(new List<SearchQuery>());
            memoryCacheMock.Setup(c => c.Set("LatestSearchQueries", It.IsAny<List<SearchQuery>>(), It.IsAny<MemoryCacheEntryOptions>()));

            var controller = new MovieController(omdbServiceMock.Object, memoryCacheMock.Object);

            // Act
            var result = await controller.SearchMovie(null, "Sample Movie", null, null);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okObjectResult = (OkObjectResult)result.Result;
            Assert.IsType<Movie>(okObjectResult.Value);
            var movie = (Movie)okObjectResult.Value;
            Assert.Equal("Sample Movie", movie.Title);

            memoryCacheMock.Verify(c => c.Set("LatestSearchQueries", It.IsAny<List<SearchQuery>>(), It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
        }
    }
}
