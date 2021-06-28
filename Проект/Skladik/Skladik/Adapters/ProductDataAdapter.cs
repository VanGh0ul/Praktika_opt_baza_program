using System;
using System.Drawing;
using MySql.Data.MySqlClient;

using Skladik.Utils;
using Skladik.Adapters;


using System.Windows.Forms;

namespace Skladik.Adapters
{
	// Класс для работы с товаром (БД)
	public class ProductDataAdapter
	{

		public MySqlConnection Conn { get; private set; }
		public int Id { get; private set; }
		public OrganizationDataAdapter Seller { get; set; }
		public string Name { get; private set; }
		public int Quantity { get; private set; }
		public string About { get; private set; }
		public Image Img { get; private set; }
		public bool ImageChanged { get; set; }      // Для определения изменилась ли картинка
		public DateTime AddedOn { get; private set; }
		public DateTime ModifiedOn { get; private set; }
		public int Price { get; private set; }
		public string MeasureUnit { get; private set; }
		public int MeasureUnitId { get; private set; }
		public int CategoryId { get; private set; }

		public ProductDataAdapter(MySqlConnection conn)
		{
			Conn = conn;
			Seller = new OrganizationDataAdapter(Conn);
		}

		// Получение данных о товаре
		public bool GetData(int id)
		{

			// Выгрузка из БД

			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText =
				"select p.name as p_name, p.about, b.quantity, p.img, p.added_on, p.modified_on, p.price, p.measure_unit_id, m.name as mu_name, p.category_id " +
				"from product p, balance b, measure_unit m " +
				"where " +
					"p.id = @id and " +
					"p.id = b.product_id and " +
					"p.measure_unit_id = m.id ";

			Query.Parameters.Add("id", MySqlDbType.VarChar).Value = id;

			// Выгрузка данных
			bool Result = true;

			Conn.Open();

			MySqlDataReader QueryReader = Query.ExecuteReader();

			// Расспределение данных по свойствам
			// Проверка на существование товара
			if (QueryReader.HasRows)
			{

				QueryReader.Read();

				Id = id;
				Name = QueryReader.GetString("p_name");
				Quantity = QueryReader.GetInt32("quantity");

				if (QueryReader.IsDBNull(2))
					About = null;
				else
					About = QueryReader.GetString("about");

				Img = QueryUtils.GetImageFromByteArray((byte[])QueryReader["img"]);

				AddedOn = QueryReader.GetDateTime("added_on");

				// Проверка на отсутствие даты модификации
				if (QueryReader.IsDBNull(6))
					ModifiedOn = AddedOn;

				else
					ModifiedOn = QueryReader.GetDateTime("modified_on");

				Price = QueryReader.GetInt32("price");
				MeasureUnit = QueryReader.GetString("mu_name");
				MeasureUnitId = QueryReader.GetInt32("measure_unit_id");
				CategoryId = QueryReader.GetInt32("category_id");

				Conn.Close();
				// Получение данных поставщика


			}
			else
				Result = false;

			QueryReader.Close();

			Conn.Close();

			return Result;

		}

		// Добавляет товар, возвращает его ид
		public int Insert(int userId, string name, string about, int price, int measureUnitId, int count, int categoryId, Image img)
		{

			// Запрос добавления товара
			int NewProductId;

			using (MySqlCommand InsertProduct = Conn.CreateCommand())
			{

				InsertProduct.CommandText =
					"insert into product " +
					"(name, about, img, added_on, modified_on, price, measure_unit_id, category_id) " +
					"values ( @name, @about, @img, @added_on, @modified_on, @price, @measure_unit_id, @category_id)";

				InsertProduct.Parameters.Add("name", MySqlDbType.VarChar).Value = name;
				InsertProduct.Parameters.Add("about", MySqlDbType.Text).Value = about;
				InsertProduct.Parameters.Add("img", MySqlDbType.Blob).Value = QueryUtils.GetImageByteArray(img);
				InsertProduct.Parameters.Add("added_on", MySqlDbType.Date).Value = DateTime.Now;
				InsertProduct.Parameters.Add("modified_on", MySqlDbType.Date).Value = DateTime.Now;
				InsertProduct.Parameters.Add("price", MySqlDbType.Int32).Value = price;
				InsertProduct.Parameters.Add("measure_unit_id", MySqlDbType.Int32).Value = measureUnitId;
				InsertProduct.Parameters.Add("category_id", MySqlDbType.Int32).Value = categoryId;

				Conn.Open();

				InsertProduct.ExecuteNonQuery();

				// Получение ид добавленного товара
				NewProductId = QueryUtils.GetLastInsertedId(Conn);

				Conn.Close();

			}

			// Запрос создания баланса 
			Balance.Insert(NewProductId, count, Conn);

			// Операция пополнения
			Operation.Create(userId, NewProductId, count, Conn);

			GetData(NewProductId);

			return NewProductId;
		}

