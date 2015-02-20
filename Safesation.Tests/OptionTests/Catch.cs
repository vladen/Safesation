using System;
using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.OptionTests
{
    public class Catch
    {
        [Test]
        public void Catch_DoesNotInvokeCatchDelegate_ForNone()
        {
            var invoked = false;
            Safe.Option().Catch(
                exception =>
                {
                    invoked = true;
                });
            invoked
                .Should().BeFalse();
        }

        [Test]
        public void Catch_DoesNotInvokeCatchDelegate_ForSome()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option(0).Catch(
                exception =>
                {
                    invoked = true;
                });
            invoked
                .Should().BeFalse();
        }

        [Test]
        public void Catch_ReturnsFail_ForFail_WhenExceptionDoesNotMatch()
        {
            Safe.Option(new Exception()).Catch<ApplicationException>(exception => { }).IsFail
                .Should().BeTrue();
        }

        [Test]
        public void Catch_ReturnsNone_ForFail()
        {
            Safe.Option(new Exception()).Catch(exception => { }).IsNone
                .Should().BeTrue();
        }

        [Test]
        public void Catch_ReturnsNone_ForNone()
        {
            Safe.Option().Catch(exception => { }).IsNone
                .Should().BeTrue();
        }

        [Test]
        public void Catch_ReturnsSome_ForSome()
        {
            Safe.Option(0).Catch(exception => { }).IsSome
                .Should().BeTrue();
        }

        [Test]
        public void GenericCatch_DoesNotInvokeCatchDelegate_ForFail_WhenExceptionDoesNotMatch()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option(new Exception()).Catch<ApplicationException>(
                exception =>
                {
                    invoked = true;
                });
            invoked
                .Should().BeFalse();
        }

        [Test]
        public void GenericCatch_InvokesCatchDelegate_ForFail_WhenExceptionMatches()
        {
            var invoked = false;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Safe.Option(new ApplicationException()).Catch<Exception>(
                exception =>
                {
                    invoked = true;
                });
            invoked
                .Should().BeTrue();
        }
    }
}