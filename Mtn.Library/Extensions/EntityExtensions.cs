using System;
using System.Collections.Generic;
using System.Data.Common.CommandTrees;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Objects;
using System.Data.Metadata.Edm;
using System.Data.Objects.DataClasses;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Mtn.Library.Extensions;
namespace Mtn.Library.ExtensionsEntity
{
    /// <summary>
    ///  Extensions for Entity Framework
    /// </summary>
    public static class EntityExtensions
    {
        #region EDMX
        #region Insert

        /// <summary>
        ///Inserts a entity on table and commit changes.
        /// </summary>
        /// <typeparam name="TEntity">
        ///Entity class.
        /// </typeparam>
        /// <param name="table">
        ///Represents a table for a particular type.
        /// </param>
        /// <param name="entity">
        ///Entity to insert.
        /// </param>
        /// <returns>
        ///Returns true if succeed.
        /// </returns>
        public static bool InsertMtn<TEntity>(this ObjectQuery<TEntity> table, TEntity entity) where TEntity : class
        {
            try
            {
                var context = table.Context;
                var entBase = context.GetEntitySetMtn(entity.GetType());
                context.AddObject(entBase.Name, entity);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        private static EntitySetBase GetEntitySetMtn(this ObjectContext context, Type entityType)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            EntityContainer container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);

            EntitySetBase entitySetBase = container.BaseEntitySets.FirstOrDefault(item => item.ElementType.Name.Equals(entityType.Name));

            return entitySetBase;
        }
        #endregion

