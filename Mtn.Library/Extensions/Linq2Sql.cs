using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Data.Linq;
using System.Data;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Linq.Expressions;
using System.Globalization;
using System.Runtime.Serialization;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.Linq.Mapping;
using Mtn.Library.Extensions;
namespace Mtn.Library.ExtensionsLinq2Sql
{
    /// <summary>
    /// Linq to Sql extensions
    /// </summary>
    public static class Linq2Sql
    {
        #region Linq2SQL

        #region Expand
       
        /// <summary>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="query">
        /// <para>Query.</para>
        /// </param>
        /// <returns>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </returns>
        public static IQueryable<T> AsExpandableMtn<T>(this IQueryable<T> query)
        {
            if (query is ExpandableQuery<T>)
                return (ExpandableQuery<T>)query;
            return new ExpandableQuery<T>(query);
        }

        /// <summary>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </summary>
        /// <typeparam name="TDelegate">
        /// <para>Type of Delegate.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <returns>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </returns>
        public static Expression<TDelegate> ExpandMtn<TDelegate>(this Expression<TDelegate> expr)
        {
            return (Expression<TDelegate>)new ExpressionExpander().Visit(expr);
        }
    
        /// <summary>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </summary>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <returns>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </returns>
        public static Expression ExpandMtn(this Expression expr)
        {
            return new ExpressionExpander().Visit(expr);
        }

        #endregion

        #region Invoke
       
        /// <summary>
        /// <para>Extends the Expression class to invoke the lambda expression while making it still possible to translate query to T-SQL</para>
        /// </summary>
        /// <typeparam name="TResult">
        /// <para>Result class.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a value of the type specified by the TResult parameter.</para>
        /// </returns>
        internal static TResult InvokeMtn<TResult>(this Expression<Func<TResult>> expr)
        {
            return expr.Compile().Invoke();
        }
       
        /// <summary>
        /// <para>Extends the Expression class to invoke the lambda expression while making it still possible to translate query to T-SQL</para>
        /// See http://tomasp.net/blog/linq-expand.aspx for information on how it's used.
        /// </summary>
        /// <typeparam name="T1">
        /// <para>The type of the parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="TResult">
        /// <para>The type of the return value of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <param name="arg1">
        /// <para>The parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a value of the type specified by the TResult parameter.</para>
        /// </returns>
        internal static TResult InvokeMtn<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1)
        {
            return expr.Compile().Invoke(arg1);
        }
       
