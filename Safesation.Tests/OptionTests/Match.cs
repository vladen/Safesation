using System;
using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.OptionTests
{
    public class Match
    {
        [Test]
        public void Match_DoesNotInvokeNoneAction_ForSome()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option(0).Match(
                value => { },
                () =>
                {
                    invoked = true;
                });
            invoked
                .Should().BeFalse();
        }

        [Test]
        public void Match_DoesNotInvokeNoneFunction_ForSome()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option(0).Match(
                value => 1,
                () =>
                {
                    invoked = true;
                    return 1;
                });
            invoked
                .Should().BeFalse();
        }

        [Test]
        public void Match_DoesNotInvokeSomeAction_ForNone()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option<int>().Match(
                exception =>
                {
                    invoked = true;
                });
            invoked
                .Should().BeFalse();
        }

        [Test]
        public void Match_DoesNotInvokeSomeFunction_ForNone()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option<int>().Match(
                exception =>
                {
                    invoked = true;
                    return 1;
                });
            invoked
                .Should().BeFalse();
        }

        [Test]
        public void Match_InvokesNoneAction_ForNone()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option<int>().Match(
                value => { },
                () =>
                {
                    invoked = true;
                });
            invoked
                .Should().BeTrue();
        }

        [Test]
        public void Match_InvokesNoneFunction_ForNone()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option<int>().Match(
                value => 1,
                () =>
                {
                    invoked = true;
                    return 1;
                });
            invoked
                .Should().BeTrue();
        }

        [Test]
        public void Match_InvokesSomeAction_ForSome()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option(0).Match(
                value =>
                {
                    invoked = true;
                });
            invoked
                .Should().BeTrue();
        }

        [Test]
        public void Match_InvokesSomeFunction_ForSome()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option(0).Match(
                value =>
                {
                    invoked = true;
                    return 1;
                });
            invoked
                .Should().BeTrue();
        }

        [Test]
        public void Match_ReturnsFail_WhenNoneActionThrows()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            var result = Safe.Option<int>().Match(
                value => { },
                () =>
                {
                    throw new Exception();
                });
            result.IsFail
                .Should().BeTrue();
        }

        [Test]
        public void Match_ReturnsFail_WhenNoneFunctionThrows()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            var result = Safe.Option<int>().Match(
                value => 1,
                () =>
                {
                    throw new Exception();
                    // ReSharper disable once CSharpWarnings::CS0162
                    return 1;
                });
            result.IsFail
                .Should().BeTrue();
        }

        [Test]
        public void Match_ReturnsFail_WhenSomeActionThrows()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            var result = Safe.Option(0).Match(
                value =>
                {
                    throw new Exception();
                });
            result.IsFail
                .Should().BeTrue();
        }

        [Test]
        public void Match_ReturnsFail_WhenSomeFunctionThrows()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            var result = Safe.Option(0).Match(
                value =>
                {
                    throw new Exception();
                    // ReSharper disable once CSharpWarnings::CS0162
                    return 1;
                });
            result.IsFail
                .Should().BeTrue();
        }
    }
}