        #region DeleteMtn
        /// <summary>
        ///Deletes a record from table and commit changes).
        /// </summary>
        /// <typeparam name="TEntity">
        ///Entity class.
        /// </typeparam>
        /// <param name="table">
        ///Represents a table for a particular type.
        /// </param>
        /// <param name="entity">
        ///Record to delete.
        /// </param>
        /// <returns>
        ///Returns true if succeed.
        /// </returns>
        public static bool DeleteMtn<TEntity>(this ObjectQuery<TEntity> table, TEntity entity) where TEntity : class
        {
            try
            {
                var context = table.Context;
                context.DeleteObject(entity);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region UpdateMtn
        /// <summary>
        ///Update a record on table and commit changes.
        /// </summary>
        /// <typeparam name="TEntity">
        ///Entity class.
        /// </typeparam>
        /// <param name="table">
        ///Represents a table for a particular type.
        /// </param>
        /// <param name="entity">
        ///Entity to update.
        /// </param>
        /// <returns>
        ///Returns true if succeed.
        /// </returns>
        public static bool UpdateMtn<TEntity>(this ObjectQuery<TEntity> table, TEntity entity) where TEntity : class
        {
            try
            {
                var context = table.Context;
                context.ObjectStateManager.GetObjectStateEntry(entity).SetModified();
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

        #region GetPrimaryKey

        private static PropertyInfo GetPrimaryKey<T>(Boolean force=false)
        {
            PropertyInfo[] infos = typeof(T).GetProperties();
            PropertyInfo pkProperty = (from info in infos let column = info.GetCustomAttributes(false).Where(x => x is EdmScalarPropertyAttribute).FirstOrDefault(x => ((EdmScalarPropertyAttribute) x).EntityKeyProperty && ((EdmScalarPropertyAttribute) x).IsNullable == false) where column != null select info).FirstOrDefault();
            if (pkProperty == null)
            {
                if (force)
                {
                    throw new NotSupportedException(
                        typeof (T).ToString() + " has no Primary Key");
                }
                else
                {
                    return infos.FirstOrDefault();
                }
            }
            return pkProperty;
        }

        private static MemberExpression GetPrimaryKeyMember<T>(Boolean force = false)
        {
            PropertyInfo[] infos = typeof(T).GetProperties();
            PropertyInfo pkProperty = (from info in infos let column = info.GetCustomAttributes(false).Where(x => x is EdmScalarPropertyAttribute).FirstOrDefault(x => ((EdmScalarPropertyAttribute)x).EntityKeyProperty && ((EdmScalarPropertyAttribute)x).IsNullable == false) where column != null select info).FirstOrDefault();
            if (pkProperty == null)
            {
                if (force)
                {
                    throw new NotSupportedException(
                        typeof (T).ToString() + " has no Primary Key");
                }
                else
                {
                    pkProperty = infos.FirstOrDefault();
                }
            }
            
            // Create entity => portion of lambda expression
            ParameterExpression parameter = Expression.Parameter(typeof(T), "entity");

            // create entity.Id portion of lambda expression
            MemberExpression property = Expression.Property(parameter, pkProperty.Name);
            return property;
        }

        /// From http://dotnetspeak.com/2013/09/use-reflection-and-expression-to-find-an-entity-by-primary-key, thanks to Sergey Barskiy
        /// <summary>
        /// Create expression for .Where(entity => entity.Id == 'id')
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="id">Entity ID</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetExpression<T>(object id)
        {
            // Find primary key property based on primary key attribute.
            var keyProperty = typeof(T).GetProperties().
                First(
                    one =>
                    one.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), true)
                    .Any(two => ((EdmScalarPropertyAttribute)two).EntityKeyProperty));

            // Create entity => portion of lambda expression
            ParameterExpression parameter = Expression.Parameter(typeof(T), "entity");

            // create entity.Id portion of lambda expression
            MemberExpression property = Expression.Property(parameter, keyProperty.Name);

            // create 'id' portion of lambda expression
            var equalsTo = Expression.Constant(id);

            // create entity.Id == 'id' portion of lambda expression
            var equality = Expression.Equal(property, equalsTo);

            // finally create entire expression - entity => entity.Id == 'id'
            Expression<Func<T, bool>> retVal =
                Expression.Lambda<Func<T, bool>>(equality, new[] { parameter });
            return retVal;
        }
        #endregion

        #region GetMtn
        /// <summary>
        ///Returns a Entity using primary key.
        /// </summary>
        /// <typeparam name="TEntity">
        ///Represents the object type for class.
        /// </typeparam>
        /// <param name="context">
        ///Current DataContext.
        /// </param>
        /// <param name="pk">
        ///Record key.
        /// </param>
        /// <returns>
        ///Returns a Entity using primary key.
        /// </returns>
        public static TEntity GetMtn<TEntity>(this ObjectContext context, object pk) where TEntity : class
        {
            PropertyInfo info = GetPrimaryKey<TEntity>();
            var typeName = context.DefaultContainerName + "." + typeof(TEntity).Name;
            var key = new EntityKey(typeName, info.Name, pk);

            var result = (TEntity)context.GetObjectByKey(key);
            return result;
        }

        /// <summary>
        ///Returns a Entity using primary key.
        /// </summary>
        /// <typeparam name="TEntity">
        ///Represents the object type for class.
        /// </typeparam>
        /// <param name="table">
        ///Represents a table for a particular type.
        /// </param>
        /// <param name="pk">
        ///Record key.
        /// </param>
        /// <returns>
        ///Returns a Entity using primary key.
        /// </returns>
        public static TEntity GetMtn<TEntity>(this ObjectQuery<TEntity> table, object pk) where TEntity : class
        {
            PropertyInfo info = GetPrimaryKey<TEntity>();
            if (info == null)
                throw new Exception(String.Format("Table {0} does not contain a Primary Key field", table.Name));
            var param = Expression.Parameter(typeof(TEntity), "e");
            var predicate = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.Property(param, info.Name), Expression.Constant(pk)), param);
            TEntity result = table.FirstOrDefault(predicate);

            return result;
        }
        #endregion

        #region DataPage

