using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Configurator.Controls {
	/// <summary>
	/// Interaction logic for DatabaseSettings.xaml
	/// </summary>
	partial class DatabaseSettings : UserControl {
		public DatabaseSettings() {
			InitializeComponent();
		}

		private Schema.DatabaseConfig Config { get { return (Schema.DatabaseConfig)DataContext; } }

		private void TestConnection_Click(object sender, RoutedEventArgs e) {
			try {
				using (var connection = Config.OpenConnection()) {
					var tables = connection.ExecuteReader("SELECT s.name + '.' + o.name FROM sys.objects o JOIN sys.schemas s ON o.schema_id = s.schema_id WHERE type = 'U'")
						 .Cast<IDataRecord>()
						 .Select(dr => dr.GetString(0));

					var allTables = String.Join("\r\n  • ", tables);
					if (String.IsNullOrEmpty(allTables))
						allTables = "Database has no tables";
					else
						allTables = "Tables:\r\n\r\n  • " + allTables;

					MessageBox.Show("Connection succeeded.\r\n" + allTables);
				}
			} catch (Exception ex) {
				MessageBox.Show("Connection failed.\r\n\r\n" + ex.Message);
			}
		}
		private void CreateDb_Click(object sender, RoutedEventArgs e) {
			try {
				using (var connection = Config.OpenConnection())
					connection.ExecuteNonQuery("SELECT 1;");
			} catch {
				CreateDatabase();
				return;
			}
			MessageBox.Show("This database already exists");
		}

		//I haven't tested this with OleDB or ODBC.
		void CreateDatabase() {
			var builder = new SqlConnectionStringBuilder(Config.ConnectionString);
			var db = builder.InitialCatalog;
			builder.InitialCatalog = "";

			var ourConfig = new Schema.DatabaseConfig { Type = Config.Type, ConnectionString = builder.ToString() };

			try {
				using (var connection = ourConfig.OpenConnection())
					connection.ExecuteNonQuery("CREATE DATABASE " + db);
				MessageBox.Show("Created database " + db);
			} catch (Exception ex) {
				MessageBox.Show("An error occurred.\r\n\r\n" + ex.Message);
			}
		}
	}
}
