using DogshouseService.BLL.DTOs;

namespace DogshouseService.BLL.Interfaces;

public interface IDogService
{
    Task<DogDto> CreateAsync(CreateDogRequest request);
    Task<(IEnumerable<DogDto> Items, int Total)> GetListAsync(string? attribute, string? order, int pageNumber, int pageSize);
}