          /// <summary>
        /// Returns a paginated DataPage of TEntity.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Represents the object type for DataPage.
        /// </typeparam>
        /// <param name="table">
        /// Represents a table for a particular type.
        /// </param>
        /// <param name="page">
        /// The number of the page you requested (starting from page 1).
        /// </param>
        /// <param name="recordsPerPage">
        /// The maximum number of items per page.
        /// </param>
        /// <param name="expression">
        /// A Lambda expression returning a <typeparamref name="TEntity"/>.
        ///</param>
        /// <returns>
        /// Returns a paginated DataPage of TEntity.
        /// </returns>
        /// <remarks>
        /// If the page is zero or less then zero will be treated as one "1".
        /// </remarks>
        public static Mtn.Library.Entities.DataPage<TEntity> GetPageMtn<TEntity>
            (this IQueryable<TEntity> table, Int32 page, Int32 recordsPerPage, Expression<Func<TEntity, Boolean>> expression = null) where TEntity : class
        {
            Expression<Func<TEntity, bool>> orderBy=null;
            return table.GetPageMtn<TEntity,bool>(page, recordsPerPage, expression, orderBy);
        }
        /// <summary>
        /// Returns a paginated DataPage of TEntity.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Represents the object type for DataPage.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The object type used for order by purposes.
        /// </typeparam>
        /// <param name="table">
        /// Represents a table for a particular type.
        /// </param>
        /// <param name="page">
        /// The number of the page you requested (starting from page 1).
        /// </param>
        /// <param name="recordsPerPage">
        /// The maximum number of items per page.
        /// </param>
        /// <param name="expression">
        /// A Lambda expression returning a <typeparamref name="TEntity"/>.
        ///</param>
        ///<param name="orderBy">
        /// A Lambda expression returning a filter order by.
        ///</param>
        /// <returns>
        /// Returns a paginated DataPage of TEntity.
        /// </returns>
        /// <remarks>
        /// If the page is zero or less then zero will be treated as one "1".
        /// </remarks>
        public static Mtn.Library.Entities.DataPage<TEntity> GetPageMtn<TEntity, TKey>
            (this IQueryable<TEntity> table, Int32 page, Int32 recordsPerPage, Expression<Func<TEntity, Boolean>> expression, Expression<Func<TEntity, TKey>> orderBy) where TEntity : class
        {
            try
            {
                page = (page > 0) ? page - 1 : 0;
                var query = table.AsQueryable();
                if (expression != null)
                    query = query.Where(expression);
                var tableSafeName = table.GetType().FullName.ToSafeStringMtn();
                if (orderBy != null)
                    query = query.OrderBy(orderBy);
                else if (tableSafeName.StartsWith("System.Data.Entity.Infrastructure.DbQuery") || tableSafeName.StartsWith("Mtn.Library.Extensions.ExpandableQuery"))
                {
                    if (!table.ToString().ToSafeStringMtn().ToLower().Contains("order by"))
                    {
                        var pk = GetPrimaryKey<TEntity>();
                        if(pk != null)
                            query = query.OrderByMtn(pk.Name);
                    }
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
        #endregion

        #region DumpCsvMtn

        /// <summary>
        /// Creates a *.csv file from an IQueryable query, dumping out the 'simple' properties/fields.
        /// </summary>
        /// <param name="query">Represents a SELECT query to execute.</param>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="deleteFile">Whether or not to delete the file specified by <paramref name="fileName"/> if it exists.</param>
        /// <param name="delimiter">Another delimiter to replace the default (,) .</param>
        /// <param name="createHeader">Whether or not insert header (property name) on file.</param>
        /// <param name="encoding"></param>
        /// <remarks>
        /// If the <paramref name="query"/> contains any properties that are entity sets (i.e. rows from a FK relationship) the values will not be dumped to the file.
        /// </remarks>
        public static void DumpCsvMtn(this IQueryable query, String fileName, Boolean deleteFile = true, String delimiter = ",", Boolean createHeader = true, Encoding encoding = null)
        {
            if (delimiter.IsNullOrEmptyMtn(false))
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
                                        writer.Write(delimiter);
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

        #region DumpCsvStringMtn

        /// <summary>
        /// Creates a csv delimited file from an IQueryable query, dumping out the 'simple' properties/fields.
        /// </summary>
        /// <param name="query">Represents a SELECT query to execute.</param>
        /// <param name="delimiter">Another delimiter to replace the default (,) .</param>
        /// <param name="createHeader">Whether or not insert header (property name) on file.</param>
        /// <param name="info">Culture</param>
        /// <remarks>
        /// If the <paramref name="query"/> contains any properties that are entity sets (i.e. rows from a FK relationship) the values will not be dumped to the file.
        /// </remarks>
        public static StringBuilder DumpCsvStringMtn(this IQueryable query, String delimiter = ",", Boolean createHeader = true, CultureInfo info = null)
        {
            if (string.IsNullOrEmpty(delimiter))
                delimiter = ",";
            
                using (var writer = new StringWriter(info??CultureInfo.CurrentCulture))
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
                                        writer.Write(delimiter);
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
                            DumpValueStringMtn(p.GetValue(r, null), writer, delimiter);
                        }
                        writer.WriteLine();
                    }
                    return writer.GetStringBuilder();

                }

        }
        #endregion

        #region DumpValue
        private static void DumpValueStringMtn(Object value, StringWriter writer, String delimiter)
        {
            if (value != null)
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    // csv encode the value
                    case TypeCode.String:
                        string convValue = (string)value;
                        if (convValue.Contains(delimiter) || convValue.Contains('"') || convValue.Contains("\n"))
                        {
                            convValue = convValue.Replace("\"", "\"\"");

                            if (convValue.Length > 31735)
                            {
                                convValue = convValue.Substring(0, 31732) + "...";
                            }
                            writer.Write("\"" + convValue + "\"");
                        }
                        else
                        {
                            writer.Write(convValue);
                        }
                        break;

                    default:
                        writer.Write(value);
                        break;
                }
            }
        }
        #endregion
        #region DumpHtmlMtn
        /// <summary>
        /// Html body template
        /// </summary>
        public const String DUMP_HTML_BODY_TEMPLATE = @"
<!DOCTYPE html><html>
	<head>
		<title>Demo</title>
		<meta charset='utf-8'>		
		<style type='text/css'>
            .mtn-head {{style-head}}
            .mtn-row {{style-row}}
            .mtn-row .odd {{style-row-odd}}
            .mtn-row .even {{style-row-even}}
            .mtn-cell {{style-cell} }

