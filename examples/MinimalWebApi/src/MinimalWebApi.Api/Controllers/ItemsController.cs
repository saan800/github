using Microsoft.AspNetCore.Mvc;
using MinimalWebApi.Api.Schema.Items;

namespace MinimalWebApi.Api.Controllers;

// TODO: dotnet v10 should have xml docs back
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/openapi-comments?view=aspnetcore-10.0

[Route("api/[controller]")]
public class ItemsController : BaseController
{
    private static List<Item> _items = Enumerable
        .Range(1, 5)
        .Select(x => new Item { Id = x, Name = $"Item {x}" })
        .ToList();

    [HttpGet]
    [ProducesResponseType(typeof(List<Item>), StatusCodes.Status200OK)]
    [EndpointDescription("Get all items, ordered by Id")]
    public IActionResult GetAll()
    {
        return Ok(_items.OrderBy(x => x.Id));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Get an item's details for the provided id")]
    public IActionResult GetById(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        return item == null
            ? NotFound()
            : Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [EndpointDescription("Create a new item. On success returns 201 Created and sets the Location header to the new item's URL.")]
    public IActionResult Create([FromBody] CreateItemRequest request)
    {
        var newItem = new Item
        {
            Id = _items.Count == 0 ? 1 : _items.Max(x => x.Id) + 1,
            Name = request.Name,
            Description = request.Description,
        };

        _items = _items
            .Where(x => x.Id != newItem.Id)
            .Append(newItem)
            .ToList();

        return Created($"/api/items/{newItem.Id}", null);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Updates only the provided fields for an item")]
    public IActionResult PatchUpdate(int id, [FromBody] PatchItemRequest patchRequest)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();

        if (patchRequest.Name != null) item.Name = patchRequest.Name;
        if (patchRequest.Description != null) item.Description = patchRequest.Description;

        _items = _items
            .Where(x => x.Id != item.Id)
            .Append(item)
            .ToList();

        return Accepted($"/api/items/{item.Id}");
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [EndpointDescription("Deletes the item with the provided id")]
    public IActionResult Delete(int id)
    {
        _items = _items
            .Where(x => x.Id != id)
            .ToList();

        return NoContent();
    }
}
