using MinimalWebApi.Api.Schema.Items;

namespace MinimalWebApi.Tests.Api.Schema;

public class ItemTests
{
    [Fact]
    public void ATest()
    {
        var item = new Item
        {
            Id = 1,
            Name = "bob",
        };

        item.Name.Should().Be("hello");
    }
}
