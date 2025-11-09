using DogshouseService.DAL.Entities;
using DogshouseService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DogshouseService.DAL.Repositories;

public class DogsRepository : IDogsRepository
{
    private readonly ApplicationDbContext _context;
    public DogsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Dog dog)
    {
        await _context.Dogs.AddAsync(dog);
    }

    public async Task DeleteAsync(Guid id)
    {
        var dog = await _context.Dogs.FindAsync(id);
        if (dog != null)
        {
            _context.Dogs.Remove(dog);
        }
    }

    public async Task<IEnumerable<Dog>> GetAllAsync()
    {
        return await _context.Dogs.ToListAsync();
    }

    public async Task<Dog?> GetByIdAsync(Guid id)
    {
        return await _context.Dogs.FindAsync(id);
    }

    public Task UpdateAsync(Dog dog)
    {
        _context.Dogs.Update(dog);
        return Task.CompletedTask;
    }
}
