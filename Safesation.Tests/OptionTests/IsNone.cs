using System;
using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.SafeTests
{
    public class IsNone
    {
        [Test]
        public void IsNone_Exception_ReturnsFalse()
        {
            Safe.Option(new Exception()).IsNone
                .Should().BeFalse();
        }

        [Test]
        public void IsNone_Empty_ReturnsTrue()
        {
            Safe.Option().IsNone
                .Should().BeTrue();
        }

        [Test]
        public void IsNone_Int32_ReturnsFalse()
        {
            Safe.Option(0).IsNone
                .Should().BeFalse();
        }
    }
}