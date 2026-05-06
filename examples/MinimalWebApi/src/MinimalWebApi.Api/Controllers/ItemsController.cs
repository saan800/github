using Microsoft.AspNetCore.Mvc;
using MinimalWebApi.Api.Schema.Items;

namespace MinimalWebApi.Api.Controllers;

// TODO: dotnet v10 should have xml docs back
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/openapi-comments?view=aspnetcore-10.0

[Route("api/[controller]")]
public class ItemsController(ItemsStore store) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<Item>), StatusCodes.Status200OK)]
    [EndpointDescription("Get all items, ordered by Id")]
    public IActionResult GetAll()
    {
        return Ok(store.GetAll().OrderBy(x => x.Id));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Get an item's details for the provided id")]
    public IActionResult GetById(int id)
    {
        var item = store.GetById(id);
        return item == null
            ? NotFound()
            : Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [EndpointDescription("Create a new item. On success returns 201 Created and sets the Location header to the new item's URL.")]
    public IActionResult Create([FromBody] CreateItemRequest request)
    {
        var newItem = store.Add(request);
        return Created($"/api/items/{newItem.Id}", null);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Updates only the provided fields for an item")]
    public IActionResult PatchUpdate(int id, [FromBody] PatchItemRequest patchRequest)
    {
        var updated = store.Patch(id, patchRequest);
        if (!updated) return NotFound();
        return Accepted($"/api/items/{id}");
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [EndpointDescription("Deletes the item with the provided id")]
    public IActionResult Delete(int id)
    {
        store.Delete(id);
        return NoContent();
    }
}
