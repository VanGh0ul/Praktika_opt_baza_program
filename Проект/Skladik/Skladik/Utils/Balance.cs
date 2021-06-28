using MySql.Data.MySqlClient;

namespace Skladik.Utils
{
	// Статический класс управления балансом
	public static class Balance
	{

		// Создание баланса
		public static void Insert(int productId, int quantity, MySqlConnection conn)
		{

			MySqlCommand Query = conn.CreateCommand();

			Query.CommandText =
				"insert into balance " +
				"(product_id, quantity) " +
				"values (@productId, @quantity)";

			Query.Parameters.Add("productId", MySqlDbType.Int32).Value = productId;
			Query.Parameters.Add("quantity", MySqlDbType.Int32).Value = quantity;


			conn.Open();

			Query.ExecuteNonQuery();

			conn.Close();

		}

		// Обновление баланса
		public static void Update(int productId, int quantity, MySqlConnection conn)
		{

			MySqlCommand Query = conn.CreateCommand();

			Query = conn.CreateCommand();
			Query.CommandText = "update balance set quantity = @quant where product_id = @id";

			Query.Parameters.Add("quant", MySqlDbType.Int32).Value = quantity;
			Query.Parameters.Add("id", MySqlDbType.Int32).Value = productId;

			conn.Open();

			Query.ExecuteNonQuery();

			conn.Close();

		}

	}
}
