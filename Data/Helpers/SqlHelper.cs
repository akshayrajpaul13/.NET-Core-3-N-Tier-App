using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Data.Helpers
{
    public static class SqlHelper
    {
        /// <summary>
        /// Executes the named stored procedure, with any number of parameters
        /// </summary>
        public static void ExecuteStoredProcedure(this DbContext dbContext, string storedProcedureName, params object[] parameters)
        {
            var sql = CompileParameterList(storedProcedureName, parameters);
            dbContext.Database.ExecuteSqlRaw(sql, parameters);
        }

        ///// <summary>
        ///// Executes the named stored procedure, with any number of parameters
        ///// </summary>
        //public static IList<T> ExecuteStoredProcedureWithResults<T>(this DbContext dbContext, string storedProcedureName, params object[] parameters)
        //{
        //    var sql = CompileParameterList(storedProcedureName, parameters);
        //    return dbContext.Database.SqlQuery<T>(sql, parameters).ToList();
        //}

        private static string CompileParameterList(string storedProcedureName, object[] parameters)
        {
            var sb = new StringBuilder();
            sb.Append("exec ");
            sb.Append(storedProcedureName);
            sb.Append(" ");
            for (var i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append("@p" + i);
            }
            var sql = sb.ToString();
            return sql;
        }
    }
}
