using System;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Diagnostics;
using NUnit.Framework;

// these tests should only run for debug configuration.
#if (DEBUG)
namespace Icodeon.Hotwire.Tests.UnitTests.DebugOnly
{

    [TestFixture]
    public class DebugContractTests
    {
        internal class Person
        {
            public string Name { get; set; }
            public int? Age { get; set; }
            public Person Father { get; set; }
        }

        
        [Test]
        public void NotNullableThrowsExceptionIfNullablePropertyIsNull()
        {
            Action action = () => {
                                    var Fred = new Person {Name = "Fred", Age = null};
                                    DebugContract.NotNullable(() => Fred.Age);
                                };
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void NotNullableDoesNotThrowsExceptionIfNullablePropertyIsNotNull()
        {
            var Fred = new Person { Name = "Fred", Age = 10 };
            DebugContract.NotNullable(()=> Fred.Age);
        }

        [Test]
        public void ExceptionContainsVariableName()
        {
            var Fred = new Person { Name = "Fred", Age = null };
            try
            {
                DebugContract.NotNullable(()=> Fred.Age);
                Assert.Fail("Did not throw exception.");
            }
            catch (ArgumentNullException an)
            {
                an.Message.Should().Contain("'Age'");
            }
        }

        [Test]
        public void ThrowsExceptionIfScalarTypeIsNull()
        {
            Action action = () =>
                                {
                                    int? age = null;
                                    DebugContract.NotNullable(() => age);
                                };
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void DoesNotThrowExceptionIfScalarTypeIsNotNull()
        {
            int? age = 5;
            DebugContract.NotNullable(() => age);
        }


        [Test]
        public void CanValidateNonNullableScalarTypesWithoutException()
        {
            int age = 5;
            DebugContract.NotNullable(() => age);
        }


 
    }
}
#endif