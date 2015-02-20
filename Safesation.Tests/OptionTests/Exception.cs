using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.SafeTests
{
    public class Exception
    {
        [Test]
        public void Exception_ReturnsException_ForFail()
        {
            var exception = new System.Exception();
            Safe.Option(exception).Exception
                .Should().Be(exception);
        }

        [Test]
        public void Exception_ReturnsNull_ForNone()
        {
            Safe.Option().Exception
                .Should().BeNull();
        }

        [Test]
        public void Exception_ReturnsNull_ForSome()
        {
            Safe.Option(0).Exception
                .Should().BeNull();
        }
    }
}