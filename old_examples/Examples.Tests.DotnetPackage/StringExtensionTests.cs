namespace Examples.Tests.DotnetPackage;

public class StringExtensionTests
{
    [Fact]
    public void ToCamelCase_should_format_string()
    {
        var str = "hello world";

        var result = str.ToCamelCase();

        result.Should().Be("Hello World");
    }
}
