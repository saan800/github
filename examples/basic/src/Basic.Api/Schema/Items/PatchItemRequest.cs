using System.ComponentModel.DataAnnotations;

namespace Basic.Api.Schema.Items;

public class PatchItemRequest
{
    [MinLength(1)]
    public string? Name { get; set; }

    [MinLength(1)]
    public string? Description { get; set; }
}
