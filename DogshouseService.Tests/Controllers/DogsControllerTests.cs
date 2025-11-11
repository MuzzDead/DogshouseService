using DogshouseService.BLL.DTOs;
using DogshouseService.BLL.Interfaces;
using DogshouseService.WebAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DogshouseService.Tests.Controllers;

public class DogsControllerTests
{
    private readonly Mock<IDogService> _serviceMock;
    private readonly DogsController _controller;

    public DogsControllerTests()
    {
        _serviceMock = new Mock<IDogService>();
        _controller = new DogsController(_serviceMock.Object);
    }

    #region Ping Tests

    [Fact]
    public void Ping_ShouldReturn_CorrectContentResult()
    {
        // Act
        var result = _controller.Ping();

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.NotNull(result);
        Assert.Equal("Dogshouseservice.Version1.0.1", contentResult.Content);
        Assert.Equal("text/plain", contentResult.ContentType);
    }

    #endregion


    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithValidParams_ShouldReturnOkWithItemsAndTotalCountHeader()
    {
        // Arrange
        var dogsList = new List<DogDto>
        {
            new DogDto ( "Neo", "Red", 22, 32 )
        };
        (IEnumerable<DogDto> Items, int Total) serviceResult = (dogsList, 1);

        int pageNumber = 1, pageSize = 10;

        _serviceMock.Setup(s => s.GetListAsync(null, null, pageNumber, pageSize))
            .ReturnsAsync(serviceResult);

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.GetAsync(null, null, pageNumber, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dogsList, okResult.Value);
        var header = _controller.Response.Headers["X-Total-Count"];
        Assert.Equal("1", header);

        _serviceMock.Verify(s => s.GetListAsync(null, null, pageNumber, pageSize), Times.Once);
    }

    [Theory]
    [InlineData(0, 10)]  // pageNumber < 1
    [InlineData(1, 0)]   // pageSize < 1
    [InlineData(1, 101)] // pageSize > 100
    public async Task GetAsync_WithInvalidPagination_ShouldReturnBadRequest(int page, int size)
    {
        // Act
        var result = await _controller.GetAsync(null, null, page, size);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Page number and page size must be greater than 0.", badRequestResult.Value);

        _serviceMock.Verify(s => s.GetListAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    #endregion


    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var request = new CreateDogRequest { Name = "Neo", Color = "Red", TailLength = 22, Weight = 32 };
        var createdDto = new DogDto("Neo", "Red", 22, 32);

        _serviceMock.Setup(s => s.CreateAsync(request))
                    .ReturnsAsync(createdDto);

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(createdDto, createdAtResult.Value);

        Assert.Equal(nameof(DogsController.GetAsync), createdAtResult.ActionName);
    }

    [Fact]
    public async Task CreateAsync_WhenServiceThrowsArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateDogRequest { Name = "" };
        var exceptionMessage = "Name cannot be null or empty";

        _serviceMock.Setup(s => s.CreateAsync(request))
                    .ThrowsAsync(new ArgumentException(exceptionMessage));

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task CreateAsync_WhenServiceThrowsInvalidOperationException_ShouldReturnConflict()
    {
        // Arrange
        var request = new CreateDogRequest { Name = "Neo" };
        var exceptionMessage = "Dog with name 'Neo' already exists.";

        _serviceMock.Setup(s => s.CreateAsync(request))
                    .ThrowsAsync(new InvalidOperationException(exceptionMessage));

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.NotNull(conflictResult.Value);
    }

    #endregion
}
