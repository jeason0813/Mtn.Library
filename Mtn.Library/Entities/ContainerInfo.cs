using System;
using System.Collections.Generic;
using System.Linq;
using Mtn.Library.ADO;

namespace Mtn.Library.Entities
{
    /// <summary>
    /// Container type for Mtn Data interface
    /// </summary>
    public enum DataContainerType
    {
        /// <summary>
        /// 
        /// </summary>
        Table = 1,
        /// <summary>
        /// 
        /// </summary>
        Procedure = 2,
        /// <summary>
        /// 
        /// </summary>
        View = 3,
        /// <summary>
        /// 
        /// </summary>
        Function = 4
    }
    /// <summary>
    /// 
    /// </summary>
    public class ContainerPropertyInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 Order { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int32 Scale { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int32 MaxLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int32 Precision { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbProviderType DBProviderType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String DbType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean AllowNulls { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean UseContains { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsReadOnly { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsPrimaryKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsAutoIncrement { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean HasSequence { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String SequenceCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Value { get; set; }
    }

    /// <summary>
    /// ContainerInfo
    /// </summary>
    public class ContainerInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int32 Order { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DataContainerType DataContainerType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Catalog { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Schema { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ContainerPropertyInfo> Properties { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ContainerInfo()
        {
            this.Properties = new List<ContainerPropertyInfo>();
        }
        /// <summary>
        /// 
        /// </summary>
        public IDataProvider Provider { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String ConnectionString { get; set; } 
    }

    /// <summary>
    /// 
    /// </summary>
    public class QueryResultRow
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 Id {get;set;}
        /// <summary>
        /// 
        /// </summary>
        public List<ContainerPropertyInfo> Columns { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public QueryResultRow()
        {
            this.Columns = new List<ContainerPropertyInfo>();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// 
        /// </summary>
        public List<QueryResultRow> Rows { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public QueryResult()
        {
            this.Rows = new List<QueryResultRow>();
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 Total
        {
            get
            {
                return Rows.Count();
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DataResult
    {
        /// <summary>
        /// 
        /// </summary>
        public QueryResult QueryResult { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean HasQuery
        {
            get
            {
                return (QueryResult != null && QueryResult.Total >0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsSucess { get; set; }        
        /// <summary>
        /// 
        /// </summary>
        public String MessageError { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Nullable<Int32> TranslatedMessage { get; set; }
    }

}
