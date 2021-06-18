using System;
using System.Data;
using MySql.Data.MySqlClient;
using DbExtensions;

namespace Skladik.Adapters.BandAdapters {
	public class BandAdapter {
		
		public MySqlConnection Conn { get; set; }
		public string SelectQueryFormat { get; set; }
		public string SelectQueryColumns { get; set; }
		public int RecordsPerPage { get; set; }

		public BandAdapter(string selectQueryFormat, string selectQueryColumns, int recordsPerPage, MySqlConnection conn) {
			SelectQueryFormat = selectQueryFormat;
			SelectQueryColumns = selectQueryColumns;
			RecordsPerPage = recordsPerPage;
			Conn = conn;
		}


	}
}
