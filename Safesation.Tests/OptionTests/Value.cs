using System;
using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.SafeTests
{
    public class Value
    {
        [Test]
        public void Value_Fail_Throws()
        {
            var exception = new System.Exception();
            var action = new Action(() =>
            {
                exception = Safe.Option(exception).Value;
            });
            action
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void Value_None_Throws()
        {
            // ReSharper disable once NotAccessedVariable
            var value = default(object);
            var action = new Action(() =>
            {
                value = Safe.Option().Value;
            });
            action
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void Value_Some_ReturnsValue()
        {
            Safe.Option(0).Value
                .Should().Be(0);
        }
    }
}