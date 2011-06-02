using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public static class DebugContract
    {
        
        /// <example>DebugContract.NotNullable(()=> Fred.Age);</example>
        [Conditional("DEBUG")]
        public static void NotNullable<TProperty>(Expression<Func<TProperty>> action) 
        {
            var propertyCaller = action.Compile();
            TProperty result = propertyCaller();
            if (result == null)
            {
                var expression = action.Body as MemberExpression;
                if (expression == null) throw new ArgumentException("Property expression cannot be null!");
                throw new ArgumentNullException("property '" + expression.Member.Name + "' cannot be null!");
            }
        }

        // Below is a copy of the code in QuickAssert, however, because it's conditional, I can't have test framework delegate to these methods
        // so QuickAssert has it's own implementation. (source copy.)

        [Conditional("DEBUG")]
        public static void Ensure<TSource>(this TSource source, params Expression<Func<TSource, bool>>[] actions)
        {
            foreach (var expression in actions)
            {
                Ensure(source, expression);
            }
        }

        [Conditional("DEBUG")]
        public static void Ensure<TSource>(this TSource source, Expression<Func<TSource, bool>> action)
        {
            var propertyCaller = action.Compile();
            bool result = propertyCaller(source);
            if (result) return;
            throw new ArgumentOutOfRangeException("Contract assert failed. " + action.ToString());
        }

        [Conditional("DEBUG")]
        public static void Ensure(params Expression<Func<bool>>[] actions)
        {
            foreach (var expression in actions)
            {
                Ensure(expression);
            }
        }

        [Conditional("DEBUG")]
        public static void Ensure(Expression<Func<bool>> action)
        {
            var propertyCaller = action.Compile();
            bool result = propertyCaller();
            if (result) return;
            throw new ArgumentOutOfRangeException("Contract assert failed. " + action.ToString());
        }


    }
}
