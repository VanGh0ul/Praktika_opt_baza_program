using System;
using System.Data;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using Skladik.Adapters.BandAdapters;
using Skladik.Utils;

namespace Skladik.Adapters
{
	// Класс для работы с данными заказов
	public class OrderDataAdapter
	{

		public MySqlConnection Conn { get; private set; }
		public int Id { get; private set; }
		public int BuyerId { get; private set; }
		public int SellerId { get; private set; }
		public DateTime? SentOn { get; private set; }
		public DateTime? DeliverOn { get; private set; }
		public int? AddressId { get; private set; }
		public string Address { get; private set; }
		public string Status { get; private set; }
		public string RusStatus { get; private set; }
		public int OrderSum { get; private set; }

		public string Seller { get; private set; }
		public string Buyer { get; private set; }


		public OrderDataAdapter(MySqlConnection conn)
		{
			Conn = conn;
		}

		// Получение даных
		public bool GetData(int id)
		{

			// Составление запроса
			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText =
				"select bo.id, bo.buyer_id, bo.sent_on, bo.deliver_date, st.name as status_name, bo.order_sum, " +
				"(select o.name from organization o where o.id = bo.buyer_id) as buyer_name, " +
				"st.rus_name as rus_status_name " +
				"from " +
					"buy_order bo inner join order_status st on bo.status_id = st.id " +
				"where " +
					"bo.id = @o_id";

			Query.Parameters.Add("o_id", MySqlDbType.Int32).Value = id;

			Conn.Open();

			bool Result = true;

			MySqlDataReader QueryReader = Query.ExecuteReader();

			try
			{

				// Выгрузка данных
				// Проверка на существование
				if (QueryReader.Read())
				{

					Id = id;
					BuyerId = QueryReader.GetInt32("buyer_id");


					Buyer = QueryReader.GetString("buyer_name");

					// Проверка дат на пустое значение
					if (QueryReader.IsDBNull(3))
						SentOn = null;

					if (QueryReader.IsDBNull(4))
						DeliverOn = null;



					// Проверка адреса на отсутствие
					//if (QueryReader.IsDBNull(5)) {
					//	AddressId = null;
					//	Address = null;
					//} else {
					//	AddressId = QueryReader.GetInt32("address_id");
					//	Address = QueryReader.GetString("adr_name");
					//}

					Status = QueryReader.GetString("status_name");
					RusStatus = QueryReader.GetString("rus_status_name");


				}
				else
					Result = false;

			}
			finally
			{

				QueryReader.Close();

				Conn.Close();

			}

			return Result;

		}

		// Получение адаптера товаров заказа
		public BandAdapter GetProducts()
		{

			MySqlCommand SelectQuery = Conn.CreateCommand();
			MySqlCommand SelectCount = Conn.CreateCommand();

			if (this.Status == "editing")
			{

				SelectQuery.CommandText =
					"select oc.product_id, oc.quantity, p.name, p.price, mu.name as mu_name, (oc.quantity * p.price) as pos_sum " +
					"from order_content oc, product p, measure_unit mu " +
					"where " +
						"oc.order_id = @ord_id and " +
						"oc.product_id = p.id and " +
						"p.measure_unit_id = mu.id";

				SelectQuery.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			}
			else
			{

				SelectQuery.CommandText =
					"select product_id, quantity, product_name, product_price, product_measure_unit, pos_sum " +
					"from order_content " +
					"where order_id = @ord_id";

				SelectQuery.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			}

			SelectCount.CommandText =
					"select count(*) " +
					"from order_content " +
					"where order_id = @ord_id";

			SelectCount.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;


			return new BandAdapter(SelectQuery, SelectCount, Styles.OrderContentListElemCount, Conn);

		}

		// Изменение заказа
		public bool Update(int addressId, DateTime deliverDate)
		{

			// Составление запроса
			MySqlCommand Query = Conn.CreateCommand();

			string CommandInner = "";

			//if (addressId != this.AddressId) {

			//	CommandInner += " address_id = @adr_id ";
			//	Query.Parameters.Add("adr_id", MySqlDbType.Int32).Value = addressId;

			//}

			if (deliverDate != this.DeliverOn)
			{

				if (CommandInner.Length > 0)
					CommandInner += " , ";

				CommandInner += " deliver_date = @deliver_date ";
				Query.Parameters.Add("deliver_date", MySqlDbType.Date).Value = deliverDate;

			}

			// Отправка запроса
			if (CommandInner.Length > 0)
			{

				Query.CommandText = "update buy_order set " + CommandInner + " where id = @id";

				Query.Parameters.Add("id", MySqlDbType.Int32).Value = Id;

				Conn.Open();

				Query.ExecuteNonQuery();

				Conn.Close();

				return true;

			}

			return false;

		}

