using ContractorsDesk.DataStore.Client.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Hangfire.@base
{
    public class Base
    {
        protected readonly IConfiguration Configuration;
        private readonly string ConnectionString;
		public ClientDbContext clientDbContext { get; set; }

		protected Base(
            IConfiguration configuration,
			ClientDbContext clientDbContext = null)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetConnectionString("default");
            this.clientDbContext = clientDbContext;
		}

        /// <summary>
        /// Execute SQL stored procedure.
        /// </summary>
        public void ExecuteStoredProcedure(string storeProcedureName)
        {
            ExecuteStoredProcedure(storeProcedureName, Array.Empty<SqlParameter>());
        }

        /// <summary>
        /// Executes an SQL stored procedure with the given parameters.
        /// </summary>
        /// <param name="storeProcedureName">The name of the stored procedure.</param>
        /// <param name="parameters">A collection of <see cref="SqlParameter"/> objects to pass to the procedure.</param>
        public void ExecuteStoredProcedure(string storeProcedureName, IEnumerable<SqlParameter> parameters)
        {
            if (string.IsNullOrWhiteSpace(storeProcedureName))
                throw new ArgumentNullException(nameof(storeProcedureName), "Stored procedure name cannot be null or empty.");

            parameters ??= Array.Empty<SqlParameter>();

            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(storeProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            foreach (var param in parameters)
            {
                command.Parameters.Add(param);
            }

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
