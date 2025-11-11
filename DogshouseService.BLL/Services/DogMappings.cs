using DogshouseService.BLL.DTOs;
using DogshouseService.DAL.Entities;

namespace DogshouseService.BLL.Services;

public static class DogMappings
{
    public static DogDto ToDto(this Dog dog)
        => new DogDto(dog.Name, dog.Color, dog.TailLength, dog.Weight);

    public static Dog ToEntity(this CreateDogRequest dogRequest) => new Dog
    {
        Name = dogRequest.Name.Trim(),
        Color = dogRequest.Color.Trim(),
        TailLength = dogRequest.TailLength,
        Weight = dogRequest.Weight
    };
}