		// Удаление заказа
		public void Delete()
		{

			// Удаление содержимого
			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText = "delete from order_content where order_id = @ord_id";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			Conn.Open();

			Query.ExecuteNonQuery();

			Conn.Close();

			// Удаление самого заказа
			Query.CommandText = "delete from buy_order where id = @ord_id";

			// Удаление
			Conn.Open();

			Query.ExecuteNonQuery();

			Conn.Close();

		}

		// Отправка заказа на рассмотрение
		public void Send()
		{

			// Фиксирование информации о позициях

			// Получение нужной для дальнейшего обновления информации
			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText =
				"select oc.id, p.name, p.price, mu.name as mu_name, (oc.quantity * p.price) as pos_sum " +
				"from order_content oc, product p, measure_unit mu " +
				"where " +
					"oc.order_id = @ord_id and " +
					"oc.product_id = p.id and " +
					"p.measure_unit_id = mu.id";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			// Формирование запроса на изменение статуса заказа
			MySqlCommand UpdateOrderStatus = Conn.CreateCommand();
			UpdateOrderStatus.CommandText =
				"update buy_order set " +
				"status_id = (select id from order_status where name = 'waiting'), " +
				"order_sum = @ord_sum, " +
				"sent_on = @sent_on " +
				"where id = @ord_id";

			UpdateOrderStatus.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;
			UpdateOrderStatus.Parameters.Add("ord_sum", MySqlDbType.Int32);
			UpdateOrderStatus.Parameters.Add("sent_on", MySqlDbType.Date).Value = DateTime.Now;



			using (MySqlConnection UpdateConnection = QueryUtils.GetConnection())
			{

				// Формирование запроса на обновление
				MySqlCommand UpdateQuery = UpdateConnection.CreateCommand();
				UpdateQuery.CommandText =
					"update order_content set " +
					"product_name = @name, " +
					"product_price = @price, " +
					"product_measure_unit = @measureUnit, " +
					"pos_sum = @sum " +
					"where id = @pos_id";

				UpdateQuery.Parameters.Add("name", MySqlDbType.VarChar);
				UpdateQuery.Parameters.Add("price", MySqlDbType.Int32);
				UpdateQuery.Parameters.Add("measureUnit", MySqlDbType.VarChar);
				UpdateQuery.Parameters.Add("sum", MySqlDbType.Int32);

				UpdateQuery.Parameters.Add("pos_id", MySqlDbType.Int32);

				int Sum = 0;

				Conn.Open();

				UpdateConnection.Open();

				MySqlDataReader QueryReader = Query.ExecuteReader();

				while (QueryReader.Read())
				{
					// Передача данных				
					UpdateQuery.Parameters["name"].Value = QueryReader.GetString("name");
					UpdateQuery.Parameters["price"].Value = QueryReader.GetInt32("price");
					UpdateQuery.Parameters["measureUnit"].Value = QueryReader.GetString("mu_name");

					int PosSum = QueryReader.GetInt32("pos_sum");
					UpdateQuery.Parameters["sum"].Value = PosSum;
					Sum += PosSum;

					UpdateQuery.Parameters["pos_id"].Value = QueryReader.GetInt32("id");

					// Обновление данных
					UpdateQuery.ExecuteNonQuery();

				}

				QueryReader.Close();

				UpdateConnection.Close();

				// Изменение заказа
				UpdateOrderStatus.Parameters["ord_sum"].Value = Sum;

				UpdateOrderStatus.ExecuteNonQuery();

				Conn.Close();

			}


		}

		// Принятие заказа
		public void Accept(int userId)
		{

			// Создание операций сбыта и изменение баланса
			// Получение содержания заказа
			MySqlDataAdapter ProductsAdapter = new MySqlDataAdapter(
				"select oc.product_id, oc.quantity, b.quantity as in_stock from order_content oc, balance b where oc.order_id = @ord_id and oc.product_id = b.product_id",
				Conn
			);

			ProductsAdapter.SelectCommand.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			DataTable Products = new DataTable();

			ProductsAdapter.Fill(Products);
			Conn.Close();

			foreach (DataRow Row in Products.Rows)
			{
				int ProductId = Convert.ToInt32(Row["product_id"]);
				int Quantity = Convert.ToInt32(Row["quantity"]);
				int InStock = Convert.ToInt32(Row["in_stock"]);

				Operation.Create(userId, ProductId, -1 * Quantity, Conn);
				Balance.Update(ProductId, InStock - Quantity, Conn);
			}

			// Изменение статуса заказа
			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText =
				"update buy_order set " +
				"status_id = (select id from order_status where name = 'accepted') " +
				"where id = @ord_id";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			// Отправка
			Conn.Open();

			Query.ExecuteNonQuery();

			Conn.Close();

		}

		// Отмена заказа
		public void Cancel()
		{

			// Составление запроса
			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText =
				"update buy_order set " +
				"status_id = (select id from order_status where name = 'canceled') " +
				"where id = @ord_id";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			// Отправка
			Conn.Open();

			Query.ExecuteNonQuery();

			Conn.Close();

		}

