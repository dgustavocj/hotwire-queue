using System;
using System.Linq.Expressions;

namespace Icodeon.Hotwire.TestFramework
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
            throw new ArgumentException("Property check failed -> " + action.ToString());
        }
    }
}
