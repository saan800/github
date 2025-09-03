using NugetPackages.Alpha;

namespace NugetPackages.Tests.Alpha;

public class GreetingTests
{
    [Theory]
    [InlineData("World", "Hello, World!")]
    [InlineData("Bob", "Hello, Bob!")]
    public void Hello_Returns_Greeting(string name, string expected)
    {
        Greetings.Hello(name).Should().Be(expected);
    }

}
