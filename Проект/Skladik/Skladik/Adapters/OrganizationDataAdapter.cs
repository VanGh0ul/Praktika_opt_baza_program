using System;
using System.IO;
using System.Drawing;
using MySql.Data.MySqlClient;

using Skladik.Utils;
using Skladik.Adapters.BandAdapters;

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

		public bool ImgChanged { get; set; }

		public OrganizationDataAdapter(MySqlConnection conn)
		{
			Conn = conn;
			ImgChanged = false;
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

			GetData(QueryUtils.GetLastInsertedId(Conn));

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

		// Отвзяка пользователя
		public void DetachUser(int userId)
		{

			MySqlCommand Query = Conn.CreateCommand();
			Query.CommandText =
				"delete from organization_content where organization_id = @org_id and user_id = @user_id";

			Query.Parameters.Add("org_id", MySqlDbType.Int32).Value = Id;
			Query.Parameters.Add("user_id", MySqlDbType.Int32).Value = userId;

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
				RegDate = QueryReader.GetDateTime("reg_date");

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

		// Изменение данных
		public bool Update(string name, string phone, Image img, string about)
		{

			MySqlCommand Query = Conn.CreateCommand();

			// Составление запроса
			string CommandInner = "";

			if (name != Name)
			{
				CommandInner += " name = @name ";
				Query.Parameters.Add("name", MySqlDbType.VarChar).Value = name;
			}

			if (about != About)
			{

				if (CommandInner.Length > 0)
					CommandInner += " , ";

				CommandInner += " about = @about ";
				Query.Parameters.Add("about", MySqlDbType.VarChar).Value = about;
			}

			if (phone != Phone)
			{

				if (CommandInner.Length > 0)
					CommandInner += " , ";

				CommandInner += " phone = @phone ";
				Query.Parameters.Add("phone", MySqlDbType.VarChar).Value = phone;
			}

			if (this.ImgChanged)
			{

				if (CommandInner.Length > 0)
					CommandInner += " , ";

				CommandInner += " avatar = @img ";
				Query.Parameters.Add("img", MySqlDbType.Blob).Value = QueryUtils.GetImageByteArray(img);
			}

			// Отправка запроса
			if (CommandInner.Length > 0)
			{

				Query.CommandText = "update organization set " + CommandInner + " where id = @id";
				Query.Parameters.Add("id", MySqlDbType.Int32).Value = Id;

				Conn.Open();

				Query.ExecuteNonQuery();

				Conn.Close();

				this.ImgChanged = false;

				return true;
			}

			return false;
		}

		// Адаптер списка работников компании
		public BandAdapter GetUsers()
		{

			MySqlCommand SelectCommand = Conn.CreateCommand();
			SelectCommand.CommandText =
				"select u.id, u.name, u.email " +
				"from organization_content oc, user u " +
				"where " +
					"oc.organization_id = @id and " +
					"oc.user_id = u.id ";

			SelectCommand.Parameters.Add("id", MySqlDbType.Int32).Value = Id;

			MySqlCommand SelectCount = Conn.CreateCommand();
			SelectCount.CommandText =
				"select count(*) " +
				"from organization_content oc " +
				"where " +
					"oc.organization_id = @id";

			SelectCount.Parameters.Add("id", MySqlDbType.Int32).Value = Id;

			return new BandAdapter(SelectCommand, SelectCount, Styles.UserListElemCount, Conn);

		}
	}
}
