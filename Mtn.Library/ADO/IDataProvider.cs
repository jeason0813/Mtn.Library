using System;
using System.Collections.Generic;
using Mtn.Library.Entities;

namespace Mtn.Library.ADO
{
    /// <summary>
    /// IDataProvider
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        ///     Connection string
        /// </summary>
        String ConnectionString { get; set; }

        /// <summary>
        ///     Returns if allow sql commands
        /// </summary>
        Boolean AllowSqlAnsi { get; }

        /// <summary>
        ///     To test connection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        Boolean TestConnection(String connectionString);

        /// <summary>
        ///     Get Schemma table, procedure, function, view etc
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        IList<ContainerInfo> GetSchemma(String connectionString);

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        DataResult Insert(ContainerInfo data);
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        DataResult Delete(ContainerInfo data);
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        DataResult Update(ContainerInfo data);
        /// <summary>
        /// Query
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        DataResult Query(ContainerInfo data);
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        DataResult Execute(ContainerInfo data);
        /// <summary>
        /// CreateTable
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        DataResult CreateTable(ContainerInfo data);
        /// <summary>
        /// DropTable
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataResult DropTable(String tableName);

        // If allow SqlAnsi
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="sqlcommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataResult Insert(String sqlcommand, object[] parameters);
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="sqlcommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataResult Update(String sqlcommand, object[] parameters);
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="sqlcommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataResult Delete(String sqlcommand, object[] parameters);
        /// <summary>
        /// Query
        /// </summary>
        /// <param name="sqlcommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataResult Query(String sqlcommand, object[] parameters);
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="sqlcommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataResult Execute(String sqlcommand, object[] parameters);

        /// <summary>
        ///     Returna a page from list
        /// </summary>
        /// <param name="container"></param>
        /// <param name="pageNumber"></param>
        /// <param name="recordsPerPage"></param>
        /// <returns></returns>
        DataPage<QueryResultRow> GetPage(ContainerInfo container, Int32 pageNumber, Int32 recordsPerPage);
    }
}