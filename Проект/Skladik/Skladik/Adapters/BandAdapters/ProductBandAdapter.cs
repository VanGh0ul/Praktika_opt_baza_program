using System.Data;
using MySql.Data.MySqlClient;

using Skladik.Adapters.BandAdapters;


using System.Windows.Forms;

namespace Skladik.Adapters.BandAdapters {

	public enum ProductsSortType { Default, Asc, Desc }

													/// Класс для ленты товаров
	public class ProductBandAdapter : FilterableBandAdapter {

		public int? CategoryFilterId { get; set; }
		public ProductsSortType SortByPrice { get; set; }
		public ProductsSortType SortByAddDate { get; set; }

		public ProductBandAdapter(MySqlCommand selectQuery, MySqlCommand selectCount, int recordsPerPage, MySqlConnection conn) 
			: base(selectQuery, selectCount, recordsPerPage, conn) {
			
			CategoryFilterId = null;
			SortByPrice = ProductsSortType.Default;
			SortByAddDate = ProductsSortType.Default;

		}

													// Составление запроса
		protected override MySqlCommand GenerateQuery(MySqlCommand query) {
			
													// Установка фильтра
			MySqlCommand Query = base.GenerateQuery(query);

													// Добавление категории
			if (CategoryFilterId != null) {
													// Добавление ключевого слова
				if (Query.CommandText.Contains("where"))
					Query.CommandText += " and ";
				else 
					Query.CommandText += " where ";

				Query.CommandText += " category_id = @c_id ";
				Query.Parameters.Add("c_id", MySqlDbType.Int32).Value = (int)CategoryFilterId;
			}

			string OrderByText = "";									
													// Добавление сортировки
			if (SortByPrice == ProductsSortType.Asc)
				OrderByText += "price asc";

			else if (SortByPrice == ProductsSortType.Desc)
				OrderByText += "price desc";

			if (SortByAddDate != ProductsSortType.Default) {
			
				if (SortByPrice != ProductsSortType.Default)
					OrderByText += ", ";

				if (SortByAddDate == ProductsSortType.Asc)
					OrderByText += "added_on asc";

				else if (SortByAddDate == ProductsSortType.Desc)
					OrderByText += "added_on desc";

			}

			if (OrderByText.Length > 0)
				Query.CommandText += " order by " + OrderByText;

			return Query;

		}

													// Очистка категории
		public void ResetCategory() {
			
			CategoryFilterId = null;

		}
											
													// Очистка сортировки
		public void ResetSort() {
			
			SortByPrice = ProductsSortType.Default;
			SortByAddDate = ProductsSortType.Default;

		}


	}

}
