namespace MinimalWebApi.Tests.Integration.Api;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        "hello".Length.Should().Be(5);
    }
}
