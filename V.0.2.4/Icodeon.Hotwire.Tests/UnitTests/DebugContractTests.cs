using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Diagnostics;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotNullableThrowsExceptionIfNullablePropertyIsNull()
        {
            var Fred = new Person { Name = "Fred", Age = null };
            DebugContract.NotNullable(()=> Fred.Age);
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsExceptionIfScalarTypeIsNull()
        {
            int? age = null;
            DebugContract.NotNullable(() => age);
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