		// Отклонение заказа
		public void Decline()
		{

			// Составление запроса
			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText =
				"update buy_order set " +
				"status_id = (select id from order_status where name = 'declined') " +
				"where id = @ord_id";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			// Отправка
			Conn.Open();

			Query.ExecuteNonQuery();

			Conn.Close();

		}

		// Пометка заказа как доставленный
		public void MarkAsDelivered()
		{

			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText =
				"update buy_order set " +
				"status_id = (select id from order_status where name = 'delivered') " +
				"where id = @ord_id";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			// Отправка
			Conn.Open();

			Query.ExecuteNonQuery();

			Conn.Close();

		}


		public void CreateWordOrderReport() { }

		public void CreateWordOrderContent() { }

		// Проверка - изменилась ли роль
		public bool StatusIsSame()
		{

			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText = "select st.name from buy_order bo, order_status st where bo.id = @id and bo.status_id = st.id";

			Query.Parameters.Add("id", MySqlDbType.Int32).Value = this.Id;

			Conn.Open();

			string NewStatus = Query.ExecuteScalar().ToString();

			Conn.Close();

			if (this.Status != NewStatus)
				return false;

			return true;

		}

		// Обновляет сумму заказа на основе содержимого
		public void UpdateOrderSum()
		{

			MySqlCommand Query = Conn.CreateCommand();

			// Вычисление суммы заказа
			Query = Conn.CreateCommand();
			Query.CommandText =
				"select sum(oc.quantity * p.price) " +
				"from order_content oc, product p " +
				"where " +
					"oc.order_id = @o_id and " +
					"oc.product_id = p.id ";

			Query.Parameters.Add("o_id", MySqlDbType.Int32).Value = Id;

			Conn.Open();

			Object Res = Query.ExecuteScalar();

			if (Res != DBNull.Value)
				OrderSum = Convert.ToInt32(Res);

			else
				OrderSum = 0;

			Conn.Close();

		}

		// Проверка на пустой заказ
		public bool IsEmpty()
		{

			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText = "select count(*) from order_content where order_id = @ord_id";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			Conn.Open();

			int RowCount = Convert.ToInt32(Query.ExecuteScalar());

			Conn.Close();

			if (RowCount == 0)
				return true;

			return false;

		}

		// Проверка - содержит удаленные товары
		// Возвращает список названий товаров, что были удалены
		// пока заказчик думал
		//public List<string> DeletedProducts() {

		//	List<string> Result = new List<string>();

		//	MySqlCommand Query = Conn.CreateCommand();

		//	Query.CommandText = 
		//		"select p.name from order_content oc, product p " +
		//		"where " +
		//			"p.name = 'Товар удален' " +
		//			"oc.order_id = @ord_id and " +
		//			"oc.product_id = p.id ";

		//	Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

		//	Conn.Open();

		//	MySqlDataReader QueryReader = Query.ExecuteReader();

		//	while (QueryReader.Read()) 
		//		Result.Add(QueryReader.GetString(0));

		//	Conn.Close();

		//	if (Result.Count > 0)
		//		return Result;

		//	return null;

		//}

		// Проверка - всё есть в наличие
		// Возвращает список товаров, что нет в наличии
		public List<string> OutOfStockProducts()
		{

			List<string> Result = new List<string>();

			MySqlCommand Query = Conn.CreateCommand();

			Query.CommandText =
				"select p.name, b.quantity, mu.name as mu_name " +
				"from order_content oc, balance b, product p, measure_unit mu " +
				"where " +
					"oc.order_id = @ord_id and " +
					"oc.product_id = p.id and " +
					"p.measure_unit_id = mu.id and " +
					"p.id = b.product_id and " +
					"b.quantity < oc.quantity";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			Conn.Open();

			MySqlDataReader QueryReader = Query.ExecuteReader();

			while (QueryReader.Read())
				Result.Add(String.Format("{0}({1} {2})", QueryReader.GetString("name"), QueryReader.GetString("quantity"), QueryReader.GetString("mu_name")));



			/*
			Result.Add(
				QueryReader.GetString("name") + "(" + QueryReader.GetString("quantity") + " " + QueryReader.GetString("mu_name") + ")"
			);
			*/
			Conn.Close();

			if (Result.Count > 0)
				return Result;

			return null;

		}

		/*
													// Обновление статуса
		public void UpdateStatus() {
			
			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText = 
				"select st.name as status_name, st.rus_name as status_rus_name " + 
				"from buy_order bo, order_status st " + 
				"where bo.id = @ord_id";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = Id;

			Conn.Open();

			MySqlDataReader QueryReader = Query.ExecuteReader();

			QueryReader.Read();

			this.Status = QueryReader.GetString("status_name");
			this.RusStatus = QueryReader.GetString("status_rus_name");

			QueryReader.Close();

			Conn.Close();

		}
		*/
	}

}
