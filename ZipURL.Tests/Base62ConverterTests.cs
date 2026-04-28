using Xunit;
using ZipURL.Services.ShorterURL.Helpers;

namespace ZipURL.Tests
{
    public class Base62ConverterTests
    {
        [Theory]
        [InlineData(0, "a")]
        [InlineData(1, "b")]
        [InlineData(61, "9")]
        [InlineData(62, "ba")]
        [InlineData(12345, "dnh")]
        [InlineData(1000000, "emjc")]
        public void Encode_ShouldMatchExpectedValues(int number, string expected)
        {
            var result = Base62Converter.Encode(number);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("a", 0)]
        [InlineData("9", 61)]
        [InlineData("ba", 62)]
        public void Decode_ShouldMatchExpectedNumbers(string code, int expected)
        {
            var result = Base62Converter.Decode(code);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test_Maximum_Int32()
        {
            int maxInt = int.MaxValue; // 2,147,483,647
            string encoded = Base62Converter.Encode(maxInt);
            int decoded = Base62Converter.Decode(encoded);

            Assert.Equal(maxInt, decoded);
        }
    }
}