using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.SafeTests
{
    public class IsFail
    {
        [Test]
        public void IsFail_ReturnsTrue_ForFail()
        {
            Safe.Option(new System.Exception()).IsFail
                .Should().BeTrue();
        }

        [Test]
        public void IsFail_ReturnsFalse_ForNone()
        {
            Safe.Option().IsFail
                .Should().BeFalse();
        }

        [Test]
        public void IsFail_ReturnsFalse_ForSome()
        {
            Safe.Option(0).IsFail
                .Should().BeFalse();
        }
    }
}