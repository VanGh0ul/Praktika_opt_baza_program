using MySql.Data.MySqlClient;

namespace Skladik.Utils {
													// Класс управления операциями 
													// пополнения и сбыта товара из баланса
	public static class Operation {

													// Создание новой операции
		public static void Create(int userId, int productId, int quantity, MySqlConnection conn) {
		
			MySqlCommand Query = conn.CreateCommand();
			Query.CommandText = 
				"insert into operation " + 
				"(user_id, product_id, quantity) " + 
				"values(@userId, @productId, @quantity)";

			Query.Parameters.Add("userId", MySqlDbType.Int32).Value = userId;
			Query.Parameters.Add("productId", MySqlDbType.Int32).Value = productId;
			Query.Parameters.Add("quantity", MySqlDbType.Int32).Value = quantity;

			conn.Open();

			Query.ExecuteNonQuery();

			conn.Close();

		}

	}
}
