using NugetPackages.Bravo;

namespace NugetPackages.Tests.Bravo;

public class NumberTests
{
    [Fact]
    public void Add_Works()
    {
        Numbers.Add(2, 3).Should().Be(5);
    }
}
