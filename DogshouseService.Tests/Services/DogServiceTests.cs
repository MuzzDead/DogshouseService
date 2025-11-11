using Azure;
using DogshouseService.BLL.DTOs;
using DogshouseService.BLL.Services;
using DogshouseService.DAL.Entities;
using DogshouseService.DAL.Interfaces;
using Moq;
using System.Drawing;

namespace DogshouseService.Tests.Services;

public class DogServiceTests
{
    private readonly Mock<IDogsRepository> _repositoryMock;
    private readonly DogService _service;
    public DogServiceTests()
    {
        _repositoryMock = new Mock<IDogsRepository>();
        _service = new DogService(_repositoryMock.Object);
    }


    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateAndReturnDog()
    {
        // Arrange
        var request = new CreateDogRequest
        {
            Name = "Buddy",
            Color = "Brown",
            TailLength = 10,
            Weight = 20
        };

        _repositoryMock.Setup(r => r.ExistsByNameAsync(request.Name))
            .ReturnsAsync(false);

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Dog>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Color, result.Color);

        _repositoryMock.Verify(r => r.ExistsByNameAsync(request.Name), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Dog>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenDogNameExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateDogRequest
        {
            Name = "Buddy",
            Color = "Brown",
            TailLength = 10,
            Weight = 20
        };

        _repositoryMock.Setup(r => r.ExistsByNameAsync(request.Name))
            .ReturnsAsync(true);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateAsync(request));

        // Assert
        Assert.Equal($"Dog with name '{request.Name}' already exists.", exception.Message);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Dog>()), Times.Never);
    }

    [Theory]
    [InlineData(null, "Red", 10, 10, "Name")]         // 1. Null Name
    [InlineData("", "Red", 10, 10, "Name")]           // 2. Empty Name
    [InlineData("Neo", null, 10, 10, "Color")]        // 3. Null Color
    [InlineData("Neo", "", 10, 10, "Color")]          // 4. Empty Color
    public async Task CreateAsync_WithInvalidStringArguments_ShouldThrowArgumentException(
        string name, string color, int tail, int weight, string paramName)
    {
        // Arrange
        var request = new CreateDogRequest
        {
            Name = name,
            Color = color,
            TailLength = tail,
            Weight = weight
        };
        // Act
        var action = async () => await _service.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(action);
        Assert.Equal(paramName, exception.ParamName);
    }

    [Theory]
    [InlineData("Neo", "Red", 0, 10, "TailLength")]   // 5. Zero Tail
    [InlineData("Neo", "Red", -5, 10, "TailLength")]  // 6. Negative Tail
    [InlineData("Neo", "Red", 10, 0, "Weight")]       // 7. Zero Weight
    [InlineData("Neo", "Red", 10, -5, "Weight")]      // 8. Negative Weight
    public async Task CreateAsync_WithInvalidNumericArguments_ShouldThrowArgumentOutOfRangeException(
        string name, string color, int tail, int weight, string paramName)
    {
        // Arrange
        var request = new CreateDogRequest
        {
            Name = name,
            Color = color,
            TailLength = tail,
            Weight = weight
        };
        // Act
        var action = async () => await _service.CreateAsync(request);
        // Assert
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(action);
        Assert.Equal(paramName, exception.ParamName);
    }

    #endregion


    #region GetListAsync Tests
    [Fact]
    public async Task GetListAsync_ShouldReturnPagedAndMappedDogs()
    {
        // Arrange
        var (attr, order, page, size) = ("weight", "desc", 1, 2);

        var dogs = new List<Dog>
{
            new Dog { Id = Guid.NewGuid(), Name = "Neo", Color = "Red", TailLength = 22, Weight = 32 },
            new Dog { Id = Guid.NewGuid(), Name = "Jessy", Color = "Black", TailLength = 7, Weight = 14 }
        };

        int totalCount = dogs.Count;

        _repositoryMock.Setup(r => r.GetPagedAsync(attr, order, page, size))
            .ReturnsAsync((dogs, totalCount));

        // Act
        var (items, total) = await _service.GetListAsync(attr, order, page, size);

        // Assert
        Assert.Equal(totalCount, total);
        Assert.Equal(dogs.Count, items.Count());
        Assert.Equal("Neo", items.First().Name);
        Assert.IsAssignableFrom<IEnumerable<DogDto>>(items);

        _repositoryMock.Verify(r => r.GetPagedAsync(attr, order, page, size), Times.Once);
    }

    [Fact]
    public async Task GetListAsync_WhenNoDogsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyList = new List<Dog>();
        var totalCount = 0;

        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((emptyList, totalCount));

        // Act
        var (items, total) = await _service.GetListAsync("name", "asc", 1, 10);

        // Assert
        Assert.Equal(0, total);
        Assert.Empty(items);
    }

    #endregion
}