## QuickAssert.cs ##

I've just started using [Fluent Assertions](http://fluentassertions.codeplex.com/), (which I love by the way), however, there are some places where I find both the NUnit approach and the Fluent approach results in code that I find is a little cluttered and not very scannable.

**QuickAssert.cs is an extension class that allows me to go from this: (fluent style)**
```
config.Active.Should().BeTrue();
config.RootServiceName.Should().Be("test-animals");
config.MethodValidation.Should().Be(MethodValidation.afterUriValidation);
var endpoints = config.Endpoints;
endpoints.Should().NotBeNull().And.HaveCount(2);
```

**to this:**

```
config.Ensure(c => c.Active,
              c => c.RootServiceName == "test-animals",
              c => c.MethodValidation == MethodValidation.afterUriValidation,
              c => c.Endpoints != null && c.Endpoints.Count() == 2);
```

QuickAssert also fails (if error is detected) with the text representation of the exact code used to check the property, without having to provide a string description. This is not a replacement for  Fluent assertions, which will do a better job of expressing very complex requirements. This is for when I need something simple, e.g. checking long lists of properties. I may simply be using Fluent Assertions properly, so please send me comments if there is a better way to do this. Cheers, Al

**Here is the code for QuickAssert so you can use it if you want, without having to get the entire Hotwire codebase**

```
using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.Framework
{
    public static class QuickAssert
    {
        public static void Ensure<TSource>(this TSource source, params Expression<Func<TSource, bool>>[] actions)
        {
            foreach (var expression in actions)
            {
                Ensure(source,expression);
            }
        }

        public static void Ensure<TSource>(this TSource source, Expression<Func<TSource, bool>> action)
        {
            var propertyCaller = action.Compile();
            bool result = propertyCaller(source);
            if (result) return;
            Assert.Fail("Property check failed -> " + action.ToString());
        }
    }
}
```