using System;
using System.IO;
using System.Drawing;
using MySql.Data.MySqlClient;

using Skladik.Utils;

namespace Skladik.Adapters
{
	public class OrganizationDataAdapter
	{

		public MySqlConnection Conn { get; private set; }
		public int Id { get; private set; }
		public string Name { get; private set; }
		public string About { get; private set; }
		public string Email { get; private set; }
		public string Phone { get; private set; }
		public Image Img { get; private set; }
		public DateTime RegDate { get; private set; }

		public OrganizationDataAdapter(MySqlConnection conn)
		{
			Conn = conn;
		}

		// Создание организации
		public void Register(string name, string email, string phone, Image img)
		{

			// Добавление организации
			MySqlCommand InsertCommand = Conn.CreateCommand();
			InsertCommand.CommandText =
				"insert into organization(name, email, phone, avatar, reg_date) " +
				"values (@name, @email, @phone, @img, @reg_date)";

			InsertCommand.Parameters.Add("name", MySqlDbType.VarChar).Value = name;
			InsertCommand.Parameters.Add("email", MySqlDbType.VarChar).Value = email;
			InsertCommand.Parameters.Add("phone", MySqlDbType.VarChar).Value = phone;
			InsertCommand.Parameters.Add("img", MySqlDbType.Blob).Value = QueryUtils.GetImageByteArray(img);
			InsertCommand.Parameters.Add("reg_date", MySqlDbType.Date).Value = DateTime.Now;

			Conn.Open();

			InsertCommand.ExecuteNonQuery();

			Conn.Close();

			// Получение её ид
			MySqlCommand LastIdSelect = Conn.CreateCommand();
			LastIdSelect.CommandText = "select last_insert_id() as id";

			Conn.Open();

			MySqlDataReader LastIdReader = LastIdSelect.ExecuteReader();

			LastIdReader.Read();

			Id = LastIdReader.GetInt32("id");

			LastIdReader.Close();

			Conn.Close();

			GetData(Id);

		}

		// Связка с пользователем по ид
		public void AttachUser(int id)
		{

			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText =
				"insert into organization_content(organization_id, user_id) " +
 				"values (@org_id, @user_id)";

			Query.Parameters.Add("org_id", MySqlDbType.Int32).Value = Id;
			Query.Parameters.Add("user_id", MySqlDbType.Int32).Value = id;

			Conn.Open();

			Query.ExecuteNonQuery();

			Conn.Close();

		}

		// Получение данных о организации
		public bool GetData(int id)
		{

			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText = "select * from organization where id = @id";

			Query.Parameters.Add("id", MySqlDbType.Int32).Value = id;

			bool Result = true;

			Conn.Open();

			// Выгрузка данных
			MySqlDataReader QueryReader = Query.ExecuteReader();

			if (QueryReader.HasRows)
			{

				QueryReader.Read();

				Id = id;
				Name = QueryReader.GetString("name");

				if (!QueryReader.IsDBNull(2))
					About = QueryReader.GetString(2);

				Email = QueryReader.GetString("email");
				Phone = QueryReader.GetString("phone");
				Img = QueryUtils.GetImageFromByteArray((byte[])QueryReader["avatar"]);

			}
			else
				Result = false;


			QueryReader.Close();
			Conn.Close();

			return Result;
		}

		// public FilterableBandAdapter GetIncomingOrders() { }

		// public FilterableBandAdapter GetOutgoingOrders() { }

		// public int GetIncImportantOrdersCount() { }

		// public int GetOutImportantOrdersCount() { }

		// public FilterableBandAdapter GetIncImportantOrders() { }

		// public FilterableBandAdapter GetOutImportantOrders() { }

		// public void Update(string name, string phone, Image img, string about) { }

		// public BandAdapter GetAddresses() { }

		// public BandAdapter GetUsers() { }

		// public void AddAddress(string address) { }

		// public void DeleteAddress(int addressId) { }

		// public void AttachUser(string userEmail) { }

		// public void DetachUser(int userId) { }
	}
}
