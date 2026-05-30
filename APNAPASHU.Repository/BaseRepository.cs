#nullable disable
using System.Data;
using System.Data.SqlClient;
using APNAPASHU.DataContract.Enums;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace APNAPASHU.Repository
{
    /// <summary>
    /// Base Repository with common database operations
    /// </summary>
    public class BaseRepository
    {
        protected readonly IConfiguration Configuration;

        public BaseRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Returns first row of type T based on query parameter
        /// </summary>
        public async Task<T> GetFirstOrDefaultAsync<T>(string query, DynamicParameters parameter = null, CommandType? commandType = null, DataBaseNameEnum? databaseID = null)
        {
            try
            {
                T result = default;
                using (IDbConnection conn = GetConnection(databaseID))
                {
                    result = await conn.QueryFirstOrDefaultAsync<T>(query, parameter, null, null, commandType);
                }

                return result;
            }
            catch (Exception)
            {
                return default;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameter"></param>
        /// <param name="commandType"></param>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        public async Task<T> GetFirstOrDefaultAsyncWithSqlParam<T>(string query, SqlParameter parameter = null, CommandType? commandType = null, DataBaseNameEnum? databaseID = null)
        {
            T result = default;
            using (IDbConnection conn = GetConnection(databaseID))
            {
                result = await conn.QueryFirstOrDefaultAsync<T>(query, parameter, null, null, commandType);
            }

            return result;
        }

        /// <summary>
        /// This Repository is used for Update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameter"></param>
        /// <param name="commandType"></param>
        /// <param name="databaseID"></param>
        /// <returns></returns>
#pragma warning disable S4144 // Methods should not have identical implementations
        public async Task<T> UpdateAsync<T>(string query, DynamicParameters parameter = null, CommandType? commandType = null, DataBaseNameEnum? databaseID = null)
#pragma warning restore S4144 // Methods should not have identical implementations
        {
            T result = default;
            using (IDbConnection conn = GetConnection(databaseID))
            {
                try
                {
                    result = await conn.QueryFirstOrDefaultAsync<T>(query, parameter, null, null, commandType);
                }
                catch (System.Exception ex)
                {
                    string message = ex.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a list of type T based on query parameter
        /// </summary>
        public async Task<IEnumerable<T>> GetAsync<T>(string query, DynamicParameters parameters = null, CommandType? commandType = null, DataBaseNameEnum? databaseID = null)
        {
            IEnumerable<T> result = default;
            using (IDbConnection conn = GetConnection(databaseID))
            {
                try
                {
                    result = await conn.QueryAsync<T>(query, parameters, null, null, commandType);
                }
                catch (System.Exception ex)
                {
                    string message = ex.Message;

                }

            }

            return result;
        }


        /// <summary>
        /// This Repository for Add
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null, DataBaseNameEnum databaseID = DataBaseNameEnum.APNAPASHU)
        {
            int result = 0;

            using (IDbConnection conn = GetConnection(databaseID))
            {
                try
                {
                    result = await conn.ExecuteAsync(sql, parameters, null, null, commandType);
                }
                catch (System.Exception ex)
                {
                    string message = ex.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        public async Task<T> AddAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null, DataBaseNameEnum databaseID = DataBaseNameEnum.APNAPASHU)
        {
            T result = default;

            using (IDbConnection conn = GetConnection(databaseID))
            {
                result = await conn.QueryFirstOrDefaultAsync<T>(sql, parameters, null, null, commandType);
            }

            return result;
        }

        /// <summary>
        /// Gets the database connection using the configured connection string
        /// </summary>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        private IDbConnection GetConnection(DataBaseNameEnum? databaseID)
        {
            // First try DatabaseSettings:ConnectionString as it seems to be the intended path for this app
            string connectionString = Configuration.GetSection("DatabaseSettings").GetSection("ConnectionString").Value;
            
            // Fallback to DefaultConnection if DatabaseSettings:ConnectionString is missing
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = Configuration.GetConnectionString("DefaultConnection");
            }
            
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// This Repository for delete
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<int> DeleteAsync(string sql, object parameters = null, CommandType? commandType = null)
        {
            using (IDbConnection conn = GetConnection(DataBaseNameEnum.APNAPASHU))
            {
                return await conn.ExecuteAsync(sql, parameters, null, null, commandType);
            }
        }

        /// <summary>
        /// This Repository for delete
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<T> DeleteAsync<T>(string sql, object parameters = null, CommandType? commandType = null)
        {
            T result = default;
            using (IDbConnection conn = GetConnection(DataBaseNameEnum.APNAPASHU))
            {
                result = await conn.QueryFirstOrDefaultAsync<T>(sql, parameters, null, null, commandType);
            }
            return result;
        }

        public async Task<SqlMapper.GridReader> QueryMultiple(string query, DynamicParameters parameters = null, CommandType? commandType = null, DataBaseNameEnum databaseID = DataBaseNameEnum.APNAPASHU)
        {
            var conn = GetConnection(databaseID) as SqlConnection;
            await conn.OpenAsync();

            // Do NOT use 'using' here — we need the connection to remain open
            var multi = await conn.QueryMultipleAsync(query, parameters, commandType: commandType);
            return multi;
        }

        public async Task<(IEnumerable<T1>, IEnumerable<T2>)> QueryMultipleAsync<T1, T2>(
        string query,
        DynamicParameters parameters = null,
        CommandType? commandType = null,
        DataBaseNameEnum databaseID = DataBaseNameEnum.APNAPASHU)
        {
            try
            {
                using (var conn = GetConnection(databaseID) as SqlConnection)
                {
                    await conn.OpenAsync();

                    using (var multi = await conn.QueryMultipleAsync(query, parameters, commandType: commandType))
                    {
                        // ✅ Read first result set as single object
                        var result1 = (await multi.ReadAsync<T1>());

                        // ✅ Read second result set only if available
                        var result2 = (await multi.ReadAsync<T2>()).ToList();


                        // ✅ Now it's safe to return after disposing
                        return (result1, result2);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<T>> GetAsyncList<T>(string query, DynamicParameters parameters = null, CommandType? commandType = null, DataBaseNameEnum? databaseID = null)
        {
            try
            {
                List<T> result = default;
                using (IDbConnection conn = GetConnection(databaseID))
                {
                    result = (await conn.QueryAsync<T>(query, parameters, null, null, commandType)).ToList();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a list of type T based on query parameter
        /// </summary>
        public async Task<List<T>> GetAsyncSQLList<T>(string query, DynamicParameters parameters = null, CommandType? commandType = null, DataBaseNameEnum databaseID = DataBaseNameEnum.APNAPASHU)
        {
            List<T> result = default;
            using (IDbConnection conn = GetSqlConnection(databaseID))
            {
                result = (await conn.QueryAsync<T>(query, parameters, null, null, commandType)).ToList();
            }
            return result;
        }

        /// <summary>
        /// Gets the SQL connection using the configured connection string
        /// </summary>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        private IDbConnection GetSqlConnection(DataBaseNameEnum databaseID)
        {
            // First try DatabaseSettings:ConnectionString
            string connectionString = Configuration.GetSection("DatabaseSettings").GetSection("ConnectionString").Value;
            
            // Fallback to DefaultConnection
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = Configuration.GetConnectionString("DefaultConnection");
            }
            
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// To get single or default row of type T based on query parameters.
        /// </summary>
        /// <typeparam name="T">T For Generic Returns Type</typeparam>
        /// <param name="query">Query Name</param>
        /// <param name="parameter">Query Parameters</param>
        /// <param name="commandType">Sql Command Type</param>
        /// <param name="databaseID">Database ID</param>
        /// <returns></returns>
        public async Task<T> GetQuerySingleOrDefaultAsync<T>(string query, DynamicParameters parameter = null, CommandType? commandType = null, DataBaseNameEnum databaseID = DataBaseNameEnum.APNAPASHU)
        {
            T result = default;
            using (IDbConnection conn = GetConnection(databaseID))
            {
                result = await conn.QuerySingleOrDefaultAsync<T>(query, parameter, null, null, commandType);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAsyncEmpLList<T>(string query, DynamicParameters parameters = null,
            CommandType? commandType = null, DataBaseNameEnum databaseID = DataBaseNameEnum.APNAPASHU)
        {
            List<T> result = default;
            using (IDbConnection conn = GetSqlConnection(databaseID))
            {
                result = (await conn.QueryAsync<T>(query, parameters, null, null, commandType)).ToList();
            }
            return result;
        }

        /// <summary>
        /// Add bulk data into database using DataTable approach.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dataTable"></param>
        /// <param name="parameterName"></param>
        /// <param name="commandType"></param>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        public async Task<List<T>> AddBulkAsyncWithResponse<T>(string sql, DataTable dataTable, string parameterName, CommandType? commandType = null,
            DataBaseNameEnum databaseID = DataBaseNameEnum.APNAPASHU)
        {
            using (IDbConnection conn = GetConnection(databaseID))
            {
                var parameters = new DynamicParameters();
                parameters.Add(parameterName, dataTable.AsTableValuedParameter());

                var result = await conn.QueryAsync<T>(sql, parameters, commandType: commandType);

                return result.ToList();
            }
        }
    }
}