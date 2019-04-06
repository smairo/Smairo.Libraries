using System;
using Xunit;
namespace Smairo.Extensions.Tests
{
    public class StringTests
    {
        [Theory]
        // ReSharper disable StringLiteralTypo
        [InlineData("Name", "ME", true)]
        [InlineData("Name", "Name", true)]
        [InlineData("Name", "NAMe", true)]
        [InlineData("Name", "n", true)]
        [InlineData("Name", "z", false)]
        [InlineData("Name", "am", true)]
        [InlineData("Name", "aM", true)]
        [InlineData("Name", "Namez", false)]
        [InlineData("Ärräpöö", "äpö", true)]
        [InlineData("Ärräpöö", "ÖÖ", true)]
        // ReSharper enable StringLiteralTypo
        public void Test_Contains(string test, string compare, bool actual)
        {
            Assert.Equal(actual, test.Contains(compare, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}