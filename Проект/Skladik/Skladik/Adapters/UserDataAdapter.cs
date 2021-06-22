using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

using Skladik.Utils;
using Skladik.Forms;


using System.Windows.Forms;

namespace Skladik.Adapters
{
	public class UserDataAdapter
	{

		public MySqlConnection Conn { get; private set; }
		public int Id { get; private set; }
		public string Name { get; private set; }
		public string Role { get; private set; }
		public string Email { get; private set; }
		public DateTime RegDate { get; private set; }
		public OrganizationDataAdapter Organization { get; set; }

		public UserDataAdapter(MySqlConnection conn)
		{
			Conn = conn;

		}

		// Регистрация пользователя
		public void Register(string name, string email, string pass)
		{

			// Зашифровать пароль
			string HashedPas = QueryUtils.GetMD5Hash(pass);

			// Запрос для получения ид 
			// роли обычного пользователя
			MySqlCommand GetRoleIdQuery = Conn.CreateCommand();
			GetRoleIdQuery.CommandText = "select id from user_role where name = 'usual'";

			Conn.Open();

			MySqlDataReader RoleIdReader = GetRoleIdQuery.ExecuteReader();

			int RoleId;

			try
			{
				if (!RoleIdReader.HasRows)
					throw new Exception("Роль 'usual' не была найдена");

				RoleIdReader.Read();

				RoleId = RoleIdReader.GetInt32("id");

			}
			finally
			{
				RoleIdReader.Close();
				Conn.Close();
			}

			// Отправить данные
			MySqlCommand InsertQuery = Conn.CreateCommand();
			InsertQuery.CommandText =
				"insert into user(name, email, password, role_id, reg_date)" +
				"values (@name, @email, @password, @role_id, @reg_date)";

			InsertQuery.Parameters.Add("name", MySqlDbType.VarChar).Value = name;
			InsertQuery.Parameters.Add("email", MySqlDbType.VarChar).Value = email;
			InsertQuery.Parameters.Add("password", MySqlDbType.VarChar).Value = HashedPas;
			InsertQuery.Parameters.Add("role_id", MySqlDbType.Int32).Value = RoleId;
			InsertQuery.Parameters.Add("reg_date", MySqlDbType.Date).Value = DateTime.Now;

			Conn.Open();

			InsertQuery.ExecuteNonQuery();

			Conn.Close();
		}

		// Авторизация пользователя
		public bool Auth(string email, string pass)
		{

			// Получение Ид пользователя
			MySqlCommand GetUserIdQuery = Conn.CreateCommand();
			GetUserIdQuery.CommandText = "select id from user where email = @email and password = @pass";

			GetUserIdQuery.Parameters.Add("email", MySqlDbType.VarChar).Value = email;
			GetUserIdQuery.Parameters.Add("pass", MySqlDbType.VarChar).Value = QueryUtils.GetMD5Hash(pass);

			bool Result = false;
			int Temp;

			Conn.Open();

			MySqlDataReader UserIdReader = GetUserIdQuery.ExecuteReader();

			if (UserIdReader.HasRows)
			{

				UserIdReader.Read();

				Temp = UserIdReader.GetInt32("id");

				UserIdReader.Close();

				Conn.Close();

				// Получение данных пользователя
				Result = GetData(Temp);

			}
			else
				Conn.Close();

			return Result;
		}

		// Получение данных пользователя
		// И его организации
		public bool GetData(int id)
		{

			// Составление запроса
			MySqlCommand SelectQuery = Conn.CreateCommand();
			SelectQuery.CommandText =
				"select u.name, u.email, r.name as role_name, u.reg_date, oc.organization_id as org_id " +
				"from " +
					"user u inner join user_role r on u.role_id = r.id " +
 					"left join organization_content oc on u.id = oc.user_id " +
				"where u.id = @id";

			SelectQuery.Parameters.Add("id", MySqlDbType.Int32).Value = id;

			Conn.Open();

			MySqlDataReader SelectReader = SelectQuery.ExecuteReader();

			bool Result = true;

			if (SelectReader.HasRows)
			{
				// Если данные есть, их запись
				SelectReader.Read();

				int OrgId;

				Id = id;
				Name = SelectReader.GetString("name");
				Role = SelectReader.GetString("role_name");
				Email = SelectReader.GetString("email");
				RegDate = SelectReader.GetDateTime("reg_date");

				// Если есть организация,
				// считывание информации о ней
				if (!SelectReader.IsDBNull(4))
				{

					OrgId = SelectReader.GetInt32("org_id");

					Conn.Close();
					Organization = new OrganizationDataAdapter(Conn);

					if (!Organization.GetData(OrgId))
						Organization = null;


				}

			}
			else
				Result = false;

			SelectReader.Close();
			Conn.Close();

			return Result;

		}

		/*
		public void Update (string name, string pass) {
		
		}

		public void IncreaseRole() {
		
		}
		*/
	}
}
