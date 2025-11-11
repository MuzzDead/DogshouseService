using DogshouseService.BLL.DTOs;
using DogshouseService.BLL.Interfaces;
using DogshouseService.DAL.Interfaces;

namespace DogshouseService.BLL.Services;

public class DogService : IDogService
{
    private readonly IDogsRepository _repository;
    public DogService(IDogsRepository repository)
    {
        _repository = repository;
    }

    public async Task<DogDto> CreateAsync(CreateDogRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
            throw new ArgumentException("Name cannot be null or empty", nameof(request.Name));

        if (string.IsNullOrEmpty(request.Color))
            throw new ArgumentException("Color cannot be null or empty", nameof(request.Color));

        if (request.TailLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.TailLength), "TailLength cannot be negative");

        if (request.Weight <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.Weight), "Weight cannot be negative");

        if (await _repository.ExistsByNameAsync(request.Name.Trim()))
            throw new InvalidOperationException($"Dog with name '{request.Name}' already exists.");

        var dogEntity = request.ToEntity();
        await _repository.AddAsync(dogEntity);
        return dogEntity.ToDto();
    }

    public async Task<(IEnumerable<DogDto> Items, int Total)> GetListAsync(string? attribute, string? order, int pageNumber, int pageSize)
    {
        var (items, total) = await _repository.GetPagedAsync(attribute, order, pageNumber, pageSize);
        return (items.Select(d => d.ToDto()), total);
    }
}
