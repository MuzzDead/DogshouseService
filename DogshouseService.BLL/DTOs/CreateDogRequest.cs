using System.Text.Json.Serialization;

namespace DogshouseService.BLL.DTOs;

public class CreateDogRequest
{
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    
    [JsonPropertyName("tail_length")]
    public int TailLength { get; set; }
    public int Weight { get; set; }
}