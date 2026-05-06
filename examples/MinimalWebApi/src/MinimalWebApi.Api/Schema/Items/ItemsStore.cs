namespace MinimalWebApi.Api.Schema.Items;

public class ItemsStore
{
    private readonly Lock _lock = new();
    private List<Item> _items = SeedData();

    public List<Item> GetAll()
    {
        lock (_lock) return [.._items];
    }

    public Item? GetById(int id)
    {
        lock (_lock) return _items.FirstOrDefault(x => x.Id == id);
    }

    public Item Add(CreateItemRequest request)
    {
        lock (_lock)
        {
            var newItem = new Item
            {
                Id = _items.Count == 0 ? 1 : _items.Max(x => x.Id) + 1,
                Name = request.Name,
                Description = request.Description,
            };
            _items = [.._items.Where(x => x.Id != newItem.Id), newItem];
            return newItem;
        }
    }

    public bool Patch(int id, PatchItemRequest request)
    {
        lock (_lock)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item == null) return false;

            if (request.Name != null) item.Name = request.Name;
            if (request.Description != null) item.Description = request.Description;

            _items = [.._items.Where(x => x.Id != id), item];
            return true;
        }
    }

    public void Delete(int id)
    {
        lock (_lock) _items = _items.Where(x => x.Id != id).ToList();
    }

    public void Reset()
    {
        lock (_lock) _items = SeedData();
    }

    private static List<Item> SeedData() =>
        Enumerable.Range(1, 5).Select(x => new Item { Id = x, Name = $"Item {x}" }).ToList();
}