		// Обновляет товар
		public bool Update(int userId, string name, string about, int price, int measureUnitId, int count, int categoryId, Image img)
		{

			bool Result = false;

			// Составление запроса
			MySqlCommand Query = Conn.CreateCommand();

			string CommandInner = "";

			if (name != Name)
			{
				CommandInner += "name = @name ";
				Query.Parameters.Add("name", MySqlDbType.VarChar).Value = name;
			}

			if (about != About)
			{

				if (CommandInner.Length > 0)
					CommandInner += " , ";

				CommandInner += " about = @about ";
				Query.Parameters.Add("about", MySqlDbType.VarChar).Value = about;
			}

			if (price != Price)
			{

				if (CommandInner.Length > 0)
					CommandInner += " , ";

				CommandInner += " price = @price ";
				Query.Parameters.Add("price", MySqlDbType.Int32).Value = price;
			}

			if (measureUnitId != MeasureUnitId)
			{

				if (CommandInner.Length > 0)
					CommandInner += " , ";

				CommandInner += " measure_unit_id = @measureUnitId ";
				Query.Parameters.Add("measureUnitId", MySqlDbType.Int32).Value = measureUnitId;
			}

			if (categoryId != CategoryId)
			{

				if (CommandInner.Length > 0)
					CommandInner += " , ";

				CommandInner += " category_id = @categoryId ";
				Query.Parameters.Add("categoryId", MySqlDbType.Int32).Value = categoryId;
			}

			if (this.ImageChanged)
			{

				if (CommandInner.Length > 0)
					CommandInner += " , ";

				CommandInner += " img = @img ";
				Query.Parameters.Add("img", MySqlDbType.Blob).Value = QueryUtils.GetImageByteArray(img);
			}

			// Если меняется количество -
			// изменение баланса,
			// составление операции
			if (count != Quantity)
			{
				// Создание операции
				Operation.Create(userId, Id, count - Quantity, Conn);

				// Обновление баланса
				Balance.Update(Id, count, Conn);

				Result = true;

			}

			if (CommandInner.Length > 0)
			{

				Query.CommandText = "update product set " + CommandInner + " , modified_on = @modifiedOn where id = @id";
				Query.Parameters.Add("id", MySqlDbType.Int32).Value = Id;
				Query.Parameters.Add("modifiedOn", MySqlDbType.Date).Value = DateTime.Now;

				Conn.Open();

				Query.ExecuteNonQuery();

				Conn.Close();

				this.ImageChanged = false;

				Result = true;
			}

			return Result;

		}

		// "Удаление" товара
		public void Delete()
		{

			// Изменеие статуса флага is_deleted
			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText = "update product set name = 'Товар удален' where id = @id";

			Query.Parameters.Add("id", MySqlDbType.Int32).Value = Id;

			Conn.Open();

			Query.ExecuteNonQuery();

			Conn.Close();

		}


