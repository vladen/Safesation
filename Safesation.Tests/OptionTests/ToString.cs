using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.SafeTests
{
    public class ToString
    {
        [Test]
        public void ToString_Fail_ReturnsExceptionToString()
        {
            var exception = new System.Exception("test");
            var @string = exception.ToString();
            Safe.Option(exception).ToString()
                .Should().Be(@string);
        }

        [Test]
        public void ToString_None_ReturnsEmptyString()
        {
            Safe.Option().ToString()
                .Should().Be(string.Empty);
        }

        [Test]
        public void ToString_Some_ReturnsValueString()
        {
            const string value = "test";
            Safe.Option(value).ToString()
                .Should().Be(value);
        }
    }
}