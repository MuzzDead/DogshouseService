using DogshouseService.DAL.Entities;

namespace DogshouseService.DAL.Interfaces;

public interface IDogsRepository
{
    Task AddAsync(Dog dog);
    Task UpdateAsync(Dog dog);
    Task DeleteAsync(Guid id);
    Task<Dog?> GetByIdAsync(Guid id);
    Task<IEnumerable<Dog>> GetAllAsync();
}
