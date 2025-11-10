using DogshouseService.DAL.Entities;

namespace DogshouseService.DAL.Interfaces;

public interface IDogsRepository
{
    Task AddAsync(Dog dog);
    Task<bool> ExistsByNameAsync(string name);
    Task<(IList<Dog> Items, int Total)> GetPagedAsync(string? attribute, string? order, int pageNumber, int pageSize);
}
