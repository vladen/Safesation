using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.SafeTests
{
    public class IsSome
    {
        [Test]
        public void IsSome_Fail_ReturnsTrue()
        {
            Safe.Option(new System.Exception()).IsSome
                .Should().BeFalse();
        }

        [Test]
        public void IsSome_None_ReturnsFalse()
        {
            Safe.Option().IsSome
                .Should().BeFalse();
        }

        [Test]
        public void IsSome_Some_ReturnsTrue()
        {
            Safe.Option(0).IsSome
                .Should().BeTrue();
        }
    }
}