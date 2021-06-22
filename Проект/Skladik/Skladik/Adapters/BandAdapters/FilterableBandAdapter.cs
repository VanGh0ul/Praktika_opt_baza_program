using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace Skladik.Adapters.BandAdapters {

	public delegate MySqlCommand DSearchStrategy(MySqlCommand query, string FilterString);


													// Адаптер списков, что поддерживает фильтрацию
	public class FilterableBandAdapter : BandAdapter {
		
		public string FilterString { get; set; }
		public DSearchStrategy SearchStrategy { get; set; }

		public FilterableBandAdapter(MySqlCommand selectQuery, MySqlCommand selectCount, int recordsPerPage, MySqlConnection conn)
			: base(selectQuery, selectCount, recordsPerPage, conn) { 
		
			FilterString = null;

		}

													// Сброс поиска
		public void ResetFilter() {
			FilterString = null;
		}

													// Составление запроса
		protected override MySqlCommand GenerateQuery(MySqlCommand query) {
			
			MySqlCommand Query;

			if (FilterString != null && SearchStrategy != null)
				Query = SearchStrategy((MySqlCommand)query.Clone(), FilterString);

			else
				Query = (MySqlCommand)query.Clone();

			return Query;

		}
		
	}
}