        </style>
	</head>
	<body>
		{body}
	</body>	
</html>";
        /// <summary>
        /// 
        /// </summary>
        private const String DumpHtmlHeadStyle = "background-color:#0F0;color:FFF";
        /// <summary>
        /// 
        /// </summary>
        private const String DumpHtmlRowStyle = "";
        /// <summary>
        /// 
        /// </summary>
        private const String DumpHtmlRowOddStyle = "";
        /// <summary>
        /// 
        /// </summary>
        private const String DumpHtmlRowEvenStyle = "";
        /// <summary>
        /// 
        /// </summary>
        private const String DumpHtmlCellStyle = "";
        /// <summary>
        /// 
        /// </summary>
        private const String DumpHtmlContainerTemplate = @"<table>{content}</table>";
        /// <summary>
        /// 
        /// </summary>
        private const String DumpHtmlRowTemplate = "<tr class='mtn-row {checkpair}'>{row}</tr>";
        /// <summary>
        /// 
        /// </summary>
        private const String DumpHtmlCellTemplate = "<td class='mtn-cell'>{cell}</td>";


        /// <summary>
        /// Creates a *.html file from an IQueryable query, dumping out the 'simple' properties/fields.
        /// </summary>
        /// <param name="query">Represents a SELECT query to execute.</param>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="deleteFile">Whether or not to delete the file specified by <paramref name="fileName"/> if it exists.</param>
        /// <param name="createHeader">Whether or not insert header (property name) on file.</param>
        /// <param name="encoding">encoding to file</param>
        /// <param name="body">Html Body</param>
        /// <param name="headStyle">Style for header</param>
        /// <param name="rowStyle">Style for row</param>
        /// <param name="cellStyle">Style for cell</param>
        /// <param name="rowOddStyle">Style for odd cell</param>
        /// <param name="rowEvenStyle">Stule for even cell</param>
        /// <param name="containerTemplate">Container template, default is html table</param>
        /// <param name="rowTemplate">template for row, default is tr</param>
        /// <param name="cellTemplate">template for cell, default is td</param>
        /// <param name="onlyReturnData">If true, will not create a file, only return data, otherwise will not return content for priorize performance</param>
        /// <remarks>
        /// If the <paramref name="query"/> contains any properties that are entity sets (i.e. rows from a FK relationship) the values will not be dumped to the file.
        /// </remarks>
        /// 
        public static String DumpHtmlMtn(this IQueryable query, String fileName="", Boolean deleteFile = true, Boolean createHeader = true, Encoding encoding = null,
            String body = DUMP_HTML_BODY_TEMPLATE, String headStyle = DumpHtmlHeadStyle, String rowStyle = DumpHtmlRowStyle, String cellStyle = DumpHtmlCellStyle,
            String rowOddStyle = DumpHtmlRowOddStyle, String rowEvenStyle = DumpHtmlRowEvenStyle,
            String containerTemplate = DumpHtmlContainerTemplate, String rowTemplate = DumpHtmlRowTemplate, String cellTemplate = DumpHtmlCellTemplate, bool onlyReturnData=false)
        {

            String sBody = body.Replace("{style-cell}", cellStyle).Replace("{style-row-even}", rowEvenStyle).Replace("{style-row-odd}", rowOddStyle)
                .Replace("{style-row}", rowStyle).Replace("{style-head}", headStyle);
            String sContainer = containerTemplate;
            String sRow = rowTemplate;
            String sCell = cellTemplate;
            var shtml = new StringBuilder();

            var isEven = false;
            var firstRow = true;
            PropertyInfo[] properties = null;
            Type type = null;

            foreach (var r in query)
            {
                var sRowTmp = new StringBuilder();
                if (type == null)
                {
                    type = r.GetType();
                    properties = type.GetProperties();
                }

                if (createHeader)
                {
                    if (firstRow)
                    {   
                        foreach (var p in properties)
                        {
                            sRowTmp.Append(sCell.Replace("{cell}", p.Name ?? ""));
                        }
                        shtml.AppendLine(sRow.Replace("{row}", sRowTmp.ToString()).Replace("{checkpair}", ""));
                    }
                }

                firstRow = false;
                sRowTmp = new StringBuilder();
                foreach (var p in properties)
                {
                    var pVal = p.GetValue(r, null);
                    sRowTmp.Append(sCell.Replace("{cell}", pVal == null ? "" : pVal.ToString()));
                }
                shtml.AppendLine(sRow.Replace("{row}", sRowTmp.ToString()).Replace("{checkpair}", isEven?"even":"odd"));
                isEven = !isEven;
            }

            var result = sBody.Replace("{body}", sContainer.Replace("{content}", shtml.ToString()));

            if (onlyReturnData) return result;
            if (File.Exists(fileName) && deleteFile)
            {
                File.Delete(fileName);
            }

            if (encoding == null)
                encoding = new UTF8Encoding();
            var filemode = deleteFile ? FileMode.CreateNew : FileMode.OpenOrCreate;
            using (var output = new FileStream(fileName, filemode))
            {
                using (var writer = new StreamWriter(output, encoding))
                {
                    writer.Write(result);
                }
            }

            return result;
        }
        #endregion

