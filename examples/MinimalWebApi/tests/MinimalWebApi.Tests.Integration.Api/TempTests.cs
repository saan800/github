namespace MinimalWebApi.Tests.Integration.Api;

public class TempTests
{
    [Fact]
    public void Test1()
    {
        "hello".Length.Should().Be(5);
    }
}

