using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBIP.Components.StoreProcedure.Required
{
    public interface IDatabaseManager
    {
        List<DbQueryParameter> OutParameters { get; }

        List<T> ExecuteList<T>(string procedureName) where T : new();
        List<T> ExecuteList<T>(string procedureName, List<DbQueryParameter> parameters) where T : new();
        List<T> ExecuteListv2<T>(string procedureName, List<DbQueryParameter> parameters);
        Tuple<List<T1>, List<T2>> ExecuteMultiListv2<T1, T2>(string procedureName, List<DbQueryParameter> parameters)
            where T1 : new()
            where T2 : new();
        int ExecuteNonQuery(string procedureName, List<DbQueryParameter> parameters);
        T ExecuteSingle<T>(string procedureName) where T : new();
        T ExecuteSingle<T>(string procedureName, List<DbQueryParameter> parameters) where T : new();
        T ExecuteSingleV2<T>(string procedureName, List<DbQueryParameter> parameters);
    }
}
