using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.TestFramework;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class ThreadTestRunnerTests : UnitTest
    {
        [Test]
        public void ShouldReturnExceptionThrownInThread()
        {
            TraceTitle("ShouldReturnExceptionThrownInThread() - Should return exeption thrown in thread.");

            Trace("given a thread test runner and an action that will throw an exception");
            int i = 5;
            var runner = new ThreadTestRunner(500);
            Action actionThatThrows = () => { throw new ArithmeticException(); };
            Action actionThatIncrements = () => { i++; };

            Trace("When I run the action in a seperate thread");
            Trace("Then the test runner should raise the exception");
            try
            {
                runner.RunInParallel(new[] { actionThatThrows, actionThatIncrements });
                Assert.Fail("was expecting ThreadTestRunnerException.");
            }
            catch (ThreadTestRunnerException ex)
            {
                ex.LastException.Should().BeOfType<ArithmeticException>();
                ex.Exceptions.Should().HaveCount(1);
            }

            Trace("And the thread that did not throw exception should be unnafected.");
            i.Should().Be(6);
        }


        [Test]
        public void ShouldRunActionsInParallel()
        {
            TraceTitle("ShouldRunActionsInParallel() - Should run actions in parallel.");

            Trace("given a thread test runner and an action that will throw an exception");
            int i = 5;
            int j = 10;
            var runner = new ThreadTestRunner(500);
            Action add10 = () => { i += 10; };
            Action halve = () => { j=(j/2); };

            Trace("When I run the action in a seperate thread");
            Action action = () => runner.RunInParallel(new[] { add10, halve });

            Trace("Then the code should have executed in parallel.");
            // not a good test, need better example.
            action.ShouldNotThrow();
            i.Should().Be(15);
            j.Should().Be(5);
        }

    }
}
