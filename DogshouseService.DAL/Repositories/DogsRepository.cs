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
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Dogs.AnyAsync(d => d.Name.ToLower() == name.ToLower());
    }

    public async Task<(IList<Dog> Items, int Total)> GetPagedAsync(string? attribute, string? order, int pageNumber, int pageSize)
    {
        var query = _context.Dogs.AsQueryable();

        attribute = attribute?.ToLower() ?? "name";
        order = order?.ToLower() ?? "asc";

        query = (attribute, order) switch
        {
            ("weight", "desc") => query.OrderByDescending(d => d.Weight),
            ("weight", "asc") => query.OrderBy(d => d.Weight),
            ("tail_length", "desc") => query.OrderByDescending(d => d.TailLength),
            ("tail_length", "asc") => query.OrderBy(d => d.TailLength),
            ("color", "desc") => query.OrderByDescending(d => d.Color),
            ("color", "asc") => query.OrderBy(d => d.Color),

            _ => (order == "desc" ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name))
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