        #region DumpValue
        private static void DumpValueMtn(Object value, StreamWriter writer, String delimiter)
        {
            if (value != null)
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    // csv encode the value
                    case TypeCode.String:
                        string convValue = (string)value;
                        if (convValue.Contains(delimiter) || convValue.Contains('"') || convValue.Contains("\n"))
                        {
                            convValue = convValue.Replace("\"", "\"\"");

                            if (convValue.Length > 31735)
                            {
                                convValue = convValue.Substring(0, 31732) + "...";
                            }
                            writer.Write("\"" + convValue + "\"");
                        }
                        else
                        {
                            writer.Write(convValue);
                        }
                        break;

                    default:
                        writer.Write(value);
                        break;
                }
            }
        }
        #endregion

        #region ToSqlStringMtn

        /// <summary>
        /// Returns the query made in the database for the Entity Framework
        /// </summary>
        /// <param name="query">
        /// Query.
        /// </param>
        /// <returns>
        /// Returns the query made in the database for the Entity Framework
        /// </returns>
        public static string ToSqlStringMtn(this IQueryable query)
        {
            var objectQuery = query as ObjectQuery;
            return objectQuery != null ? objectQuery.ToTraceString() : null;
        }
        #endregion

        #region TruncateTableMtn

        /// <summary>
        /// Performs a truncate table on MS SQL Server
        /// </summary>
        /// <typeparam name="TEntity">Represents the object type for class.</typeparam>
        /// <param name="table">Represents a table for a particular type in the underlying database to be truncated.</param>
        /// <param name="name">The table name.</param>
        /// <returns>
        /// Returns false if not be able to truncate.
        /// </returns>
        public static bool TruncateTableMtn<TEntity>(this ObjectQuery<TEntity> table, String name = null) where TEntity : class
        {
            try
            {
                name = name ?? table.Name;
                string truncate = string.Format("TRUNCATE TABLE {0}\r\n", name);
                object[] parms = new object[] { };
                int result = table.Context.ExecuteStoreCommand(truncate, parms);
                return result > 0;
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }
            return false;
        }

        #endregion

        #region CloneMtn

        /// <summary>
        /// Return a clone from the original entity.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity class.
        /// </typeparam>
        /// <param name="entity">
        /// Entity to be cloned.
        /// </param>
        /// <param name="nullableProperties">
        /// Delimited string by comma with the names of properties to be set to null.
        /// </param>
        /// <returns>
        /// Return a clone from the original entity.
        /// </returns>
        public static TEntity CloneMtn<TEntity>(this TEntity entity, String nullableProperties=null)
        {
            var ser = new DataContractSerializer(typeof(TEntity));
            using (var ms = new System.IO.MemoryStream())
            {
                ser.WriteObject(ms, entity);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                var retVal = (TEntity)ser.ReadObject(ms);

                if (nullableProperties.IsNullOrWhiteSpaceMtn()) return retVal;
                
                foreach (var name in nullableProperties.SplitMtn(","))
                    retVal.GetType().GetProperty(name).SetValue(retVal, null, null);

                return retVal;
            }
        }
        #endregion

        #region CastMtn

        /// <summary>
        /// Return a clone from the original entity.
        /// </summary>
        /// <typeparam name="TOtherEntity">
        /// Entity class type to do cast.
        /// </typeparam>
        /// <param name="entity">
        /// Entity to be casted.
        /// </param>
        /// <param name="types">
        /// types for costructor.
        /// </param>
        /// <param name="arguments">
        /// arguments for constructor.
        /// </param>
        /// <returns>
        /// Return a clone from the original entity.
        /// </returns>
        public static TOtherEntity CastMtn<TOtherEntity>(this object entity, Type[] types = null, object[] arguments = null)
        {
            if (entity.IsPrimitiveMtn())
                throw new ArgumentOutOfRangeException("entity", "Cast cannot be used for primitives type, without constructor");
            if (types == null)
                types = new Type[] {};

            var type = typeof (TOtherEntity);
            var props = type.GetProperties();
            
            var construct = type.GetConstructor(types);
            if (construct == null)
                throw new ArgumentOutOfRangeException("types", "Cannot find constructor for this types");

            if (arguments == null)
                arguments = new object[] {};
            var ret = (TOtherEntity)construct.Invoke(arguments);

            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (ret == null)
                throw new ArgumentOutOfRangeException("arguments", "Cannot invoke constructor withs this arguments");

            foreach (var prop in entity.GetType().GetProperties())
            {
                var name = prop.Name;

                if (!props.Any(x => x.Name.Equals(name) && x.PropertyType == prop.PropertyType)) continue;

                if(!entity.GetType().GetProperty(name).CanRead || !ret.GetType().GetProperty(name).CanWrite)
                    continue;

                var retVal = entity.GetType().GetProperty(name).GetValue(entity,null);
                if (retVal == null)
                    continue;
                ret.GetType().GetProperty(name).SetValue(ret, retVal, null);
            }
            return ret;
        }
        #endregion

        #region OrderByMtn
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


        /// <summary>
        /// Returns a query ordered by according to the parameters.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity class.
        /// </typeparam>
        /// <param name="query">
        /// Represents a query for a particular type.
        /// </param>
        /// <param name="predicates">
        /// Predicate collection generated by the predicate builder to Order By.
        ///</param>
        /// <returns>
        /// Returns a query ordered by according to the parameters.
        /// </returns>
        public static IQueryable<TEntity> OrderByManyMtn<TEntity>
          (this IQueryable<TEntity> query, params PredicateBuilder.OrderByMtn[] predicates)
        {
            var lstPredcate = predicates.Reverse();

            foreach (var predicate in lstPredcate.ToList())
            {
                query = OrderByMtn(query, predicate);
            }
            return query;
        }

        #endregion

        #region Predicate
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> TrueMtn<T>() { return f => true; }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> FalseMtn<T>() { return f => false; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> OrMtn<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndMtn<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

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

        /// <summary>
        /// <para>Through each element in source and performs the action method as parameter passing this element.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Type of IQueryable.</para>
        /// </typeparam>
        /// <param name="source">
        /// <para>IQueryable to be traversed.</para>
        /// </param>
        /// <param name="action">
        /// <para>Action to be executed.</para>
        /// </param>
        public static void ForEachMtn<T>(this IQueryable source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
        #endregion

        #endregion
    }
}
