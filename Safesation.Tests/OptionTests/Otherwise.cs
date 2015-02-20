using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.OptionTests
{
    public class Otherwise
    {
        [Test]
        public void Otherwise_Fail_ReturnsOtherwise()
        {
            var otherwise = new object();
            Safe.Option<object>(new System.Exception()).Otherwise(otherwise)
                .Should().Be(otherwise);
        }

        [Test]
        public void Otherwise_None_ReturnsOtherwise()
        {
            var otherwise = new object();
            Safe.Option().Otherwise(otherwise)
                .Should().Be(otherwise);
        }

        [Test]
        public void Otherwise_Some_ReturnsValue()
        {
            Safe.Option(0).Otherwise(1)
                .Should().Be(0);
        }
    }
}
