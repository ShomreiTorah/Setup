using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Configurator {
	static class Database {
		static readonly Dictionary<string, DbProviderFactory> factoryNames = new Dictionary<string, DbProviderFactory>(StringComparer.OrdinalIgnoreCase){
			{ "SQL Server",		SqlClientFactory.Instance	},
			{ "SQLServer",		SqlClientFactory.Instance	},
			{ "OleDB",			OleDbFactory.Instance		},
			{ "ODBC",			OdbcFactory.Instance		},
		};

		public static readonly ReadOnlyCollection<string> Types =
			factoryNames.Keys.Except(new[] { "SQLServer" })
							 .OrderByDescending(s => s)
							 .ToList()
							 .AsReadOnly();

		public static DbConnection OpenConnection(this Schema.DatabaseConfig config) {
			var factory = factoryNames[config.Type];
			var retVal = factory.CreateConnection();
			retVal.ConnectionString = config.ConnectionString;
			retVal.Open();
			return retVal;
		}


		///<summary>Creates a DbCommand.</summary>
		///<param name="connection">The connection to create the command for.</param>
		///<param name="sql">The SQL of the command.</param>
		public static DbCommand CreateCommand(this DbConnection connection, string sql) {
			if (connection == null) throw new ArgumentNullException("connection");

			var retVal = connection.CreateCommand();
			retVal.CommandText = sql;
			return retVal;
		}

		///<summary>Creates a DbCommand.</summary>
		///<param name="transaction">The transaction with a connection to the database.  The connection is not closed.</param>
		///<param name="sql">The SQL of the command.</param>
		public static DbCommand CreateCommand(this DbTransaction transaction, string sql) {
			var command = transaction.Connection.CreateCommand(sql);
			command.Transaction = transaction;
			return command;
		}
		///<summary>Executes a SQL statement against a connection.</summary>
		///<param name="connection">The connection to the database.  The connection is not closed.</param>
		///<param name="sql">The SQL to execute.</param>
		///<returns>The number of rows affected by the statement.</returns>
		public static int ExecuteNonQuery(this DbConnection connection, string sql) {
			using (var command = connection.CreateCommand(sql)) return command.ExecuteNonQuery();
		}
		///<summary>Executes a SQL statement against a transaction.</summary>
		///<param name="transaction">The transaction with a connection to the database.  The connection is not closed.</param>
		///<param name="sql">The SQL to execute.</param>
		///<returns>The number of rows affected by the statement.</returns>
		public static int ExecuteNonQuery(this DbTransaction transaction, string sql) {
			using (var command = transaction.CreateCommand(sql)) return command.ExecuteNonQuery();
		}
		///<summary>Executes a SQL statement against a connection.</summary>
		///<param name="connection">The connection to the database.  The connection is not closed.</param>
		///<param name="sql">The SQL to execute.</param>
		///<returns>A DbDataReader object, which will close its underlying connection when disposed.</returns>
		public static DbDataReader ExecuteReader(this DbConnection connection, string sql) {
			return connection.CreateCommand(sql).ExecuteReader();
		}
	}
}
