using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	}
}
