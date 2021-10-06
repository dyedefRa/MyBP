using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBIP.Components.StoreProcedure.Required
{
    public class DatabaseHelper
    {
        public static IDatabaseManager GetDatabaseManager(string connectionStringName)
        {
            return new MssqlDatabaseManager(connectionStringName);
        }
    }
}
