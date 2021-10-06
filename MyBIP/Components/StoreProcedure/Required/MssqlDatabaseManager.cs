
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyBIP.Components.StoreProcedure.Required
{
    public class MssqlDatabaseManager : IDatabaseManager
    {
        public Action<object, Exception> OnError;


        private string ConnectionString
        {
            get
            {

                //return ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
                return "Server=DESKTOP-STGF1UC;Database=BIPv2;Trusted_Connection=True;";


            }
        }

        public string ConnectionStringName { get; set; }

        private SqlConnection Connection { get; set; }

        private SqlCommand Command { get; set; }

        public List<DbQueryParameter> OutParameters { get; private set; }

        public int TotalRows { get; set; }

        public MssqlDatabaseManager(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        private void Open()
        {
            try
            {
                Connection = new SqlConnection(ConnectionString);

                Connection.Open();
            }
            catch (DataException ex)
            {
                if (OnError != null)
                    OnError(null, ex);
                else
                    throw;
                Close();
            }
        }

        private void Close()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }

        // executes stored procedure with DB parameteres if they are passed
        private object ExecuteProcedure(string procedureName, ExecuteType executeType, IEnumerable<DbQueryParameter> parameters)
        {
            object returnObject = null;

            if (Connection == null) return null;
            if (Connection.State != ConnectionState.Open) return null;

            Command = new SqlCommand(procedureName, Connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // pass stored procedure parameters to command
            if (parameters != null)
            {
                Command.Parameters.Clear();

                foreach (var dbParameter in parameters)
                {
                    var parameter = new SqlParameter
                    {
                        ParameterName = "@" + dbParameter.Name,
                        Direction = dbParameter.Direction
                    };
                    if (dbParameter.Value is DateTime && dbParameter.HasDateToFloatConversion)
                        parameter.Value = Convert.ToDateTime(dbParameter.Value).ToOADate();
                    else
                        parameter.Value = dbParameter.Value;

                    if (dbParameter.Value is DateTime && Convert.ToDateTime(dbParameter.Value) <= (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue)
                        parameter.Value = System.Data.SqlTypes.SqlDateTime.MinValue;

                    if (parameter.Value == null)
                        parameter.Value = DBNull.Value;

                    Command.Parameters.Add(parameter);
                }
            }

            switch (executeType)
            {
                case ExecuteType.ExecuteReader:
                    returnObject = Command.ExecuteReader();
                    break;
                case ExecuteType.ExecuteNonQuery:
                    returnObject = Command.ExecuteNonQuery();
                    break;
                case ExecuteType.ExecuteScalar:
                    returnObject = Command.ExecuteScalar();
                    break;
            }

            return returnObject;
        }

        // updates output parameters from stored procedure
        private void UpdateOutParameters()
        {
            if (Command.Parameters.Count <= 0) return;

            OutParameters = new List<DbQueryParameter>();
            OutParameters.Clear();

            for (var i = 0; i < Command.Parameters.Count; i++)
            {
                if (Command.Parameters[i].Direction == ParameterDirection.Output)
                {
                    OutParameters.Add(new DbQueryParameter(Command.Parameters[i].ParameterName,
                        Command.Parameters[i].Value,
                        ParameterDirection.Output));
                }
            }
        }

        // executes scalar query stored procedure without parameters
        public T ExecuteSingle<T>(string procedureName) where T : new()
        {
            return ExecuteSingle<T>(procedureName, null);
        }

        // executes scalar query stored procedure and maps result to single object
        public T ExecuteSingle<T>(string procedureName, List<DbQueryParameter> parameters) where T : new()
        {
            Open();
            var tempObject = new T();

            try
            {
                var reader = (IDataReader)ExecuteProcedure(procedureName, ExecuteType.ExecuteReader, parameters);


                if (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        SetObjectValue(reader.GetName(i), reader.GetValue(i), ref tempObject);
                    }
                }
                else
                {
                    tempObject = default(T);
                }
                reader.Close();
                UpdateOutParameters();
            }
            catch (Exception ex)
            {
                tempObject = default(T);
                if (OnError != null)
                    OnError(new { procedureName, parameters }, ex);
                else
                    throw;
            }



            Close();

            return tempObject;
        }

        public T ExecuteSingleV2<T>(string procedureName, List<DbQueryParameter> parameters)
        {
            Open();
            var tempObject = default(T);

            try
            {
                var reader = (IDataReader)ExecuteProcedure(procedureName, ExecuteType.ExecuteReader, parameters);
                if (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        SetObjectValue(reader.GetName(i), reader.GetValue(i), ref tempObject);
                    }
                }
                reader.Close();
                UpdateOutParameters();
            }
            catch (Exception ex)
            {
                if (OnError != null)
                    OnError(new { procedureName, parameters }, ex);
                else
                    throw;
            }
            Close();
            return tempObject;
        }

        // executes list query stored procedure without parameters
        public List<T> ExecuteList<T>(string procedureName) where T : new()
        {
            return ExecuteList<T>(procedureName, null);
        }

        // executes list query stored procedure and maps result generic list of objects
        public List<T> ExecuteList<T>(string procedureName, List<DbQueryParameter> parameters) where T : new()
        {
            var listObjects = new List<T>();
            Open();

            try
            {


                var reader = (IDataReader)ExecuteProcedure(procedureName, ExecuteType.ExecuteReader, parameters);
                while (reader.Read())
                {
                    var listItemObject = new T();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        SetObjectValue(reader.GetName(i), reader.GetValue(i), ref listItemObject);
                    }

                    listObjects.Add(listItemObject);
                }

                reader.Close();

                UpdateOutParameters();
            }
            catch (Exception ex)
            {
                if (OnError != null)
                    OnError(new { procedureName, parameters }, ex);
                else
                    throw;
            }
            Close();

            return listObjects;
        }

        public List<T> ExecuteListv2<T>(string procedureName, List<DbQueryParameter> parameters)
        {
            var listObjects = new List<T>();
            Open();

            try
            {
                var reader = (IDataReader)ExecuteProcedure(procedureName, ExecuteType.ExecuteReader, parameters);
                while (reader.Read())
                {
                    var listItemObject = default(T);

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        SetObjectValue(reader.GetName(i), reader.GetValue(i), ref listItemObject);
                    }

                    listObjects.Add(listItemObject);
                }

                reader.Close();

                UpdateOutParameters();
            }
            catch (Exception ex)
            {
                if (OnError != null)
                    OnError(new { procedureName, parameters }, ex);
                else
                    throw;
            }
            Close();

            return listObjects;
        }

        public Tuple<List<T1>, List<T2>> ExecuteMultiListv2<T1, T2>(string procedureName, List<DbQueryParameter> parameters)
            where T1 : new()
            where T2 : new()
        {
            Tuple<List<T1>, List<T2>> response = null;
            Open();

            try
            {
                var listObjects = new List<T1>();
                var reader = (IDataReader)ExecuteProcedure(procedureName, ExecuteType.ExecuteReader, parameters);
                while (reader.Read())
                {
                    var listItemObject = new T1();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        SetObjectValue(reader.GetName(i), reader.GetValue(i), ref listItemObject);
                    }

                    listObjects.Add(listItemObject);
                }

                var listObjects2 = new List<T2>();
                reader.NextResult();
                while (reader.Read())
                {
                    var listItemObject = new T2();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        SetObjectValue(reader.GetName(i), reader.GetValue(i), ref listItemObject);
                    }

                    listObjects2.Add(listItemObject);
                }

                reader.Close();

                response = Tuple.Create(listObjects, listObjects2);

                UpdateOutParameters();
            }
            catch (Exception ex)
            {
                if (OnError != null)
                    OnError(new { procedureName, parameters }, ex);
                else
                    throw;
            }
            Close();

            return response;
        }

        // executes non query stored procedure with parameters
        public int ExecuteNonQuery(string procedureName, List<DbQueryParameter> parameters)
        {
            Open();

            int returnValue = 0;
            try
            {
                returnValue = (int)ExecuteProcedure(procedureName, ExecuteType.ExecuteNonQuery, parameters);
            }
            catch (Exception ex)
            {
                if (OnError != null)
                    OnError(new { procedureName, parameters }, ex);
                else
                    throw;
            }

            UpdateOutParameters();

            Close();

            return returnValue;
        }

        private void SetObjectValue<T>(string name, object value, ref T itemObject)
        {
            if (value == DBNull.Value) return;
            var typeofT = typeof(T);
            if (typeofT.IsValueType || typeofT.FullName == "System.String")
            {
                if (typeof(T).GetInterface("IConvertible") == typeof(IConvertible))
                    itemObject = (T)Convert.ChangeType(value, typeofT);
                else
                    itemObject = (T)value;
            }
            else
            {
                if (name == "TotalRows")
                {
                    TotalRows = Convert.ToInt32(value);
                    return;
                }

                if (name.Contains("."))
                {
                    string[] propertyNames = name.Split('.');
                    PropertyInfo nestedObjectProperty = typeofT.GetProperty(propertyNames[0]);
                    if (nestedObjectProperty != null && nestedObjectProperty.PropertyType.IsClass)
                    {
                        object subObjectInstance = nestedObjectProperty.GetValue(itemObject) ??
                                                   Activator.CreateInstance(nestedObjectProperty.PropertyType);

                        PropertyInfo propertyInfoOfNestedObject =
                            subObjectInstance.GetType().GetProperty(propertyNames[1]);
                        if (propertyInfoOfNestedObject.PropertyType == typeof(DateTime) && value is Double)
                        {
                            value = DateTime.FromOADate(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                        }
                        if (propertyInfoOfNestedObject.PropertyType == typeof(TimeSpan))
                        {
                            value = TimeSpan.Parse(value.ToString(), CultureInfo.InvariantCulture);
                        }

                        if (propertyInfoOfNestedObject.CanWrite)
                            propertyInfoOfNestedObject.SetValue(subObjectInstance, value, null);

                        if (nestedObjectProperty.CanWrite)
                            nestedObjectProperty.SetValue(itemObject, subObjectInstance, null);

                    }
                }
                else
                {
                    var propertyInfo = typeofT.GetProperty(name);
                    if (propertyInfo == null) return;
                    if (propertyInfo.PropertyType == typeof(DateTime) && value is Double)
                    {
                        value = DateTime.FromOADate(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                    }
                    if (propertyInfo.PropertyType == typeof(TimeSpan))
                    {
                        value = TimeSpan.Parse(value.ToString(), CultureInfo.InvariantCulture);
                    }

                    if (propertyInfo.CanWrite)
                        propertyInfo.SetValue(itemObject, value, null);
                }
            }
        }

        public DataTable ExecuteQuery(string query)
        {
            Open();

            var sqlDataAdapter = new SqlDataAdapter(query, ConnectionString);
            var dt = new DataTable();
            sqlDataAdapter.Fill(dt);
            sqlDataAdapter.Dispose();
            Close();
            return dt;
        }
    }
}
