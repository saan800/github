using System.ComponentModel.DataAnnotations;

namespace MinimalWebApi.Api.Schema.Items;

public class CreateItemRequest
{
    [Required, MinLength(1)]
    public required string Name { get; set; }
    public string? Description { get; set; }
}
