using System;
using System.Data;
using MySql.Data.MySqlClient;
using DbExtensions;

using System.Windows.Forms;

namespace Skladik.Adapters.BandAdapters {

													// Класс для получения данных для
													// списков, поддерживающий страницы
	public class BandAdapter {
		
		public MySqlConnection Conn { get; set; }
		public MySqlCommand SelectQuery { get; set; }
		public MySqlCommand SelectCount { get; set; }
		public int RecordsPerPage { get; set; }

		public BandAdapter(MySqlCommand selectQuery, MySqlCommand selectCount, int recordsPerPage, MySqlConnection conn) {
			SelectQuery = selectQuery;
			SelectCount = selectCount;
			RecordsPerPage = recordsPerPage;
			Conn = conn;

			SelectQuery.Connection = Conn;
			SelectCount.Connection = Conn;

		}

													// Составление запроса
		protected virtual MySqlCommand GenerateQuery(MySqlCommand query) {

			return (MySqlCommand)query.Clone();

		}
								
													// Получение данных по странице
		public virtual DataTable GetData(int page) {

													// Добавление limit offset
			MySqlCommand Query = GenerateQuery(SelectQuery);

			Query.CommandText += " limit @fpos, @secpos";
			Query.Parameters.Add("fpos", MySqlDbType.Int32).Value = (page - 1) * RecordsPerPage;
			Query.Parameters.Add("secpos", MySqlDbType.Int32).Value = RecordsPerPage;

													// Возврат данных
			MySqlDataAdapter Adapter = new MySqlDataAdapter(Query);

			DataSet Temp = new DataSet();

			Adapter.Fill(Temp);
									
			return Temp.Tables[0];
													

		}

													// Получение количества страниц
		public virtual int GetPageCount() {
			
			MySqlCommand Query = GenerateQuery(SelectCount);
			
			int Result = 0;

			Conn.Open();
			
			double RowCount = Convert.ToDouble(Query.ExecuteScalar());

			Conn.Close();

			Result = Convert.ToInt32(Math.Ceiling(RowCount / RecordsPerPage));

			return Result;

		}


	}
}
