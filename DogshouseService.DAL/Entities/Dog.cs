namespace DogshouseService.DAL.Entities;

public class Dog
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public int TailLength { get; set; }
    public int Weight { get; set; }
}