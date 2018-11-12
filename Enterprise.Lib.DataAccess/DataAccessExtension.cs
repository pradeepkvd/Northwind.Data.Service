﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;

namespace Enterprise.Lib.DataAccess
{

    public static class DataAccessExtension
    {
        public static T Insert<T>(this IDbConnection cnn, string tableName, dynamic param)
        {
            IEnumerable<T> result = SqlMapper.Query<T>(cnn, DynamicQuery.GetInsertQuery(tableName, param), param);
            return result.First();
        }

        public static void Update(this IDbConnection cnn, string tableName, dynamic param)
        {
            SqlMapper.Execute(cnn, DynamicQuery.GetUpdateQuery(tableName, param), param);
        }

        //private static string connectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;

        ///// <summary>
        ///// Gets the open connection.
        ///// </summary>
        ///// <param name="name">The name of the connection string (optional).</param>
        ///// <returns></returns>
        //public static SqlConnection GetOpenConnection(string name = null)
        //{
        //    string connString = "";
        //    connString = name == null ? connString = ConfigurationManager.ConnectionStrings[0].ConnectionString : connString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
        //    var connection = new SqlConnection(connString);
        //    connection.Open();
        //    return connection;
        //}

        public static int InsertMultiple<T>(this IDbConnection cnn, string sql, IEnumerable<T> entities, string connectionName = null)
            where T : class, new()
        {
            int records = 0;

            foreach (T entity in entities)
            {
                records += cnn.Execute(sql, entity);
            }
            return records;
        }

        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in list)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                table.Rows.Add(values);
            }
            return table;
        }

        public static DynamicParameters GetParametersFromObject(object obj, string[] propertyNamesToIgnore)
        {
            if (propertyNamesToIgnore == null) propertyNamesToIgnore = new string[] { String.Empty };
            DynamicParameters p = new DynamicParameters();
            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in properties)
            {
                if (!propertyNamesToIgnore.Contains(prop.Name))
                    p.Add("@" + prop.Name, prop.GetValue(obj, null));
            }
            return p;
        }

        public static void SetIdentity<T>(IDbConnection connection, Action<T> setId)
        {
            dynamic identity = connection.Query("SELECT @@IDENTITY AS Id").Single();
            T newId = (T)identity.Id;
            setId(newId);
        }


        public static object GetPropertyValue(object target, string propertyName)
        {
            PropertyInfo[] properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            object theValue = null;
            foreach (PropertyInfo prop in properties)
            {
                if (string.Compare(prop.Name, propertyName, true) == 0)
                {
                    theValue = prop.GetValue(target, null);
                }
            }
            return theValue;
        }

        public static void SetPropertyValue(object p, string propName, object value)
        {
            Type t = p.GetType();
            PropertyInfo info = t.GetProperty(propName);
            if (info == null)
                return;
            if (!info.CanWrite)
                return;
            info.SetValue(p, value, null);
        }

        /// <summary>
        /// Stored proc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procname">The procname.</param>
        /// <param name="parms">The parms.</param>
        /// <returns></returns>
        public static List<T> StoredProcWithParams<T>(this IDbConnection cnn, string procname, dynamic parms, string connectionName = null)
        {
            
                return cnn.Query<T>(procname, (object)parms, commandType: CommandType.StoredProcedure).ToList();
    

        }


        /// <summary>
        /// Stored proc with params returning dynamic.
        /// </summary>
        /// <param name="procname">The procname.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public static List<dynamic> StoredProcWithParamsDynamic(this IDbConnection cnn, string procname, dynamic parms,
            string connectionName = null)
        {
            
                return cnn.Query(procname, (object)parms, commandType: CommandType.StoredProcedure).ToList();
          
        }

        /// <summary>
        /// Stored proc insert with ID.
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <typeparam name="U">The Type of the ID</typeparam>
        /// <param name="procName">Name of the proc.</param>
        /// <param name="parms">instance of DynamicParameters class. This
        /// should include defined output parameter</param>

        /// <returns>U - the @@Identity value from output parameter</returns>
        public static U StoredProcInsertWithID<T, U>(this IDbConnection cnn, string procName, DynamicParameters parms,
            string connectionName = null)
        {
            
                var x = cnn.Execute(procName, (object)parms, commandType: CommandType.StoredProcedure);
                return parms.Get<U>("@ID");
        }


        /// <summary>
        /// SQL with params.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <returns></returns>
        public static List<T> SqlWithParams<T>(this IDbConnection cnn, string sql, dynamic parms, string connectionnName = null)
        {
            
                return cnn.Query<T>(sql, (object)parms).ToList();
        }

        /// <summary>
        /// Insert update or delete SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <returns></returns>
        public static int InsertUpdateOrDeleteSql(this IDbConnection cnn, string sql, dynamic parms, string connectionName = null)
        {
           
                return cnn.Execute(sql, (object)parms);
        }

        /// <summary>
        /// Insert update or delete stored proc.
        /// </summary>
        /// <param name="procName">Name of the proc.</param>
        /// <param name="parms">The parms.</param>
        /// <returns></returns>
        public static int InsertUpdateOrDeleteStoredProc(this IDbConnection cnn, string procName, dynamic parms, string connectionName = null)
        {
           
                return cnn.Execute(procName, (object)parms, commandType: CommandType.StoredProcedure);
           
        }

        /// <summary>
        /// SQLs the with params single.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public static T SqlWithParamsSingle<T>(this IDbConnection cnn, string sql, dynamic parms, string connectionName = null)
        {

            return cnn.Query<T>(sql, (object)parms).FirstOrDefault();
        }

        /// <summary>
        ///  proc with params single returning Dynamic object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public static System.Dynamic.DynamicObject DynamicProcWithParamsSingle<T>(this IDbConnection cnn, string sql, dynamic parms,
            string connectionName = null)
        {
           
                return cnn.Query(sql, (object)parms, commandType: CommandType.StoredProcedure).FirstOrDefault();
           
        }

        /// <summary>
        /// proc with params returning Dynamic.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public static IEnumerable<dynamic> DynamicProcWithParams<T>(this IDbConnection cnn, string sql, dynamic parms,
            string connectionName = null)
        {
           
                return cnn.Query(sql, (object)parms, commandType: CommandType.StoredProcedure);
            
        }


        /// <summary>
        /// Stored proc with params returning single.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procname">The procname.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public static T StoredProcWithParamsSingle<T>(this IDbConnection cnn, string procname, dynamic parms, string connectionName = null)
        {
              return
                    cnn.Query<T>(procname, (object)parms, commandType: CommandType.StoredProcedure)
                        .SingleOrDefault();
        }

    }
}
