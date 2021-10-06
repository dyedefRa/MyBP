using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyBIP.Components.StoreProcedure.Required
{
    public class DbQueryParameter
    {
        public DbQueryParameter(string paramName, object paramValue, ParameterDirection paramDirection = ParameterDirection.Input, bool hasDateToFloatConversion = false)
        {
            Name = paramName;
            Value = paramValue;
            Direction = paramDirection;
            HasDateToFloatConversion = hasDateToFloatConversion;
        }

        public string Name { get; set; }
        public ParameterDirection Direction { get; set; }
        public object Value { get; set; }
        public bool HasDateToFloatConversion { get; set; }
    }
}
