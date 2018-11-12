using Enterprise.Lib.Core.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Enterprise.Lib.DataAccess
{
    /// <summary>
    /// Dynamic query class.
    /// </summary>
    public sealed class DynamicQuery
    {
        public static string GetScript(
            IEnumerable<DBScript> queries,
           Type associatedObjectType,
            ScriptType scriptType = ScriptType.GetAll,
            dynamic item = null,
            string idPropertyName = "Id",
            string dbIdColumnName = "Id")
        {
            if (associatedObjectType == null)
                throw new ArgumentNullException(nameof(associatedObjectType));

            if (queries == null || !queries.Any())
                throw new ArgumentNullException(nameof(queries));

            var query = queries.FirstOrDefault(q => q.AssociatedObjectType == associatedObjectType && q.Type == scriptType);

            if (query == null)
                return string.Empty;

            //Get property from object to identify columns and values
            PropertyInfo[] props = item?.GetType().GetProperties();
            string[] columns = null;

            //prepare scripts as per query
            var script = string.Empty;
            switch (scriptType)
            {
                case ScriptType.Add:
                    if (props == null) break;
                    columns = props.Select(p => p.Name).Where(s => s != idPropertyName).ToArray();
                    return $"{query.Script} ({string.Join(",", columns)}) OUTPUT inserted.{dbIdColumnName} VALUES (@{string.Join(",@", columns)})";

                case ScriptType.Update:
                    if (props == null) break;
                    columns = props.Select(p => p.Name).ToArray();
                    IEnumerable<string> parameters = columns.Select(name => name + "=@" + name).ToList();
                    return $"{query.Script} SET {string.Join(",", parameters)} WHERE {dbIdColumnName}=@{idPropertyName}";

                case ScriptType.NA:
                    break;

                case ScriptType.Delete:
                    break;

                case ScriptType.GetById:
                    return $"{query.Script}";

                case ScriptType.GetAll:
                    return $"{query.Script}";

                default:
                    return $"{query.Script}";
            }

            return query.Script;
        }

        /// <summary>
        /// Gets the insert query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The Sql query based on the item properties.
        /// </returns>
        public static string GetInsertQuery(string tableName, dynamic item, string idPropertyName = "Id", string dbIdColumnName = "Id")
        {
            PropertyInfo[] properties = item.GetType().GetProperties();
            var mappedProps = properties.Where(a => !a.IsNotMapped());
            var columns = mappedProps.Select(p => p.Name).Where(s => s != idPropertyName).ToArray();

            return
                $"INSERT INTO {tableName} ({string.Join(",", columns)}) OUTPUT inserted.{dbIdColumnName} VALUES (@{string.Join(",@", columns)})";
        }

        /// <summary>
        /// Gets the update query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The Sql query based on the item properties.
        /// </returns>
        public static string GetUpdateQuery(string tableName, dynamic item, string idPropertyName = "Id", string dbIdColumnName = "Id")
        {
            PropertyInfo[] properties = item.GetType().GetProperties();
            var mappedProps = properties.Where(a => !a.IsNotMapped());
            var columns = mappedProps.Select(p => p.Name).ToArray();

            var parameters = columns.Select(name => name + "=@" + name).ToList();

            return $"UPDATE {tableName} SET {string.Join(",", parameters)} WHERE {dbIdColumnName}=@{idPropertyName}";
        }

        /// <summary>
        /// Gets the where query.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="populateValues">actual values to be inserted in Query instead of FIELDNAME=@VARIABLE</param>
        /// <returns>The where clause of a query from an anonymous type.</returns>
        public static string GetWhereQuery(object item, bool populateValues = false)
        {
            PropertyInfo[] properties = item.GetType().GetProperties();
            var mappedProps = properties.Where(a => !a.IsNotMapped());
            string[] columns = mappedProps.Select(p => p.Name).ToArray();

            var builder = new StringBuilder();

            for (var i = 0; i < columns.Count(); i++)
            {
                string col = columns[i];

                var colVal = item.GetType().GetProperty(col).GetValue(item, null);

                if (
                    populateValues &&
                    (
                        colVal == null ||
                        (colVal is string && string.IsNullOrEmpty(((string)colVal).Trim())) ||
                        (colVal is Guid && ((Guid)colVal) == Guid.Empty)
                    )
                  ) continue;

                if (i > 0)
                {
                    builder.Append(" AND ");
                }

                builder.Append(col);

                if (!populateValues)
                {
                    builder.Append("=@");
                    builder.Append(col);
                }
                else
                {
                    builder.Append("=");
                    builder.Append($"'{colVal}'");
                }

                //if (i < columns.Count() - 1)
                //{
                //    builder.Append(" AND ");
                //}
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the dynamic query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A result object with the generated sql and dynamic params.</returns>
        public static QueryResult GetDynamicQuery<T>(string tableName, Expression<Func<T, bool>> expression)
        {
            var queryProperties = new List<QueryParameter>();
            var body = expression.Body as BinaryExpression;
            IDictionary<string, object> expando = new ExpandoObject();
            var builder = new StringBuilder();

            // walk the tree and build up a list of query parameter objects
            // from the left and right branches of the expression tree
            WalkTree(body, ExpressionType.Default, ref queryProperties);

            // convert the query parms into a SQL string and dynamic property object
            builder.Append("SELECT * FROM ");
            builder.Append(tableName);
            builder.Append(" WHERE ");

            for (int i = 0; i < queryProperties.Count(); i++)
            {
                QueryParameter item = queryProperties[i];

                if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                {
                    builder.Append(string.Format("{0} {1} {2} @{1} ", item.LinkingOperator, item.PropertyName,
                                                 item.QueryOperator));
                }
                else
                {
                    builder.Append(string.Format("{0} {1} @{0} ", item.PropertyName, item.QueryOperator));
                }

                expando[item.PropertyName] = item.PropertyValue;
            }

            return new QueryResult(builder.ToString().TrimEnd(), expando);
        }

        public static QueryResult GetDynamicQueryWhere<T>(Expression<Func<T, bool>> expression)
        {
            var queryProperties = new List<QueryParameter>();
            var body = expression.Body as BinaryExpression;
            IDictionary<string, object> expando = new ExpandoObject();
            var builder = new StringBuilder();

            // walk the tree and build up a list of query parameter objects
            // from the left and right branches of the expression tree
            WalkTree(body, ExpressionType.Default, ref queryProperties);

            // convert the query parms into a SQL string and dynamic property object
            builder.Append("$$QUERY$$");
            builder.Append(" WHERE ");

            for (int i = 0; i < queryProperties.Count(); i++)
            {
                QueryParameter item = queryProperties[i];
                var queryOperator = !string.IsNullOrEmpty(item.PropertyValue?.ToString()) && item.PropertyValue.ToString().Contains("%") ? "like" : item.QueryOperator;

                if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                {
                    builder.Append(string.Format("{0} {1} {2} @{1} ", item.LinkingOperator, item.PropertyName,
                                                 queryOperator));
                }
                else
                {
                    builder.Append(string.Format("{0} {1} @{0} ", item.PropertyName, queryOperator));
                }

                expando[item.PropertyName] = item.PropertyValue;
            }

            return new QueryResult(builder.ToString().TrimEnd(), expando);
        }

        public static QueryResult GetDynamicQueryWhereWithValues<T>(Expression<Func<T, bool>> expression)
        {
            var queryProperties = new List<QueryParameter>();
            var body = expression.Body as BinaryExpression;
            IDictionary<string, object> expando = new ExpandoObject();
            var builder = new StringBuilder();

            // walk the tree and build up a list of query parameter objects
            // from the left and right branches of the expression tree
            WalkTree(body, ExpressionType.Default, ref queryProperties);

            // convert the query parms into a SQL string and dynamic property object
            builder.Append("$$QUERY$$");
            builder.Append(" WHERE ");

            for (int i = 0; i < queryProperties.Count(); i++)
            {
                QueryParameter item = queryProperties[i];

                var queryOperator = !string.IsNullOrEmpty(item.PropertyValue?.ToString()) && item.PropertyValue.ToString().Contains("%") ? "like" : item.QueryOperator;

                if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                {
                   
                    builder.Append(string.Format("{0} {1} {2} '{3}' ", item.LinkingOperator, item.PropertyName, queryOperator, item.PropertyValue ?? "NULL"));
                }
                else
                {
                    builder.Append(string.Format("{0} {1} '{2}' ", item.PropertyName, queryOperator, item.PropertyValue ?? "NULL"));
                }

                expando[item.PropertyName] = item.PropertyValue;
            }

            return new QueryResult(builder.ToString().TrimEnd(), expando);
        }

        /// <summary>
        /// Walks the tree.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="linkingType">Type of the linking.</param>
        /// <param name="queryProperties">The query properties.</param>
        private static void WalkTree(BinaryExpression body, ExpressionType linkingType,
                                     ref List<QueryParameter> queryProperties)
        {
            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {
                string propertyName = GetPropertyName(body);
                dynamic propertyValue = body.Right;
                string opr = GetOperator(body.NodeType);
                string link = GetOperator(linkingType);

                queryProperties.Add(new QueryParameter(link, propertyName, propertyValue.Value, opr));
            }
            else
            {
                WalkTree((BinaryExpression)body.Left, body.NodeType, ref queryProperties);
                WalkTree((BinaryExpression)body.Right, body.NodeType, ref queryProperties);
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns>The property name for the property expression.</returns>
        private static string GetPropertyName(BinaryExpression body)
        {
            string propertyName = body.Left.ToString().Split(new char[] { '.' })[1];

            if (body.Left.NodeType == ExpressionType.Convert)
            {
                // hack to remove the trailing ) when convering.
                propertyName = propertyName.Replace(")", string.Empty);
            }

            return propertyName;
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The expression types SQL server equivalent operator.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private static string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                    return "=";

                case ExpressionType.NotEqual:
                    return "!=";

                case ExpressionType.LessThan:
                    return "<";

                case ExpressionType.GreaterThan:
                    return ">";

                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    return "AND";

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "OR";

                case ExpressionType.Default:
                    return string.Empty;

                default:
                    throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Class that models the data structure in coverting the expression tree into SQL and Params.
    /// </summary>
    internal class QueryParameter
    {
        public string LinkingOperator { get; set; }
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
        public string QueryOperator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter" /> class.
        /// </summary>
        /// <param name="linkingOperator">The linking operator.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="queryOperator">The query operator.</param>
        internal QueryParameter(string linkingOperator, string propertyName, object propertyValue, string queryOperator)
        {
            this.LinkingOperator = linkingOperator;
            this.PropertyName = propertyName;
            this.PropertyValue = propertyValue;
            this.QueryOperator = queryOperator;
        }
    }

    /// <summary>
    /// A result object with the generated sql and dynamic params.
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// The _result
        /// </summary>
        private readonly Tuple<string, dynamic> _result;

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <value>
        /// The SQL.
        /// </value>
        public string Sql
        {
            get
            {
                return _result.Item1;
            }
        }

        /// <summary>
        /// Gets the param.
        /// </summary>
        /// <value>
        /// The param.
        /// </value>
        public dynamic Param
        {
            get
            {
                return _result.Item2;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult" /> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The param.</param>
        public QueryResult(string sql, dynamic param)
        {
            _result = new Tuple<string, dynamic>(sql, param);
        }
    }

    public enum ScriptType
    {
        [Description("None")]
        NA,

        [Description("Add")]
        Add,

        [Description("Update")]
        Update,

        [Description("Delete")]
        Delete,

        [Description("GetSingle")]
        GetById,

        [Description("GetAll")]
        GetAll
    }

    public class DBScript
    {
        public Type AssociatedObjectType { get; set; }

        public string Script { get; set; }

        public ScriptType Type { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Params { get; set; }
    }
}