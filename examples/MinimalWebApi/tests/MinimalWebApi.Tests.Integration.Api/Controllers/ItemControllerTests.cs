using System.Net;
using System.Net.Http.Json;
using MinimalWebApi.Api.Schema.Items;

namespace MinimalWebApi.Tests.Integration.Api.Controllers;

[Collection(TestConfigConstants.Collections.Api)]
public class ItemControllerTests(TestHttpClientFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ShouldReturnSeededItems()
    {
        var items = await _client.GetFromJsonAsync<List<Item>>("/api/items");

        items.Should().NotBeNull();
        items.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_ShouldReturnCorrectItem()
    {
        var item = await _client.GetFromJsonAsync<Item>("/api/items/1");

        item.Should().NotBeNull();
        item.Id.Should().Be(1);
        item.Name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Create_ShouldAddNewItem()
    {
        var currentItems = await _client.GetFromJsonAsync<List<Item>>("/api/items");
        currentItems.Should().NotBeNull("Need existing items to be able to test the create functionality");

        var newItemRequest = new CreateItemRequest { Name = "New Item", Description = $"Cool items here {Guid.NewGuid()}" };

        var response = await _client.PostAsJsonAsync("/api/items", newItemRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.ToString().Should().StartWith("/api/items/");

        // Verify it now exists
        var item = await _client.GetFromJsonAsync<Item>(response.Headers.Location!.ToString());
        item.Should().NotBeNull();
        item.Id.Should().BeGreaterThan(currentItems.Max(x => x.Id));
        item.Name.Should().Be(newItemRequest.Name);
        item.Description.Should().Be(newItemRequest.Description);
    }

    [Fact]
    public async Task Patch_ShouldModifyExistingItem()
    {
        var currentItems = await _client.GetFromJsonAsync<List<Item>>("/api/items");
        currentItems.Should().NotBeNull("Need existing items to be able to test the create functionality");

        var itemToModify = currentItems.Last();
        var patchRequest = new PatchItemRequest { Description = $"Updated description {Guid.NewGuid()}" };

        var response = await _client.PatchAsJsonAsync($"/api/items/{itemToModify.Id}", patchRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.ToString().Should().Be($"/api/items/{itemToModify.Id}");

        // Verify it has been updated
        var item = await _client.GetFromJsonAsync<Item>(response.Headers.Location!.ToString());
        item.Should().NotBeNull();
        item.Id.Should().Be(itemToModify.Id);
        item.Name.Should().Be(itemToModify.Name);
        item.Description.Should().Be(patchRequest.Description);
    }

    [Fact]
    public async Task Delete_ShouldRemoveItem()
    {
        var items = await _client.GetFromJsonAsync<List<Item>>("/api/items");
        items.Should().NotBeNull("Need existing items to be able to test the delete functionality");

        var itemIdToDelete = items.First().Id;
        var response = await _client.DeleteAsync($"/api/items/{itemIdToDelete}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var notFound = await _client.GetAsync($"/api/items/{itemIdToDelete}");
        notFound.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
