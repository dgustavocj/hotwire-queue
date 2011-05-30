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

    }
}