        /// <summary>
        /// <para>Extends the Expression class to invoke the lambda expression while making it still possible to translate query to T-SQL</para>
        /// See http://tomasp.net/blog/linq-expand.aspx for information on how it's used.
        /// </summary>
        /// <typeparam name="T1">
        /// <para>The type of the first parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T2">
        /// <para>The type of the second parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="TResult">
        /// <para>The type of the return value of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <param name="arg1">
        /// <para>The first parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg2">
        /// <para>The second parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a value of the type specified by the TResult parameter.</para>
        /// </returns>
        internal static TResult InvokeMtn<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2)
        {
            return expr.Compile().Invoke(arg1, arg2);
        }
        
        /// <summary>
        /// <para>Extends the Expression class to invoke the lambda expression while making it still possible to translate query to T-SQL</para>
        /// See http://tomasp.net/blog/linq-expand.aspx for information on how it's used.
        /// </summary>
        /// <typeparam name="T1">
        /// <para>The type of the first parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T2">
        /// <para>The type of the second parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T3">
        /// <para>The type of the third parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="TResult">
        /// <para>The type of the return value of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <param name="arg1">
        /// <para>The first parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg2">
        /// <para>The second parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg3">
        /// <para>The third parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a value of the type specified by the TResult parameter.</para>
        /// </returns>
        internal static TResult InvokeMtn<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3);
        }
       
        /// <summary>
        /// <para>Extends the Expression class to invoke the lambda expression while making it still possible to translate query to T-SQL</para>
        /// See http://tomasp.net/blog/linq-expand.aspx for information on how it's used.
        /// </summary>
        /// <typeparam name="T1">
        /// <para>The type of the first parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T2">
        /// <para>The type of the second parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T3">
        /// <para>The type of the third parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T4">
        /// <para>The type of the fourth parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="TResult">
        /// <para>The type of the return value of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <param name="arg1">
        /// <para>The first parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg2">
        /// <para>The second parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg3">
        /// <para>The third parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg4">
        /// <para>The fourth parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a value of the type specified by the TResult parameter.</para>
        /// </returns>
        internal static TResult InvokeMtn<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4);
        }

        #endregion

        #region ForEach

        /// <summary>
        /// <para>Through each element in source and performs the action method as parameter passing this element.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Type of IEnumerable.</para>
        /// </typeparam>
        /// <param name="source">
        /// <para>IEnumerable to be traversed.</para>
        /// </param>
        /// <param name="action">
        /// <para>Action to be executed.</para>
        /// </param>
        public static void ForEachMtn<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
        #endregion

        #region ToSqlString
        /// <summary>
        /// <para>Returns the query made in the database for the Entity Framework</para>
        /// </summary>
        /// <param name="query">
        /// <para>Query.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the query made in the database for the Entity Framework</para>
        /// </returns>
        public static String ToSqlStringMtn(this IQueryable query)
        {
            return (query as System.Data.Objects.ObjectQuery).ToTraceString();
        }
        #endregion

        #region DumpCSV

        /// <summary>
        /// <para>Creates a *.csv file from an IQueryable query, dumping out the 'simple' properties/fields.</para>
        /// </summary>
        /// <param name="query">
        /// <para>Represents a SELECT query to execute.</para>
        /// </param>
        /// <param name="fileName">
        /// <para>The name of the file to create.</para>
        /// </param>
        /// <param name="deleteFile">
        /// <para>Whether or not to delete the file specified by <paramref name="fileName"/> if it exists.</para>
        /// </param>
        /// <param name="delimiter">
        /// <para>Another delimiter to replace the default (,) .</para>
        /// </param>
        /// <param name="createHeader">
        /// <para>Whether or not insert header (property name) on file.</para>
        /// </param>
        /// <param name="encoding"></param>
        /// <remarks>
        /// <para>If the <paramref name="query"/> contains any properties that are entity sets (i.e. rows from a FK relationship) the values will not be dumped to the file.</para>
        /// <para>This method is useful for debugging purposes or when used in other utilities such as LINQPad.</para>
        /// </remarks>
        public static void DumpCsvMtn(this IQueryable query, String fileName, Boolean deleteFile = true, String delimiter = ",", Boolean createHeader = true, Encoding encoding = null)
        {
        
            if (delimiter.IsNullOrEmptyMtn(true))
                delimiter = ",";

            if (File.Exists(fileName) && deleteFile)
            {
                File.Delete(fileName);
            }

            if (encoding == null)
                encoding = new UTF8Encoding();

            using (var output = new FileStream(fileName, FileMode.CreateNew))
            {
                using (var writer = new StreamWriter(output, encoding))
                {
                    var firstRow = true;

                    PropertyInfo[] properties = null;
                    Type type = null;

                    foreach (var r in query)
                    {
                        if (type == null)
                        {
                            type = r.GetType();
                            properties = type.GetProperties();
                        }

                        var firstCol = true;


                        if (createHeader)
                        {
                            if (firstRow)
                            {
                                foreach (var p in properties)
                                {
                                    if (!firstCol)
                                        writer.Write(",");
                                    else { firstCol = false; }

                                    writer.Write(p.Name);
                                }
                                writer.WriteLine();
                            }
                        }
                        firstRow = false;
                        firstCol = true;

                        foreach (var p in properties)
                        {
                            if (!firstCol)
                                writer.Write(delimiter);
                            else { firstCol = false; }
                            DumpValueMtn(p.GetValue(r, null), writer, delimiter);
                        }
                        writer.WriteLine();
                    }
                }
            }
        }
        #endregion

        #region DumpValue

        private static void DumpValueMtn(object v, StreamWriter writer, string delimiter)
        {
            if (v != null)
            {
                switch (Type.GetTypeCode(v.GetType()))
                {
                    // csv encode the value
                    case TypeCode.String:
                        string value = (string)v;
                        if (value.Contains(delimiter) || value.Contains('"') || value.Contains("\n"))
                        {
                            value = value.Replace("\"", "\"\"");

                            if (value.Length > 31735)
                            {
                                value = value.Substring(0, 31732) + "...";
                            }
                            writer.Write("\"" + value + "\"");
                        }
                        else
                        {
                            writer.Write(value);
                        }
                        break;

                    default:
                        writer.Write(v);
                        break;
                }
            }
        }
        #endregion

        #region IsAnonymous
       
        private static Boolean IsAnonymousMtn(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                       && type.IsGenericType && type.Name.Contains("AnonymousType")
                       && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                       && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;

        }
        #endregion

        #region SelectMultipleResults
  
        /// <summary>
        /// <para>Batches together multiple IQueryable queries into a single DbCommand and returns all data in a single roundtrip to the database.</para>
        /// </summary>
        /// <param name="context">
        /// <para>The DataContext to execute the batch select against.</para>
        /// </param>
        /// <param name="queries">
        /// <para>Represents a collections of SELECT queries to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to be used.</para>
        /// </param>
        /// <returns>
        /// <para>Returns an IMultipleResults object containing all results.</para>
        /// </returns>
        public static IMultipleResults SelectMultipleResultsMtn(this DataContext context, IQueryable[] queries, DbTransaction transaction = null)
        {
            var commandList = new List<DbCommand>();
            var ctx = context.NewDataContextMtn(transaction);
            //List<object> lstResults = new List<object>();
            foreach (IQueryable query in queries)
            {
                var command = context.GetCommand(query);
                commandList.Add(command);
            }

            SqlCommand batchCommand = CombineCommandsMtn(commandList);
            batchCommand.Connection = ctx.Connection as SqlConnection;

            DbDataReader dr = null;

            if (batchCommand.Connection.State == ConnectionState.Closed)
            {
                batchCommand.Connection.Open();
                dr = batchCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            else
            {
                dr = batchCommand.ExecuteReader();
            }

            IMultipleResults mr = ctx.Translate(dr);
            return mr;
        }

        /// <summary>
        /// <para>Combines multiple SELECT commands into a single SqlCommand so that all statements can be executed in a single roundtrip to the database and return multiple result sets.</para>
        /// </summary>
        /// <param name="selectCommands">Represents a collection of commands to be batched together.</param>
        /// <returns>Returns a single SqlCommand that executes all SELECT statements at once.</returns>
        private static SqlCommand CombineCommandsMtn(IEnumerable<DbCommand> selectCommands)
        {
            var batchCommand = new SqlCommand();
            var newParamList = batchCommand.Parameters;

            int commandCount = 0;

            foreach (var cmd in selectCommands)
            {
                string commandText = cmd.CommandText;
                var paramList = cmd.Parameters;
                int paramCount = paramList.Count;

                for (int currentParam = paramCount - 1; currentParam >= 0; currentParam--)
                {
                    var param = paramList[currentParam];
                    var newParam = CloneParameterMtn(param);
                    string newParamName = param.ParameterName.Replace("@", string.Format("@{0}_", commandCount));
                    commandText = commandText.Replace(param.ParameterName, newParamName);
                    newParam.ParameterName = newParamName;
                    newParamList.Add(newParam);
                }
                if (batchCommand.CommandText.Length > 0)
                {
                    batchCommand.CommandText += ";";
                }
                batchCommand.CommandText += commandText;
                commandCount++;
            }

            return batchCommand;
        }


        /// <summary>
        /// Returns a clone (via copying all properties) of an existing DbParameter.
        /// </summary>
        /// <param name="src">The DbParameter to clone.</param>
        /// <returns>Returns a clone (via copying all properties) of an existing DbParameter.</returns>
        private static DbParameter CloneParameterMtn(DbParameter src)
        {
            SqlParameter source = (SqlParameter)src;
            SqlParameter destination = new SqlParameter();

            destination.Value = source.Value;
            destination.Direction = source.Direction;
            destination.Size = source.Size;
            destination.Offset = source.Offset;
            destination.SourceColumn = source.SourceColumn;
            destination.SourceVersion = source.SourceVersion;
            destination.SourceColumnNullMapping = source.SourceColumnNullMapping;
            destination.IsNullable = source.IsNullable;

            destination.CompareInfo = source.CompareInfo;
            destination.XmlSchemaCollectionDatabase = source.XmlSchemaCollectionDatabase;
            destination.XmlSchemaCollectionOwningSchema = source.XmlSchemaCollectionOwningSchema;
            destination.XmlSchemaCollectionName = source.XmlSchemaCollectionName;
            destination.UdtTypeName = source.UdtTypeName;
            destination.TypeName = source.TypeName;
            destination.ParameterName = source.ParameterName;
            destination.Precision = source.Precision;
            destination.Scale = source.Scale;

            return destination;
        }
        #endregion

        #region Delete

        /// <summary>
        /// <para>Immediately deletes all entities from the collection with a single delete command.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for rows contained in <paramref name="table"/>.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database containing rows are to be deleted.</para>
        /// </param>
        /// <param name="primaryKey">
        /// <para>Represents the primary key of the item to be removed from <paramref name="table"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to be used.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows deleted from the database (maximum of 1).</para>
        /// </returns>
        /// <remarks>
        /// <para>If the primary key for <paramref name="table"/> is a composite key, <paramref name="primaryKey"/> should be an anonymous type with property names mapping to the property names of objects of type <typeparamref name="TEntity"/>.</para>
        /// </remarks>
        public static Int32 DeleteByPkMtn<TEntity>(this Table<TEntity> table, Object primaryKey, DbTransaction transaction = null) where TEntity : class
        {
            DbCommand delete = table.GetDeleteByPkCommandMtn<TEntity>(primaryKey);

            var parameters = from p in delete.Parameters.Cast<DbParameter>()
                             select p.Value;

            var context = table.NewDataContextMtn(transaction);

            int retVal = context.ExecuteCommand(delete.CommandText, parameters.ToArray());

            context.Dispose();
            return retVal;
        }

        /// <summary>
        /// <para>Returns a string representation the LINQ  command text and parameters used that would be issued to delete a entity row via the supplied primary key.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for rows contained in <paramref name="table"/>.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database containing rows are to be deleted.</para>
        /// </param>
        /// <param name="primaryKey">
        /// <para>Represents the primary key of the item to be removed from <paramref name="table"/>.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a string representation the LINQ  command text and parameters used that would be issued to delete a entity row via the supplied primary key.</para>
        /// </returns>
        /// <remarks>
        /// <para>This method is useful for debugging purposes or when used in other utilities such as LINQPad.</para>
        /// </remarks>
        public static String DeleteByPkPreviewMtn<TEntity>(this Table<TEntity> table, Object primaryKey) where TEntity : class
        {
            DbCommand delete = table.GetDeleteByPkCommandMtn(primaryKey);
            return delete.PreviewCommandTextMtn(false) + table.Context.GetLogMtn();
        }

        private static DbCommand GetDeleteByPkCommandMtn<TEntity>(this Table<TEntity> table, Object primaryKey) where TEntity : class
        {
            Type type = primaryKey.GetType();
            bool typeIsAnonymous = type.IsAnonymousMtn();
            string dbName = table.GetDbNameMtn();

            var metaTable = table.Context.Mapping.GetTable(typeof(TEntity));

            var keys = from mdm in metaTable.RowType.DataMembers
                       where mdm.IsPrimaryKey
                       select new { mdm.MappedName, mdm.Name, mdm.Type };

            SqlCommand deleteCommand = new SqlCommand();
            deleteCommand.Connection = table.Context.Connection as SqlConnection;

            var whereSb = new StringBuilder();

            foreach (var key in keys)
            {
                // Add new parameter with massaged name to avoid clashes.
                whereSb.AppendFormat("[{0}] = @p{1}, ", key.MappedName, deleteCommand.Parameters.Count);

                object value = primaryKey;
                if (typeIsAnonymous || (type.IsClass && type != typeof(string)))
                {
                    if (typeIsAnonymous)
                    {
                        PropertyInfo property = type.GetProperty(key.Name);

                        if (property == null)
                        {
                            throw new ArgumentOutOfRangeException(string.Format("The property {0} which is defined as part of the primary key for {1} was not supplied by the parameter primaryKey.", key.Name, metaTable.TableName));
                        }

                        value = property.GetValue(primaryKey, null);
                    }
                    else
                    {
                        FieldInfo field = type.GetField(key.Name);

                        if (field == null)
                        {
                            throw new ArgumentOutOfRangeException(string.Format("The property {0} which is defined as part of the primary key for {1} was not supplied by the parameter primaryKey.", key.Name, metaTable.TableName));
                        }

                        value = field.GetValue(primaryKey);
                    }

                    if (value.GetType() != key.Type)
                    {
                        throw new InvalidCastException(string.Format("The property {0} ({1}) does not have the same type as {2} ({3}).", key.Name, value.GetType(), key.MappedName, key.Type));
                    }
                }
                else if (value.GetType() != key.Type)
                {
                    throw new InvalidCastException(string.Format("The value supplied in primaryKey ({0}) does not have the same type as {1} ({2}).", value.GetType(), key.MappedName, key.Type));
                }

                deleteCommand.Parameters.Add(new SqlParameter(string.Format("@p{0}", deleteCommand.Parameters.Count), value));
            }

            string wherePk = whereSb.ToString();

            if (wherePk == "")
            {
                throw new MissingPrimaryKeyException(string.Format("{0} does not have a primary key defined.  Batch updating/deleting can not be used for tables without a primary key.", metaTable.TableName));
            }

            deleteCommand.CommandText = string.Format("DELETE {0}\r\nWHERE {1}", dbName, wherePk.Substring(0, wherePk.Length - 2));

            return deleteCommand;
        }

        /// <summary>
        /// <para>Immediately deletes all entities from the collection with a single delete command.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for rows contained in <paramref name="table"/>.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database containing rows are to be deleted.</para>
        /// </param>
        /// <param name="entities">
        /// <para>Represents the collection of items which are to be removed from <paramref name="table"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to be used.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows deleted from the database.</para>
        /// </returns>
        /// <remarks>
        /// <para>Similiar to stored procedures, and opposite from DeleteAllOnSubmit, rows provided in entities will be deleted immediately with no need to call <see cref="DataContext.SubmitChanges()"/>. Additionally, to improve performance, instead of creating a delete command for each item in entities, a single delete command is created.</para>
        /// </remarks>
        public static Int32 DeleteBatchMtn<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities, DbTransaction transaction = null) where TEntity : class
        {
            DbCommand delete = table.GetDeleteBatchCommandMtn<TEntity>(entities);

            var parameters = from p in delete.Parameters.Cast<DbParameter>()
                             select p.Value;

            var ctx = table.Context.NewDataContextMtn(transaction);

            int retVal = ctx.ExecuteCommand(delete.CommandText, parameters.ToArray());

            ctx.Dispose();

            return retVal;
        }

        /// <summary>
        /// <para>Immediately deletes all entities from the collection with a single delete command.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for rows contained in <paramref name="table"/>.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database containing rows are to be deleted.</para>
        /// </param>
        /// <param name="filter">
        /// <para>Represents a filter of items to be deleted in <paramref name="table"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to be used.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows deleted from the database.</para>
        /// </returns>
        /// <remarks>
        /// <para>Similiar to stored procedures, and opposite from DeleteAllOnSubmit, rows provided in entities will be deleted immediately with no need to call <see cref="DataContext.SubmitChanges()"/>. Additionally, to improve performance, instead of creating a delete command for each item in entities, a single delete command is created.</para>
        /// </remarks>
        public static Int32 DeleteBatchMtn<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, Boolean>> filter, DbTransaction transaction = null) where TEntity : class
        {
            return table.DeleteBatchMtn(table.Where(filter), transaction);
        }

     

        /// <summary>
        /// <para>Returns the Transact SQL string representation the LINQ  command text and parameters used that would be issued to delete all entities from the collection with a single delete command, but is only preview.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for rows contained in <paramref name="table"/>.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database containing rows are to be deleted.</para>
        /// </param>
        /// <param name="entities">
        /// <para>Represents the collection of items which are to be removed from <paramref name="table"/>.</para>
        /// </param>
        /// <param name="returnLog">
        /// <para>Indicates if returns log data.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the Transact SQL string representation the LINQ.</para>
        /// </returns>
        /// <remarks>
        /// <para>This method is useful for debugging purposes or when used in other utilities such as LINQPad.</para>
        /// </remarks>
        public static String DeleteBatchPreviewMtn<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities, Boolean returnLog = true) where TEntity : class
        {
            DbCommand delete = table.GetDeleteBatchCommandMtn<TEntity>(entities);
            return delete.PreviewCommandTextMtn(false) + (returnLog ? table.Context.GetLogMtn() : "");
        }

        /// <summary>
        /// <para>Returns the Transact SQL string representation the LINQ  command text and parameters used that would be issued to delete all entities from the collection with a single delete command, but is only preview.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for rows contained in <paramref name="table"/>.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database containing rows are to be deleted.</para>
        /// </param>
        /// <param name="filter">
        /// <para>Represents a filter of items to be deleted in <paramref name="table"/>.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the Transact SQL string representation the LINQ.</para>
        /// </returns>
        /// <remarks>
        /// <para>This method is useful for debugging purposes or when used in other utilities such as LINQPad.</para>
        /// </remarks>
        public static String DeleteBatchPreviewMtn<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, Boolean>> filter) where TEntity : class
        {
            return table.DeleteBatchPreviewMtn(table.Where(filter));
        }
        #endregion

        #region Update
       
        /// <summary>
        /// <para>Returns the Transact SQL string representation the LINQ  command text and parameters used that would be issued to update all entities from the collection with a single update command.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for rows contained in <paramref name="table"/>.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database containing rows are to be updated.</para>
        /// </param>
        /// <param name="entities">
        /// <para>Represents the collection of items which are to be updated from <paramref name="table"/>.</para>
        /// </param>
        ///<param name="evaluator">
        /// <para>A Lambda expression returning a <typeparamref name="TEntity"/> that defines the update assignments to be performed on each item in entities.</para>
        ///</param>
        /// <param name="returnLog">
        /// <para>Indicates if returns log data.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the Transact SQL string representation the LINQ  command text and parameters used that would be issued to update all entities from the collection with a single update command.</para>
        /// </returns>
        /// <remarks>
        /// <para>This method is useful for debugging purposes or when used in other utilities such as LINQPad.</para>
        /// </remarks>
        public static String UpdateBatchPreviewMtn<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities, Expression<Func<TEntity, TEntity>> evaluator, Boolean returnLog = true) where TEntity : class
        {
            DbCommand update = table.GetUpdateBatchCommandMtn<TEntity>(entities, evaluator);
            return update.PreviewCommandTextMtn(false) + (returnLog ? table.Context.GetLogMtn() : "");
        }

        /// <summary>
        /// <para>Returns the Transact SQL string representation the LINQ  command text and parameters used that would be issued to update all entities from the collection with a single update command.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for rows contained in <paramref name="table"/>.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database containing rows are to be updated.</para>
        /// </param>
        /// <param name="filter">
        /// <para>Represents a filter of items to be updated in <paramref name="table"/>.</para>
        /// </param>
        ///<param name="evaluator">
        /// <para>A Lambda expression returning a <typeparamref name="TEntity"/> that defines the update assignments to be performed on each item in entities.</para>
        ///</param>
        /// <returns>
        /// <para>Returns the Transact SQL string representation the LINQ  command text and parameters used that would be issued to update all entities from the collection with a single update command.</para>
        /// </returns>
        /// <remarks>
        /// <para>This method is useful for debugging purposes or when used in other utilities such as LINQPad.</para>
        /// </remarks>
        public static String UpdateBatchPreviewMtn<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, Boolean>> filter, Expression<Func<TEntity, TEntity>> evaluator) where TEntity : class
        {
            return table.UpdateBatchPreviewMtn(table.Where(filter), evaluator);
        }

        /// <summary>
        /// <para>Immediately updates all entities in the collection with a single update command based on a <typeparamref name="TEntity"/> created from a Lambda expression.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for rows contained in <paramref name="table"/>.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database containing rows are to be updated.</para>
        /// </param>
        /// <param name="entities">
        /// <para>Represents the collection of items which are to be updated from <paramref name="table"/>.</para>
        /// </param>
        ///<param name="evaluator">
        /// <para>A Lambda expression returning a <typeparamref name="TEntity"/> that defines the update assignments to be performed on each item in entities.</para>
        ///</param>
        /// <param name="transaction">
        /// <para>Transaction to be used.</para>
        /// </param>
        /// <returns>
        /// <para>The number of records affected.</para>
        /// </returns> 
        public static Int32 UpdateBatchMtn<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities, Expression<Func<TEntity, TEntity>> evaluator, DbTransaction transaction = null) where TEntity : class
        {
            var context = table.NewDataContextMtn(transaction);

            DbCommand update = table.GetUpdateBatchCommandMtn<TEntity>(entities, evaluator);

            var parameters = from p in update.Parameters.Cast<DbParameter>()
                             select p.Value;

            int retVal = context.ExecuteCommand(update.CommandText, parameters.ToArray());
            context.Dispose();
            return retVal;
        }

        /// <summary>
        /// <para>Immediately updates all entities in the collection with a single update command based on a <typeparamref name="TEntity"/> created from a Lambda expression.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for rows contained in <paramref name="table"/>.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database containing rows are to be updated.</para>
        /// </param>
        /// <param name="filter">
        /// <para>Represents a filter of items to be updated in <paramref name="table"/>.</para>
        /// </param>
        ///<param name="evaluator">
        /// <para>A Lambda expression returning a <typeparamref name="TEntity"/> that defines the update assignments to be performed on each item in entities.</para>
        ///</param>
        /// <returns>
        /// <para>The number of records affected.</para>
        /// </returns>    
        public static int UpdateBatchMtn<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> evaluator) where TEntity : class
        {
            return table.UpdateBatchMtn(table.Where(filter), evaluator);
        }

        #endregion

        #region Update

       
        /// <summary>
        /// <para>Update a entity on table.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="entity">
        /// <para>Entity to be updated.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to be used.</para>
        /// </param>
        /// <param name="rollbackOnError">
        /// <para>Indicates if must execute a transaction rollback if occur any error.</para>
        /// </param>
        /// <param name="commitOnSuccess">
        /// <para>Indicates if must execute a transaction commit if succes.</para>
        /// </param>
        /// <param name="rethrow">
        /// <para>Indicates if must rethrow the exception.</para>
        /// </param>
        /// <returns>
        /// <para>Returns true if succeed.</para>
        /// </returns>
        public static Boolean UpdateMtn<TEntity>(this Table<TEntity> table, TEntity entity, DbTransaction transaction = null, Boolean rollbackOnError = false, Boolean commitOnSuccess = true, Boolean rethrow = true) where TEntity : class
        {
            try
            {
                var retVal = UpdateMtn(table, entity, transaction);
                if (commitOnSuccess)
                    transaction.Commit();
                return retVal;
            }
            catch (Exception ex)
            {
                if (rollbackOnError)
                    transaction.Rollback();

                if (rethrow)
                    throw ex;

                return false;
            }
        }

       
        /// <summary>
        /// <para>Update a entity on table.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="entity">
        /// <para>Entity to be updated.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to be used.</para>
        /// </param>
        /// <returns>
        /// <para>Returns true if succeed.</para>
        /// </returns>
        public static Boolean UpdateMtn<TEntity>(this Table<TEntity> table, TEntity entity, DbTransaction transaction = null) where TEntity : class
        {
            var context = table.NewDataContextMtn(transaction);
            if (transaction != null)
                context.Transaction = transaction;

            var mapping = context.Mapping.GetTable(typeof(TEntity));
            var tableCtx = context.GetTable<TEntity>();
            List<System.Data.Linq.Mapping.MetaDataMember> pkfield = null;
            try
            {
                pkfield = mapping.RowType.DataMembers.Where(d => d.IsPrimaryKey).ToList();
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }

            if (pkfield != null && pkfield.Count > 0)
            {
                tableCtx.Attach(entity, false);
                context.SubmitChanges();
                context.Dispose();
                return true;
            }
            else
            {
                throw new Exception("The entity must have a primary key to update");
            }
        }

        /// <summary>
        /// <para>Update only the first record on table.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="entity">
        /// <para>Updated entity.</para>
        /// </param>
        /// <param name="filter">
        /// <para>Filter to find the record to be updated.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to be used.</para>
        /// </param>    
        /// <returns>
        /// <para>Returns true if succeed.</para>
        /// </returns>
        public static Boolean UpdateFirstMtn<TEntity>(this Table<TEntity> table, TEntity entity, Expression<Func<TEntity, Boolean>> filter, DbTransaction transaction = null) where TEntity : class
        {
            var context = table.NewDataContextMtn(transaction);
            if (transaction != null)
                context.Transaction = transaction;
            var tableCtx = context.GetTable<TEntity>();

            TEntity entityOriginal = tableCtx.Where(filter).FirstOrDefault();
            TEntity newEntity = entity.CloneMtn();

            var mapping = context.Mapping.GetTable(typeof(TEntity));

            List<System.Data.Linq.Mapping.MetaDataMember> pkfield = null;
            try
            {
                pkfield = mapping.RowType.DataMembers.Where(d => d.IsPrimaryKey).ToList();
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }

            if (pkfield != null && pkfield.Count > 0)
            {
                newEntity.GetType().GetProperty(pkfield.First().Name).SetValue(newEntity, entityOriginal.GetType().GetProperty(pkfield.First().Name).GetValue(entityOriginal, null), null);
            }
            else
            {
                throw new Exception("The entity must have a primary key (one only) to update");
            }

            context.Dispose();

            if (entityOriginal == null)
                return UpdateMtn(table, entity, transaction);
            else
                return UpdateMtn(table, entityOriginal, newEntity, transaction);

        }
        
        /// <summary>
        /// <para>Update only the single record on table.If found more than one throw a exception.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="entity">
        /// <para>Updated entity.</para>
        /// </param>
        /// <param name="filter">
        /// <para>Filter to find the record to be updated.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to be used.</para>
        /// </param>    
        /// <returns>
        /// <para>Returns true if succeed.</para>
        /// </returns>
        public static Boolean UpdateMtn<TEntity>(this Table<TEntity> table, TEntity entity, Expression<Func<TEntity, Boolean>> filter, DbTransaction transaction = null) where TEntity : class
        {
            TEntity entityOriginal = table.Where(filter).SingleOrDefault();

            if (entityOriginal == null)
                return UpdateMtn(table, entity, transaction);
            else
                return UpdateMtn(table, entityOriginal, entity, transaction);

        }
        
        /// <summary>
        /// <para>Update the entity on table.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="entityOriginal">
        /// <para>Original entity.</para>
        /// </param>
        /// <param name="entity">
        /// <para>Updated entity.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to be used.</para>
        /// </param> 
        /// <returns>
        /// <para>Returns true if succeed.</para>
        /// </returns>
        public static Boolean UpdateMtn<TEntity>(this Table<TEntity> table, TEntity entityOriginal, TEntity entity, DbTransaction transaction = null) where TEntity : class
        {
            var context = table.NewDataContextMtn(transaction);
            if (transaction != null)
                context.Transaction = transaction;
            var tableCtx = context.GetTable<TEntity>();
            tableCtx.Attach(entity, entityOriginal);
            context.SubmitChanges();
            context.Dispose();
            return true;
        }
        #endregion

        #region Preview

        /// <summary>
        /// <para>Returns the Transact SQL string representation the LINQ command text and parameters used that would be issued to perform the query's select statement.</para>
        /// </summary>
        /// <param name="context">
        /// <para>The DataContext to execute the batch select against.</para>
        /// </param>
        /// <param name="query">
        /// <para>Represents the SELECT query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the Transact SQL string representation the LINQ command text and parameters used that would be issued to perform the query's select statement.</para>
        /// </returns>
        public static String PreviewSqlMtn(this DataContext context, IQueryable query)
        {
            var cmd = context.GetCommand(query);
            return cmd.PreviewCommandTextMtn(true);
        }

        /// <summary>
        /// <para>Returns the Transact SQL string representation the LINQ command text and parameters used that would be issued to perform the query's select statement.</para>
        /// </summary>
        /// <param name="context">
        /// <para>The DataContext to execute the batch select against.</para>
        /// </param>
        /// <param name="query">
        /// <para>Represents the SELECT query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the Transact SQL string representation the LINQ command text and parameters used that would be issued to perform the query's select statement.</para>
        /// </returns>
        public static String PreviewCommandTextMtn(this DataContext context, IQueryable query)
        {
            var cmd = context.GetCommand(query);
            return cmd.PreviewCommandTextMtn(false);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="DbCommand.CommandText"/> along with <see cref="DbCommand.Parameters"/> if present.
        /// </summary>
        /// <param name="cmd">The <see cref="DbCommand"/> to analyze.</param>
        /// <param name="forTransactSql">Whether or not the text should be formatted as 'logging' similiar to LINQ to SQL output, or in valid Transact SQL syntax ready for use with a 'query analyzer' type tool.</param>
        /// <returns>Returns a string representation of the <see cref="DbCommand.CommandText"/> along with <see cref="DbCommand.Parameters"/> if present.</returns>
        /// <remarks>This method is useful for debugging purposes or when used in other utilities such as LINQPad.</remarks>
        private static string PreviewCommandTextMtn(this DbCommand cmd, bool forTransactSql)
        {
            var output = new StringBuilder();

            if (!forTransactSql)
                output.AppendLine(cmd.CommandText);

            foreach (DbParameter parameter in cmd.Parameters)
            {
                int num = 0;
                int num2 = 0;
                PropertyInfo property = parameter.GetType().GetProperty("Precision");
                if (property != null)
                {
                    num = (int)Convert.ChangeType(property.GetValue(parameter, null), typeof(int), CultureInfo.InvariantCulture);
                }
                PropertyInfo info2 = parameter.GetType().GetProperty("Scale");
                if (info2 != null)
                {
                    num2 = (int)Convert.ChangeType(info2.GetValue(parameter, null), typeof(int), CultureInfo.InvariantCulture);
                }
                SqlParameter parameter2 = parameter as SqlParameter;

                if (forTransactSql)
                {
                    output.AppendFormat("DECLARE {0} {1}{2}; SET {0} = {3}\r\n",
                        new object[] { 
                                parameter.ParameterName, 
                                ( parameter2 == null ) ? parameter.DbType.ToString() : parameter2.SqlDbType.ToString(), 
                                ( parameter.Size > 0 ) ? "( " + parameter.Size.ToString( CultureInfo.CurrentCulture ) + " )" : "", 
                                GetParameterTransactValueMtn( parameter, parameter2 ) });
                }
                else
                {
                    output.AppendFormat("-- {0}: {1} {2} (Size = {3}; Prec = {4}; Scale = {5}) [{6}]\r\n", new object[] { parameter.ParameterName, parameter.Direction, (parameter2 == null) ? parameter.DbType.ToString() : parameter2.SqlDbType.ToString(), parameter.Size.ToString(CultureInfo.CurrentCulture), num, num2, (parameter2 == null) ? parameter.Value : parameter2.SqlValue });
                }
            }

            if (forTransactSql)
                output.Append("\r\n" + cmd.CommandText);

            return output.ToString();
        }

        #endregion

        #region GetParameterTransactValue

        private static string GetParameterTransactValueMtn(DbParameter parameter, SqlParameter parameter2)
        {
            if (parameter2 == null)
                return parameter.Value.ToString(); // Not going to deal with NON SQL parameters.

            switch (parameter2.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.UniqueIdentifier:
                    return string.Format("'{0}'", parameter2.SqlValue);

                default:
                    return parameter2.SqlValue.ToString();
            }
        }

        #endregion

        #region Auxiliar methods
        private static DbCommand GetDeleteBatchCommandMtn<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities) where TEntity : class
        {
            var deleteCommand = table.Context.GetCommand(entities);
            deleteCommand.CommandText = string.Format("DELETE {0}\r\n", table.GetDbNameMtn()) + GetBatchJoinQueryMtn<TEntity>(table, entities);
            return deleteCommand;
        }

        private static DbCommand GetUpdateBatchCommandMtn<TEntity>(this Table<TEntity> table, IQueryable<TEntity> entities, Expression<Func<TEntity, TEntity>> evaluator) where TEntity : class
        {
            var updateCommand = table.Context.GetCommand(entities);

            var setSb = new StringBuilder();
            int memberInitCount = 1;

            // Process the MemberInitExpression (there should only be one in the evaluator Lambda) to convert the expression tree
            // into a valid DbCommand.  The Visit<> method will only process expressions of type MemberInitExpression and requires
            // that a MemberInitExpression be returned - in our case we'll return the same one we are passed since we are building
            // a DbCommand and not 'really using' the evaluator Lambda.
            evaluator.VisitMtn<MemberInitExpression>(delegate(MemberInitExpression expression)
            {
                if (memberInitCount > 1)
                {
                    throw new NotImplementedException("Currently only one MemberInitExpression is allowed for the evaluator parameter.");
                }
                memberInitCount++;

                setSb.Append(GetDbSetStatementMtn<TEntity>(expression, table, updateCommand));

                return expression; // just return passed in expression to keep 'visitor' happy.
            });

            // Complete the command text by concatenating bits together.
            updateCommand.CommandText = string.Format("UPDATE {0}\r\n{1}\r\n\r\n{2}",
                                                            table.GetDbNameMtn(),									// Database table name
                                                            setSb.ToString(),									// SET fld = {}, fld2 = {}, ...
                                                            GetBatchJoinQueryMtn<TEntity>(table, entities));	// Subquery join created from entities command text

            if (updateCommand.CommandText.IndexOf("[arg0]", System.StringComparison.Ordinal) >= 0 || updateCommand.CommandText.IndexOf("NULL AS [EMPTY]", System.StringComparison.Ordinal) >= 0)
            {
                // TODO (Chris): Probably a better way to determine this by using an visitor on the expression before the
                //				 var selectExpression = Expression.Call... method call (search for that) and see which funcitons
                //				 are being used and determine if supported by LINQ to SQL
                throw new NotSupportedException(string.Format("The evaluator Expression<Func<{0},{0}>> has processing that needs to be performed once the query is returned (i.e. string.Format()) and therefore can not be used during batch updating.", table.GetType()));
            }

            return updateCommand;
        }

        private static string GetDbSetStatementMtn<TEntity>(MemberInitExpression memberInitExpression, Table<TEntity> table, DbCommand updateCommand) where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (memberInitExpression.Type != entityType)
            {
                throw new NotImplementedException(string.Format("The MemberInitExpression is intializing a class of the incorrect type '{0}' and it should be '{1}'.", memberInitExpression.Type, entityType));
            }

            var setSb = new StringBuilder();

            var tableName = table.GetDbNameMtn();
            var metaTable = table.Context.Mapping.GetTable(entityType);
            // Used to look up actual field names when MemberAssignment is a constant,
            // need both the Name (matches the property name on LINQ object) and the
            // MappedName (db field name).
            var dbCols = from mdm in metaTable.RowType.DataMembers
                         select new { mdm.MappedName, mdm.Name };

            // Walk all the expression bindings and generate SQL 'commands' from them.  Each binding represents a property assignment
            // on the TEntity object initializer Lambda expression.
            foreach (var binding in memberInitExpression.Bindings)
            {
                var assignment = binding as MemberAssignment;

                if (binding == null)
                {
                    throw new NotImplementedException("All bindings inside the MemberInitExpression are expected to be of type MemberAssignment.");
                }

                // TODO (Document): What is this doing?  I know it's grabbing existing parameter to pass into Expression.Call() but explain 'why'
                //					I assume it has something to do with fact we can't just access the parameters of assignment.Expression?
                //					Also, any concerns of whether or not if there are two params of type entity type?
                ParameterExpression entityParam = null;
                assignment.Expression.VisitMtn<ParameterExpression>(delegate(ParameterExpression p) { if (p.Type == entityType) entityParam = p; return p; });

                // Get the real database field name.  binding.Member.Name is the 'property' name of the LINQ object
                // so I match that to the Name property of the table mapping DataMembers.
                string name = binding.Member.Name;
                var dbCol = (from c in dbCols
                             where c.Name == name
                             select c).FirstOrDefault();

                if (dbCol == null)
                {
                    throw new ArgumentOutOfRangeException(name, string.Format("The corresponding field on the {0} table could not be found.", tableName));
                }

                // If entityParam is NULL, then no references to other columns on the TEntity row and need to eval 'constant' value...
                if (entityParam == null)
                {
                    // Compile and invoke the assignment expression to obtain the contant value to add as a parameter.
                    var constant = Expression.Lambda(assignment.Expression, null).Compile().DynamicInvoke();

                    // use the MappedName from the table mapping DataMembers - that is field name in DB table.
                    if (constant == null)
                    {
                        setSb.AppendFormat("[{0}] = null, ", dbCol.MappedName);
                    }
                    else
                    {
                        // Add new parameter with massaged name to avoid clashes.
                        setSb.AppendFormat("[{0}] = @p{1}, ", dbCol.MappedName, updateCommand.Parameters.Count);
                        updateCommand.Parameters.Add(new SqlParameter(string.Format("@p{0}", updateCommand.Parameters.Count), constant));
                    }
                }
                else
                {
                    // TODO (Documentation): Explain what we are doing here again, I remember you telling me why we have to call but I can't remember now.
                    // Wny are we calling Expression.Call and what are we passing it?  Below comments are just 'made up' and probably wrong.

                    // Create a MethodCallExpression which represents a 'simple' select of *only* the assignment part (right hand operator) of
                    // of the MemberInitExpression.MemberAssignment so that we can let the Linq Provider do all the 'sql syntax' generation for
                    // us. 
                    //
                    // For Example: TEntity.Property1 = TEntity.Property1 + " Hello"
                    // This selectExpression will be only dealing with TEntity.Property1 + " Hello"
                    var selectExpression = Expression.Call(
                                                typeof(Queryable),
                                                "Select",
                                                new Type[] { entityType, assignment.Expression.Type },

                    // TODO (Documentation): How do we know there are only 'two' parameters?  And what is Expression.Lambda
                        //						 doing?  I assume it's returning a type of assignment.Expression.Type to match above?

                                                Expression.Constant(table),
                                                Expression.Lambda(assignment.Expression, entityParam));

                    setSb.AppendFormat("[{0}] = {1}, ",
                                            dbCol.MappedName,
                                            GetDbSetAssignmentMtn(table, selectExpression, updateCommand, name));
                }
            }

            var setStatements = setSb.ToString();
            return "SET " + setStatements.Substring(0, setStatements.Length - 2); // remove ', '
        }

        /// <summary>
        /// Some LINQ Query syntax is invalid because SQL (or whomever the provider is) can not translate it to its native language.  
        /// DataContext.GetCommand() does not detect this, only IProvider.Execute or IProvider.Compile call the necessary code to 
        /// check this.  This function invokes the IProvider.Compile to make sure the provider can translate the expression.
        /// </summary>
        /// <remarks>
        /// An example of a LINQ query that previously 'worked' in the *Batch methods but needs to throw an exception is something
        /// like the following:
        /// 
        /// var pay = 
        ///		from h in HistoryData
        ///		where h.his.Groups.gName == "Ochsner" and h.hisType == "pay"
        ///		select h;
        ///		
        /// HistoryData.UpdateBatchPreview( pay, h => new HistoryData { hisIndex = ( int.Parse( h.hisIndex ) - 1 ).ToString() } ).Dump();
        /// 
        /// The int.Parse is not valid and needs to throw an exception like: 
        /// 
        ///		Could not translate expression '(Parse(p.hisIndex) - 1).ToString()' into SQL and could not treat it as a local expression.
        ///		
        ///	Unfortunately, the IProvider.Compile is internal and I need to use Reflection to call it (ugh).  I've several e-mails sent into
        ///	MS LINQ team members and am waiting for a response and will correct/improve code as soon as possible.
        /// </remarks>
        private static void ValidateExpressionMtn(ITable table, Expression expression)
        {
            var context = table.Context;
            PropertyInfo providerProperty = context.GetType().GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);
            var provider = providerProperty.GetValue(context, null);
            var compileMi = provider.GetType().GetMethod("System.Data.Linq.Provider.IProvider.Compile", BindingFlags.Instance | BindingFlags.NonPublic);

            // Simply compile the expression to see if it will work.
            compileMi.Invoke(provider, new object[] { expression });
        }

        private static string GetDbSetAssignmentMtn(ITable table, MethodCallExpression selectExpression, DbCommand updateCommand, string bindingName)
        {
            ValidateExpressionMtn(table, selectExpression);

            // Convert the selectExpression into an IQueryable query so that I can get the CommandText
            var selectQuery = (table as IQueryable).Provider.CreateQuery(selectExpression);

            // Get the DbCommand so I can grab relavent parts of CommandText to construct a field 
            // assignment and based on the 'current TEntity row'.  Additionally need to massage parameter 
            // names from temporary command when adding to the final update command.
            var selectCmd = table.Context.GetCommand(selectQuery);
            var selectStmt = selectCmd.CommandText;
            selectStmt = selectStmt.Substring(7,									// Remove 'SELECT ' from front ( 7 )
                                        selectStmt.IndexOf("\r\nFROM ", System.StringComparison.Ordinal) - 7)		// Return only the selection field expression
                                    .Replace("[t0].", "")							// Remove table alias from the select
                                    .Replace(" AS [value]", "")					// If the select is not a direct field (constant or expression), remove the field alias
                                    .Replace("@p", "@p" + bindingName);			// Replace parameter name so doesn't conflict with existing ones.

            foreach (var selectParam in selectCmd.Parameters.Cast<DbParameter>())
            {
                var paramName = string.Format("@p{0}", updateCommand.Parameters.Count);

                // DataContext.ExecuteCommand ultimately just takes a object array of parameters and names them p0-N.  
                // So I need to now do replaces on the massaged value to get it in proper format.
                selectStmt = selectStmt.Replace(
                                    selectParam.ParameterName.Replace("@p", "@p" + bindingName),
                                    paramName);

                updateCommand.Parameters.Add(new SqlParameter(paramName, selectParam.Value));
            }

            return selectStmt;
        }

        private static string GetBatchJoinQueryMtn<TEntity>(Table<TEntity> table, IQueryable<TEntity> entities) where TEntity : class
        {
            var metaTable = table.Context.Mapping.GetTable(typeof(TEntity));

            var keys = from mdm in metaTable.RowType.DataMembers
                       where mdm.IsPrimaryKey
                       select new { mdm.MappedName };

            var joinSb = new StringBuilder();
            var subSelectSb = new StringBuilder();

            foreach (var key in keys)
            {
                joinSb.AppendFormat("j0.[{0}] = j1.[{0}] AND ", key.MappedName);
                // For now, always assuming table is aliased as t0.  Should probably improve at some point.
                // Just writing a smaller sub-select so it doesn't get all the columns of data, but instead
                // only the primary key fields used for joining.
                subSelectSb.AppendFormat("[t0].[{0}], ", key.MappedName);
            }

            var selectCommand = table.Context.GetCommand(entities);
            var select = selectCommand.CommandText;

            var join = joinSb.ToString();

            if (join == "")
            {
                throw new MissingPrimaryKeyException(string.Format("{0} does not have a primary key defined.  Batch updating/deleting can not be used for tables without a primary key.", metaTable.TableName));
            }

            join = join.Substring(0, join.Length - 5);											// Remove last ' AND '
            #region - Better ExpressionTree Handling Needed -


            //Below is a sample query where the let statement was used to simply the 'where clause'.  However, it produced an extra level
            //in the query.

            //var manage =
            //    from u in User
            //    join g in Groups on u.User_Group_id equals g.gKey into groups
            //    from g in groups.DefaultIfEmpty()
            //    let correctGroup = groupsToManage.Contains( g.gName ) || ( groupsToManage.Contains( "_GLOBAL" ) && g.gKey == null )
            //    where correctGroup && ( users.Contains( u.User_Authenticate_id ) || userEmails.Contains( u.User_Email ) ) || userKeys.Contains( u.User_id )
            //    select u;

            //Produces this SQL:
            //SELECT [t2].[User_id] AS [uKey], [t2].[User_Authenticate_id] AS [uAuthID], [t2].[User_Email] AS [uEmail], [t2].[User_Pin] AS [uPin], [t2].[User_Active] AS [uActive], [t2].[uAdminAuthID], [t2].[uFailureCount]
            //FROM (
            //    SELECT [t0].[User_id], [t0].[User_Authenticate_id], [t0].[User_Email], [t0].[User_Pin], [t0].[User_Active], [t0].[uFailureCount], [t0].[uAdminAuthID], 
            //        (CASE 
            //            WHEN [t1].[gName] IN (@p0) THEN 1
            //            WHEN NOT ([t1].[gName] IN (@p0)) THEN 0
            //            ELSE NULL
            //         END) AS [value]
            //    FROM [User] AS [t0]
            //    LEFT OUTER JOIN [Groups] AS [t1] ON [t0].[User_Group_id] = ([t1].[gKey])
            //    ) AS [t2]
            //WHERE (([t2].[value] = 1) AND (([t2].[User_Authenticate_id] IN (@p1)) OR ([t2].[User_Email] IN (@p2)))) OR ([t2].[User_id] IN (@p3))			 

            //If I put the entire where in one line...
            //where 	( groupsToManage.Contains( g.gName ) || ( groupsToManage.Contains( "_GLOBAL" ) && g.gKey == null ) ) && 
            //        ( users.Contains( u.User_Authenticate_id ) || userEmails.Contains( u.User_Email ) ) || userKeys.Contains ( u.User_id )

            //I get this SQL:
            //SELECT [t0].[User_id] AS [uKey], [t0].[User_Authenticate_id] AS [uAuthID], [t0].[User_Email] AS [uEmail], [t0].[User_Pin] AS [uPin], [t0].[User_Active] AS [uActive], [t0].[uAdminAuthID], [t0].[uFailureCount]
            //FROM [User] AS [t0]
            //LEFT OUTER JOIN [Groups] AS [t1] ON [t0].[User_Group_id] = ([t1].[gKey])
            //WHERE (([t1].[gName] IN (@p0)) AND (([t0].[User_Authenticate_id] IN (@p1)) OR ([t0].[User_Email] IN (@p2)))) OR ([t0].[User_id] IN (@p3))			

            //The second 'cleaner' SQL worked with my original 'string parsing' of simply looking for [t0] and stripping everything before it
            //to get rid of the SELECT and any 'TOP' clause if present.  But the first SQL introduced a layer which caused [t2] to be used.  So
            //I have to do a bit different string parsing.  There is probably a more efficient way to examine the ExpressionTree and figure out
            //if something like this is going to happen.  I will explore it later.

            #endregion
            var endSelect = @select.IndexOf("[t", System.StringComparison.Ordinal);													// Get 'SELECT ' and any TOP clause if present
            var selectClause = select.Substring(0, endSelect);
            var selectTableNameStart = endSelect + 1;												// Get the table name LINQ to SQL used in query generation
            var selectTableName = select.Substring(selectTableNameStart,							// because I have to replace [t0] with it in the subSelectSB
                                        @select.IndexOf("]", selectTableNameStart, System.StringComparison.Ordinal) - (selectTableNameStart));

            // TODO (Chris): I think instead of searching for ORDER BY in the entire select statement, I should examine the ExpressionTree and see
            // if the *outer* select (in case there are nested subselects) has an orderby clause applied to it.
            var needsTopClause = selectClause.IndexOf(" TOP ", System.StringComparison.Ordinal) < 0 && @select.IndexOf("\r\nORDER BY ", System.StringComparison.Ordinal) > 0;

            var subSelect = selectClause
                                + (needsTopClause ? "TOP 100 PERCENT " : "")							// If order by in original select without TOP clause, need TOP
                                + subSelectSb.ToString()												// Append just the primary keys.
                                             .Replace("[t0]", string.Format("[{0}]", selectTableName));
            subSelect = subSelect.Substring(0, subSelect.Length - 2);									// Remove last ', '

            subSelect += select.Substring(@select.IndexOf("\r\nFROM ", System.StringComparison.Ordinal)); // Create a sub SELECT that *only* includes the primary key fields

            var batchJoin = String.Format("FROM {0} AS j0 INNER JOIN (\r\n\r\n{1}\r\n\r\n) AS j1 ON ({2})\r\n", table.GetDbNameMtn(), subSelect, join);
            return batchJoin;
        }

        private static string GetDbNameMtn<TEntity>(this Table<TEntity> table) where TEntity : class
        {
            var entityType = typeof(TEntity);
            var metaTable = table.Context.Mapping.GetTable(entityType);
            var tableName = metaTable.TableName;

            if (!tableName.StartsWith("["))
            {
                string[] parts = tableName.Split('.');
                tableName = string.Format("[{0}]", string.Join("].[", parts));
            }

            return tableName;
        }

        #endregion

        #region GetLog
       
        private static string GetLogMtn(this DataContext context)
        {
            PropertyInfo providerProperty = context.GetType().GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);
            var provider = providerProperty.GetValue(context, null);
            Type providerType = provider.GetType();

            PropertyInfo modeProperty = providerType.GetProperty("Mode", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo servicesField = providerType.GetField("services", BindingFlags.Instance | BindingFlags.NonPublic);
            object services = servicesField != null ? servicesField.GetValue(provider) : null;
            PropertyInfo modelProperty = services != null ? services.GetType().GetProperty("Model", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty) : null;

            return string.Format("-- Context: {0}({1}) Model: {2} Build: {3}\r\n",
                            providerType.Name,
                            modeProperty != null ? modeProperty.GetValue(provider, null) : "unknown",
                            modelProperty != null ? modelProperty.GetValue(services, null).GetType().Name : "unknown",
                            "3.5.21022.8");
        }
        #endregion

        #region GetChangedItems
     
        /// <summary>
        /// <para>Returns a list of changed items inside the Context before being applied to the data store.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="context">
        /// <para>The DataContext to interrogate  for pending changes.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a list of changed items inside the Context before being applied to the data store.</para>
        /// </returns>
        /// <remarks>Based on Ryan Haney's code at http://dotnetslackers.com/articles/csharp/GettingChangedEntitiesFromLINQToSQLDataContext.aspx.  Note that this code relies on reflection of private fields and members.</remarks>
        public static List<ChangedItemsMtn<TEntity>> GetChangedItemsMtn<TEntity>(this DataContext context)
        {
            // create a dictionary of type TItem for return to caller
            List<ChangedItemsMtn<TEntity>> changedItems = new List<ChangedItemsMtn<TEntity>>();

            PropertyInfo providerProperty = context.GetType().GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);
            var provider = providerProperty.GetValue(context, null);
            Type providerType = provider.GetType();

            // use reflection to get changed items from data context
            object services = providerType.GetField("services",
              BindingFlags.NonPublic |
              BindingFlags.Instance |
              BindingFlags.GetField).GetValue(provider);

            object tracker = services.GetType().GetField("tracker",
              BindingFlags.NonPublic |
              BindingFlags.Instance |
              BindingFlags.GetField).GetValue(services);

            System.Collections.IDictionary trackerItems =
              (System.Collections.IDictionary)tracker.GetType().GetField("items",
              BindingFlags.NonPublic |
              BindingFlags.Instance |
              BindingFlags.GetField).GetValue(tracker);

            // iterate through each item in context, adding
            // only those that are of type TItem to the changedItems dictionary
            foreach (System.Collections.DictionaryEntry entry in trackerItems)
            {
                object original = entry.Value.GetType().GetField("original",
                                  BindingFlags.NonPublic |
                                  BindingFlags.Instance |
                                  BindingFlags.GetField).GetValue(entry.Value);

                if (entry.Key is TEntity && original is TEntity)
                {
                    changedItems.Add(
                      new ChangedItemsMtn<TEntity>((TEntity)entry.Key, (TEntity)original)
                    );
                }
            }
            return changedItems;
        }

        /// <summary>
        /// <para>Class that represents the changes </para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for class.</para>
        /// </typeparam>
        public class ChangedItemsMtn<TEntity>
        {
            /// <summary>
            /// Contructor
            /// </summary>
            /// <param name="current">
            /// <para>Current entity, with the changes.</para>
            /// </param>
            /// <param name="original">
            /// <para>Original entity.</para>
            /// </param>
            public ChangedItemsMtn(TEntity current, TEntity original)
            {
                this.Current = current;
                this.Original = original;
            }
            /// <summary>
            /// <para>Current Entity </para>
            /// </summary>
            public TEntity Current { get; set; }
            /// <summary>
            /// <para>Original Entity </para>
            /// </summary>
            public TEntity Original { get; set; }
        }
        #endregion

        #region Truncate
   
        /// <summary>
        /// <para>Performs a truncate table</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type in the underlying database to be truncated.</para>
        /// </param>
        /// <returns> 
        /// <para>Returns false if not be able to truncate.</para>
        /// </returns>
        public static Boolean TruncateTableMtn<TEntity>(this Table<TEntity> table) where TEntity : class
        {
            try
            {
                string truncate = string.Format("TRUNCATE TABLE {0}\r\n", table.GetDbNameMtn());
                object[] parms = new object[] { };
                table.Context.ExecuteCommand(truncate, parms);
                return true;
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }
            return false;
        }

        #endregion

        #region GetMtn
       
        /// <summary>
        /// <para>Returns a Entity using primary key.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for class.</para>
        /// </typeparam>
        /// <param name="context">
        /// <para>Current DataContext.</para>
        /// </param>
        /// <param name="pk">
        /// <para>Record key.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a Entity using primary key.</para>
        /// </returns>
        public static TEntity GetMtn<TEntity>(this DataContext context, object pk, DbTransaction transaction = null) where TEntity : class
        {
            var ctx = context.NewDataContextMtn(transaction);
            var table = ctx.GetTable<TEntity>();
            var mapping = ctx.Mapping.GetTable(typeof(TEntity));
            var pkfield = mapping.RowType.DataMembers.SingleOrDefault(d => d.IsPrimaryKey);
            if (pkfield == null)
                throw new Exception(String.Format("Table {0} does not contain a Primary Key field", mapping.TableName));
            var param = Expression.Parameter(typeof(TEntity), "e");
            var predicate = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.Property(param, pkfield.Name), Expression.Constant(pk)), param);
            TEntity result = table.SingleOrDefault(predicate);
            ctx.Dispose();
            return result;
        }

        /// <summary>
        /// <para>Returns a Entity using primary key.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="pk">
        /// <para>Record key.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a Entity using primary key.</para>
        /// </returns>
        public static TEntity GetMtn<TEntity>(this Table<TEntity> table, object pk, DbTransaction transaction = null) where TEntity : class
        {
            var context = table.Context.NewDataContextMtn(transaction);
            var mapping = context.Mapping.GetTable(typeof(TEntity));
            var pkfield = mapping.RowType.DataMembers.SingleOrDefault(d => d.IsPrimaryKey);
            if (pkfield == null)
                throw new Exception(String.Format("Table {0} does not contain a Primary Key field", mapping.TableName));
            var param = Expression.Parameter(typeof(TEntity), "e");
            var predicate = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.Property(param, pkfield.Name), Expression.Constant(pk)), param);
            TEntity result = table.SingleOrDefault(predicate);
            context.Dispose();
            return result;
        }
        #endregion

        #region GetPage
       
        /// <summary>
        /// <para>Returns a paginated DataPage of TEntity.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for DataPage.</para>
        /// </typeparam>
        /// <typeparam name="TKey">
        /// <para>The object type used for order by purposes.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="page">
        /// <para>The number of the page you requested (starting from page 1).</para>
        /// </param>
        /// <param name="recordsPerPage">
        /// <para>The maximum number of items per page.</para>
        /// </param>
        /// <param name="expression">
        /// <para>A Lambda expression returning a <typeparamref name="TEntity"/>.</para>
        ///</param>
        ///<param name="orderBy">
        /// <para>A Lambda expression returning a filter order by.</para>
        ///</param>
        /// <returns>
        /// <para>Returns a paginated DataPage of TEntity.</para>
        /// </returns>
        /// <remarks>
        /// <para>If the page is zero or less then zero will be treated as one "1".</para>
        /// </remarks>
        public static Mtn.Library.Entities.DataPage<TEntity> GetPageMtn<TEntity, TKey>
            (this Table<TEntity> table, int page, int recordsPerPage, Expression<Func<TEntity, bool>> expression = null, Expression<Func<TEntity, TKey>> orderBy = null) where TEntity : class
        {
            try
            {
                page = (page > 0) ? page - 1 : 0;
                var query = table.AsQueryable();
                if (expression != null)
                    query = query.Where(expression);

                if (orderBy != null)
                    query = query.OrderBy(orderBy);

                int count = query.Count();
                query = query.Skip(page * recordsPerPage).Take(recordsPerPage);
                return new Mtn.Library.Entities.DataPage<TEntity>(query.ToList(), page + 1, recordsPerPage, count);
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }
            return null;
        }

        /// <summary>
        /// <para>Returns a paginated DataPage of TEntity.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Represents the object type for DataPage.</para>
        /// </typeparam>
        /// <typeparam name="TKey">
        /// <para>The object type used for order by purposes.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="page">
        /// <para>The number of the page you requested (starting from page 1).</para>
        /// </param>
        /// <param name="recordsPerPage">
        /// <para>The maximum number of items per page.</para>
        /// </param>
        /// <param name="expression">
        /// <para>A Lambda expression returning a <typeparamref name="TEntity"/>.</para>
        ///</param>
        ///<param name="orderBy">
        /// <para>A Lambda expression returning a filter order by.</para>
        ///</param>
        /// <returns>
        /// <para>Returns a paginated DataPage of TEntity.</para>
        /// </returns>
        /// <remarks>
        /// <para>If the page is zero or less then zero will be treated as one "1".</para>
        /// </remarks>
        public static Mtn.Library.Entities.DataPage<TEntity> GetPageMtn<TEntity, TKey>
            (this IQueryable<TEntity> table, int page, int recordsPerPage, Expression<Func<TEntity, bool>> expression = null, Expression<Func<TEntity, TKey>> orderBy = null) where TEntity : class
        {
            try
            {
                page = (page > 0) ? page - 1 : 0;
                var query = table.AsQueryable();
                if (expression != null)
                    query = query.Where(expression);

                if (orderBy != null)
                    query = query.OrderBy(orderBy);
                else if (table is ObjectQuery)
                {
                    PropertyInfo info = GetPrimaryKey<TEntity>();

                    query = query.OrderByMtn(info.Name);
                }

                int count = query.Count();
                query = query.Skip(page * recordsPerPage).Take(recordsPerPage);
                return new Mtn.Library.Entities.DataPage<TEntity>(query.ToList(), page + 1, recordsPerPage, count);
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }
            return null;
        }
        private static readonly MethodInfo OrderByMethodMtn =
           typeof(Queryable).GetMethods()
               .Where(method => method.Name == "OrderBy").Single(method => method.GetParameters().Length == 2);
        private static readonly MethodInfo OrderByDescendingMethodMtn =
            typeof(Queryable).GetMethods()
                .Where(method => method.Name == "OrderByDescending").Single(method => method.GetParameters().Length == 2);

        /// <summary>
        ///Returns a query ordered by according to the parameters.
        /// </summary>
        /// <typeparam name="TEntity">
        ///Entity class.
        /// </typeparam>
        /// <param name="query">
        ///Represents a query for a particular type.
        /// </param>
        /// <param name="orderby">
        ///Comma separated string eg. "Field1 ASC, field2 DESC, field3".
        ///</param>
        /// <returns>
        ///Returns a query ordered by according to the parameters.
        /// </returns>
        public static IQueryable<TEntity> OrderByMtn<TEntity>(this IQueryable<TEntity> query, String orderby)
        {
            var lstProperty = orderby.RemoveSurroundingWhitespaceMtn(" ").SplitMtn(',').Reverse();

            foreach (string property in lstProperty)
            {
                if (property.IsNullOrEmptyMtn(true))
                    continue;
                var lstByProperty = property.SplitMtn(' ');
                string propertyName = lstByProperty[0];
                PredicateBuilder.OrderTypeMtn ordType = (lstByProperty.Length > 1 ? (lstByProperty[1].ToLowerInvariant() == "desc" ? PredicateBuilder.OrderTypeMtn.Descending : PredicateBuilder.OrderTypeMtn.Ascending) : PredicateBuilder.OrderTypeMtn.Ascending);
                ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "Mtn");
                Expression orderByProperty = Expression.Property(parameter, propertyName);

                LambdaExpression lambda = Expression.Lambda(orderByProperty, new[] { parameter });
                MethodInfo genericMethod;

                genericMethod = ordType == PredicateBuilder.OrderTypeMtn.Ascending ? OrderByMethodMtn.MakeGenericMethod(new[] { typeof(TEntity), orderByProperty.Type }) : OrderByDescendingMethodMtn.MakeGenericMethod(new[] { typeof(TEntity), orderByProperty.Type });

                var newQuery = genericMethod.Invoke(null, new object[] { query, lambda });
                query = (IQueryable<TEntity>)newQuery;
            }
            return query;
        }

        /// <summary>
        /// Returns a query ordered by according to the parameters.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity class.
        /// </typeparam>
        /// <param name="query">
        /// Represents a query for a particular type.
        /// </param>
        /// <param name="predicate">
        /// Predicate generated by the predicate builder to Order By.
        ///</param>
        /// <returns>
        /// Returns a query ordered by according to the parameters.
        /// </returns>
        public static IQueryable<TEntity> OrderByMtn<TEntity>
           (this IQueryable<TEntity> query, Mtn.Library.Extensions.PredicateBuilder.OrderByMtn predicate)
        {
            foreach (var order in predicate.ListOfOrders.OrderByDescending(i => i.Hierarchy))
            {
                ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "Mtn");
                Expression orderByProperty = Expression.Property(parameter, order.Name);

                LambdaExpression lambda = Expression.Lambda(orderByProperty, new[] { parameter });
                MethodInfo genericMethod;

                if (order.OrderType == PredicateBuilder.OrderTypeMtn.Ascending)
                    genericMethod = OrderByMethodMtn.MakeGenericMethod(new[] { typeof(TEntity), orderByProperty.Type });
                else
                    genericMethod = OrderByDescendingMethodMtn.MakeGenericMethod(new[] { typeof(TEntity), orderByProperty.Type });

                var newQuery = genericMethod.Invoke(null, new object[] { query, lambda });
                query = (IQueryable<TEntity>)newQuery;
            }
            return query;
        }

        private static PropertyInfo GetPrimaryKey<T>()
        {
            PropertyInfo[] infos = typeof(T).GetProperties();
            PropertyInfo pkProperty = null;
            foreach (var info in infos)
            {
                var column = info.GetCustomAttributes(false)
                 .Where(x => x != null && x is ColumnAttribute)
                 .FirstOrDefault(x =>
                  ((ColumnAttribute)x).IsPrimaryKey &&
                  ((ColumnAttribute)x).DbType.Contains("NOT NULL"));
                if (column != null)
                {
                    pkProperty = info;
                    break;
                }
                else
                {
                    column = info.GetCustomAttributes(false)
                 .Where(x => x != null && x is EdmScalarPropertyAttribute).FirstOrDefault(x =>
                  ((EdmScalarPropertyAttribute)x).EntityKeyProperty &&
                  ((EdmScalarPropertyAttribute)x).IsNullable == false);

                    if (column != null)
                    {
                        pkProperty = info;
                        break;
                    }
                }
            }

            if (pkProperty == null)
            {

                throw new NotSupportedException(
                    typeof(T).ToString() + " has no Primary Key");
            }

            return pkProperty;
        }

        #endregion
       
        #region Transaction
        
        /// <summary>
        /// <para>Returns a transaction.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="isolationLevel">
        /// <para>Isolation level.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a transaction.</para>
        /// </returns>
        public static DbTransaction BeginTransactionMtn<TEntity>(this Table<TEntity> table, System.Data.IsolationLevel isolationLevel) where TEntity : class
        {
            return BeginTransactionMtn(table.Context, isolationLevel);
        }
    
        /// <summary>
        /// <para>Returns a transaction.</para>
        /// </summary>
        /// <param name="context">
        /// <para>Current DataContext.</para>
        /// </param>
        /// <param name="isolationLevel">
        /// <para>Isolation level.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a transaction.</para>
        /// </returns>
        public static DbTransaction BeginTransactionMtn(this DataContext context, System.Data.IsolationLevel isolationLevel)
        {
            var cnn = context.Connection;
            if (cnn.State == ConnectionState.Broken || cnn.State == ConnectionState.Closed)
                cnn.Open();
            var tran = cnn.BeginTransaction(isolationLevel);
            return tran;
        }
       
        /// <summary>
        /// <para>Returns a transaction.</para>
        /// </summary>
        /// <param name="context">
        /// <para>Current DataContext.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a transaction.</para>
        /// </returns>
        public static DbTransaction BeginTransactionMtn(this DataContext context)
        {
            var cnn = context.Connection;
            if (cnn.State == ConnectionState.Broken || cnn.State == ConnectionState.Closed)
                cnn.Open();
            var tran = cnn.BeginTransaction();
            return tran;
        }
        #endregion

        #region NewDataContext
       
        /// <summary>
        /// <para>Return a opened DataContext copy to use in queries.Very usefull to set another transaction type, isolating the original DataContext. </para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to use.</para>
        /// </param>
        /// <returns>
        /// <para>Return a opened DataContext copy to use in queries.Very usefull to set another transaction type, isolating the original DataContext. </para>
        /// </returns>
        public static DataContext NewDataContextMtn<TEntity>(this Table<TEntity> table, DbTransaction transaction = null) where TEntity : class
        {
            return NewDataContextMtn(table.Context, transaction);
        }

        /// <summary>
        /// <para>Return a opened DataContext copy to use in queries.Very usefull to set another transaction type, isolating the original DataContext. </para>
        /// </summary>
        /// <param name="context">
        /// <para>Current DataContext.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to use.</para>
        /// </param>
        /// <returns>
        /// <para>Return a opened DataContext copy to use in queries.Very usefull to set another transaction type, isolating the original DataContext. </para>
        /// </returns>
        public static DataContext NewDataContextMtn(this DataContext context, DbTransaction transaction = null)
        {
            var cnn = context.Connection;
            try
            {
                if (cnn.State == ConnectionState.Broken || cnn.State == ConnectionState.Closed)
                    cnn.Open();
            }
            catch
            {
                try
                {
                    cnn.Close();
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
                cnn.Dispose();
                cnn = context.Connection;
                if (cnn.State == ConnectionState.Broken || cnn.State == ConnectionState.Closed)
                    cnn.Open();
            }
            DataContext newDataContext = new DataContext(cnn, context.Mapping.MappingSource);

            if (transaction != null)
                newDataContext.Transaction = transaction;

            return newDataContext;
        }
        #endregion

        #region Insert

        /// <summary>
        /// <para>Inserts a entity on table (if don't have a primary key, executes a "insert into" directly on the base).</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="entity">
        /// <para>Entity to insert.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to use.</para>
        /// </param>
        /// <param name="rollbackOnError">
        /// <para>Indicates if must execute a transaction rollback if occur any error.</para>
        /// </param>
        /// <param name="commitOnSuccess">
        /// <para>Indicates if must execute a transaction commit if succes.</para>
        /// </param>
        /// <param name="rethrow">
        /// <para>Indicates if must rethrow the exception.</para>
        /// </param>
        /// <returns>
        /// <para>Returns true if succeed.</para>
        /// </returns>
        public static Boolean InsertMtn<TEntity>(this Table<TEntity> table, TEntity entity, DbTransaction transaction = null, Boolean rollbackOnError = false, Boolean commitOnSuccess = true, Boolean rethrow = true) where TEntity : class
        {
            try
            {
                var retVal = InsertMtn(table, entity, transaction);
                if (commitOnSuccess)
                    transaction.Commit();
                return retVal;
            }
            catch (Exception ex)
            {
                if (rollbackOnError)
                    transaction.Rollback();

                if (rethrow)
                    throw ex;

                return false;
            }
        }

        /// <summary>
        /// <para>Returns a opened DataContext.</para>
        /// </summary>
        /// <param name="context">
        /// <para>Current DataContext.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to use.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a opened DataContext.</para>
        /// </returns>
        public static DataContext GetOpenedDataContextMtn(this DataContext context, DbTransaction transaction = null)
        {
            var cnn = context.Connection;
            try
            {
                if (cnn.State == ConnectionState.Broken || cnn.State == ConnectionState.Closed)
                    cnn.Open();
            }
            catch
            {
                try
                {
                    cnn.Close();
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
                cnn.Dispose();
                cnn = context.Connection;
                if (cnn.State == ConnectionState.Broken || cnn.State == ConnectionState.Closed)
                    cnn.Open();
            }

            if (transaction != null)
                context.Transaction = transaction;

            return context;
        }

        /// <summary>
        /// <para>Inserts a entity on table (if don't have a primary key, executes a "insert into" directly on the base).</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type.</para>
        /// </param>
        /// <param name="entity">
        /// <para>Entity to insert.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction to use.</para>
        /// </param>
        /// <returns>
        /// <para>Returns true if succeed.</para>
        /// </returns>
        public static Boolean InsertMtn<TEntity>(this Table<TEntity> table, TEntity entity, DbTransaction transaction = null) where TEntity : class
        {
            var context = table.Context.NewDataContextMtn(transaction);

            var mapping = context.Mapping.GetTable(typeof(TEntity));
            var tableCtx = context.GetTable<TEntity>();
            List<System.Data.Linq.Mapping.MetaDataMember> pkfield = null;
            try
            {
                pkfield = mapping.RowType.DataMembers.Where(d => d.IsPrimaryKey).ToList();
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }

            if (pkfield != null && pkfield.Count > 0)
            {
                tableCtx.InsertOnSubmit(entity);
                context.SubmitChanges();
                context.Dispose();
                return true;
            }
            else
            {
                StringBuilder cmdQuery = new StringBuilder("INSERT INTO " + table.GetDbNameMtn() + " (");
                StringBuilder cmdValue = new StringBuilder(" VALUES(");
                List<object> lstParms = new List<object>();
                bool firstInsert = true;
                int nIdxParm = 0;
                for (int i = 0; i < mapping.RowType.DataMembers.Count; i++)
                {
                    var res = entity.GetType().GetProperty(mapping.RowType.DataMembers[i].Name).GetValue(entity, null);
                    var attrCol = (System.Data.Linq.Mapping.ColumnAttribute)entity.GetType().GetProperty(mapping.RowType.DataMembers[i].Name).GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), false).FirstOrDefault();
                    if (res != null && attrCol != null && (attrCol.AutoSync == System.Data.Linq.Mapping.AutoSync.Default || attrCol.AutoSync == System.Data.Linq.Mapping.AutoSync.Always))
                    {
                        lstParms.Add(res);
                        cmdQuery.Append((firstInsert ? "" : ",") + mapping.RowType.DataMembers[i].Name);
                        cmdValue.Append(value: (firstInsert ? "" : ",") + "{" + nIdxParm.ToString() + "}");
                        nIdxParm++;
                        firstInsert = false;
                    }
                }
                cmdQuery.Append(")");
                cmdValue.Append(")");
                cmdQuery.Append(cmdValue);
                context.ExecuteCommand(cmdQuery.ToString(), lstParms.ToArray());
                context.Dispose();
                return true;
            }
        }
        #endregion

        #region Clone

        /// <summary>
        /// <para>Return a clone from the original entity.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="entity">
        /// <para>Entity to be cloned.</para>
        /// </param>
        /// <param name="nullableProperties">
        /// <para>Delimited string by comma with the names of properties to be set to null.</para>
        /// </param>
        /// <returns>
        /// <para>Return a clone from the original entity.</para>
        /// </returns>
        public static TEntity CloneMtn<TEntity>(this TEntity entity, String nullableProperties = null)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(TEntity));
            using (var ms = new System.IO.MemoryStream())
            {
                ser.WriteObject(ms, entity);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                TEntity retVal = (TEntity)ser.ReadObject(ms);

                if (nullableProperties.IsNullOrEmptyMtn(true) == false)
                {
                    foreach (string name in nullableProperties.SplitMtn(","))
                    {
                        retVal.GetType().GetProperty(name).SetValue(retVal, null, null);
                    }
                }

                return retVal;
            }
        }
        #endregion

        #region Where

        /// <summary>
        /// <para>Returns the query using the specified transaction.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type .</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction.</para>
        /// </param>
        /// <param name="filter">
        /// <para>Filter to query based on predicate.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the query using the specified transaction.</para>
        /// </returns>
        public static IQueryable<TEntity> WhereMtn<TEntity>(this Table<TEntity> table, DbTransaction transaction, Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            DataContext context = table.NewDataContextMtn<TEntity>(transaction);

            var tableQuery = context.GetTable<TEntity>();
            return tableQuery.Where(filter);
        }
        #endregion

        #region Count

        /// <summary>
        /// <para>Returns the number of elements in a sequence using the specified transaction.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="table">
        /// <para>Represents a table for a particular type .</para>
        /// </param>
        /// <param name="transaction">
        /// <para>Transaction.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the number of elements in a sequence using the specified transaction.</para>
        /// </returns>
        public static int CountMtn<TEntity>(this Table<TEntity> table, DbTransaction transaction = null) where TEntity : class
        {
            DataContext context = table.NewDataContextMtn<TEntity>(transaction);
            var tableQuery = context.GetTable<TEntity>();

            return tableQuery.Count();
        }
        #endregion

        #endregion
    }
}