		// Добавление в заказ
		public void AddToOrder(int orgId, int quantity)
		{

			MySqlCommand Query = Conn.CreateCommand();

			// Поиск заказа со статусом "в процессе оформления"
			// направленного на данного поставщика
			Query.CommandText =
				"select o.id from buy_order o, order_status os " +
				"where " +
					"o.buyer_id = @b_id and " +
					"o.status_id = os.id and " +
					"os.name = 'editing'";

			Query.Parameters.Add("b_id", MySqlDbType.Int32).Value = orgId;
			Query.Parameters.Add("s_id", MySqlDbType.Int32).Value = Seller.Id;

			Conn.Open();

			MySqlDataReader QueryReader = Query.ExecuteReader();

			// Если заказ есть
			if (QueryReader.Read())
			{

				int OrderId = QueryReader.GetInt32(0);

				QueryReader.Close();
				Conn.Close();
				// Поиск этого товара в данном заказе
				Query = Conn.CreateCommand();
				Query.CommandText = "select id, quantity from order_content where order_id = @o_id and product_id = @p_id";

				Query.Parameters.Add("o_id", MySqlDbType.Int32).Value = OrderId;
				Query.Parameters.Add("p_id", MySqlDbType.Int32).Value = Id;


				Conn.Open();
				QueryReader = Query.ExecuteReader();

				// Если товар уже есть  
				if (QueryReader.Read())
				{

					int PositionId = QueryReader.GetInt32("id");

					// Если старое количество + новое количество
					// не больше максимального имеющегося в наличии
					int NewQuantity = quantity + QueryReader.GetInt32("quantity");


					QueryReader.Close();
					Conn.Close();

					if (NewQuantity < this.Quantity)

						// Если товара больше чем допускатеся товара
						// одного типа в одном заказе (10000)
						if (NewQuantity > QueryUtils.MaxQuantityInOrder)

							// Установка максимального количества
							quantity = QueryUtils.MaxQuantityInOrder;

						// Если товара меньше макс допустимого
						else
							// Установка суммы значений товаров 
							quantity = NewQuantity;

					// Если нужного товара больше или равно тому что есть
					else
						// Установка всего количества товара в
						// качестве покупаемого
						quantity = this.Quantity;

					// Изменение товара в заказе
					Query = Conn.CreateCommand();
					Query.CommandText = "update order_content set quantity = @quant where id = @pos_id";

					Query.Parameters.Add("quant", MySqlDbType.Int32).Value = quantity;
					Query.Parameters.Add("pos_id", MySqlDbType.Int32).Value = PositionId;

					Conn.Open();

					Query.ExecuteNonQuery();

					Conn.Close();

				}
				else
				{                           // Если товара нет

					QueryReader.Close();
					Conn.Close();
					// Добавление товара к заказу	
					Query = Conn.CreateCommand();
					Query.CommandText =
						"insert into order_content " +
						"(order_id, product_id, quantity) " +
						"values (@o_id, @p_id, @quant)";

					Query.Parameters.Add("o_id", MySqlDbType.Int32).Value = OrderId;
					Query.Parameters.Add("p_id", MySqlDbType.Int32).Value = this.Id;
					Query.Parameters.Add("quant", MySqlDbType.Int32).Value = quantity;

					Conn.Open();

					Query.ExecuteNonQuery();

					Conn.Close();
				}

			}
			else
			{                               // Если заказа нет

				QueryReader.Close();
				Conn.Close();
				// Создание заказа

				Query = Conn.CreateCommand();
				Query.CommandText =
					"insert into buy_order " +
					"(buyer_id, status_id) " +
					"values (@b_id, (select id from order_status where name = 'editing'))";

				Query.Parameters.Add("b_id", MySqlDbType.Int32).Value = orgId;
				Query.Parameters.Add("s_id", MySqlDbType.Int32).Value = this.Seller.Id;

				Conn.Open();

				Query.ExecuteNonQuery();

				//Conn.Close();

				// Получение ид заказа
				int OrderId = QueryUtils.GetLastInsertedId(Conn);
				Conn.Close();
				// Добавление товара к заказу
				Query = Conn.CreateCommand();
				Query.CommandText =
					"insert into order_content " +
					"(order_id, product_id, quantity) " +
					"values (@o_id, @p_id, @quant)";

				Query.Parameters.Add("o_id", MySqlDbType.Int32).Value = OrderId;
				Query.Parameters.Add("p_id", MySqlDbType.Int32).Value = this.Id;
				Query.Parameters.Add("quant", MySqlDbType.Int32).Value = quantity;


				Conn.Open();

				Query.ExecuteNonQuery();

				Conn.Close();


			}

		}

	}
}
