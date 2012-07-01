using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Configurator.Controls {
	/// <summary>
	/// Interaction logic for DatabaseSettings.xaml
	/// </summary>
	partial class DatabaseSettings : UserControl {
		readonly List<SqlScript> scripts;

		readonly Regex sqlScriptMatcher = new Regex(@"^\d+-");
		public DatabaseSettings() {
			InitializeComponent();
			if (App.HasEnvironmentLocations) {
				scripts = Directory.EnumerateFiles(Path.Combine(App.SourceRoot, @"Setup\Sql Scripts"), "*.sql")
								   .Where(p => sqlScriptMatcher.IsMatch(Path.GetFileName(p)))		//Skip Schema Changes.sql
								   .Select(p => new SqlScript(p))
								   .ToList();
				scriptsList.ItemsSource = scripts;
			}
		}

		private Schema.DatabaseConfig Config { get { return (Schema.DatabaseConfig)DataContext; } }

		#region Create & Test
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
		#endregion

		class SqlScript {
			public SqlScript(string path) {
				FilePath = path;
				Name = Path.GetFileNameWithoutExtension(FilePath);

				var dash = Name.IndexOf('-');
				Index = int.Parse(Name.Remove(dash));
				Name = Name.Substring(dash + 1);

				Description = String.Join(Environment.NewLine,
					File.ReadLines(FilePath)
						.TakeWhile(s => s.StartsWith("--"))
						.Select(s => s.Substring(2))
				);

				IsSelected = true;		//By default, all scripts are checked
			}

			public string Name { get; private set; }
			public string FilePath { get; private set; }
			public int Index { get; private set; }
			public string Description { get; private set; }

			public bool IsSelected { get; set; }

			public void Execute(DbTransaction transaction) {
				transaction.ExecuteNonQuery(File.ReadAllText(FilePath));
			}
		}

		private void RunScripts_Click(object sender, RoutedEventArgs e) {
			using (var connection = Config.OpenConnection())
			using (var transaction = connection.BeginTransaction()) {
				string name = null;
				try {
					foreach (var script in scripts) {
						name = script.Name;
						if (script.IsSelected)
							script.Execute(transaction);
					}
				} catch (Exception ex) {
					MessageBox.Show("An error occurred while executing " + name + ".\r\n\r\n" + ex.Message);
					transaction.Rollback();
					return;
				}

				transaction.Commit();
			}
		}
	}
}