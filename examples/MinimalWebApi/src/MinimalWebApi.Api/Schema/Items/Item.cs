namespace MinimalWebApi.Api.Schema.Items;

public class Item
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
