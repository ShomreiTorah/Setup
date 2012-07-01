using System;
using System.Collections.Generic;
using System.Data;
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
			using (var connection = Config.OpenConnection()) {
				try {
					var tables = connection.ExecuteReader("SELECT s.name + '.' + o.name FROM sys.objects o JOIN sys.schemas s ON o.schema_id = s.schema_id WHERE type = 'U'")
						 .Cast<IDataRecord>()
						 .Select(dr => dr.GetString(0));

					MessageBox.Show("Connection succeeded.\r\nTables:\r\n\r\n  • " + String.Join("\r\n  • ", tables));
				} catch (Exception ex) {
					MessageBox.Show("Connection failed.\r\n\r\n" + ex.Message);
				}
			}
		}
		private void CreateDb_Click(object sender, RoutedEventArgs e) {

		}

	}
}
