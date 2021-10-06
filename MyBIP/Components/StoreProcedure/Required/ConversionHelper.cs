using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBIP.Components.StoreProcedure.Required
{
    public static class ConversionHelper
    {
        public static T ChangeType<T>(object value)
        {
            var t = typeof(T);

            if (value == null)
            {
                return default(T);
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                t = Nullable.GetUnderlyingType(t);
            }
            return (T)Convert.ChangeType(value, t);
        }
    }
}
