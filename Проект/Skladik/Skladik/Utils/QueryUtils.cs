using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace Skladik.Utils
{
	public static class QueryUtils
	{

		public const int MaxPrice = 200000;         // Максимальная цена за единицу товара
		public const int MaxQuantity = 500000;      // Максимальное допустимое количество товара
		public const int MaxQuantityInOrder = 10000;// Максимальное допустимое количество определенного товара в заказ

		// Создание объекта соединения
		public static MySqlConnection GetConnection()
		{

			return new MySqlConnection(
				"Server=localhost;" +
				"Database=skladik;" +
				"port=3306;" +
				"User Id=root;" +
				"password=6199"
			);

		}

		// Проверяет соединение
		public static bool CheckConnection(MySqlConnection conn)
		{
			try
			{
				conn.Open();

			}
			catch
			{
				return false;

			}
			finally
			{
				conn.Close();

			}

			return true;
		}

		// Метод для хэширования строк
		public static string GetMD5Hash(string aStr)
		{

			byte[] StrBytes = Encoding.Default.GetBytes(aStr);

			MD5 Md5 = MD5.Create();

			StrBytes = Md5.ComputeHash(StrBytes);

			string Result = "";

			foreach (byte b in StrBytes)
				Result += string.Format("{0:x2}", b);

			return Result;

		}

		// Проверка элетронного адреса на соответствие формату
		public static bool CheckEmail(string email)
		{

			return new Regex(@"^[a-zA-Z0-9_\.]{6,33}@[a-z]{4,10}\.[a-z]{2,5}$").IsMatch(email);

		}

		// Проверка соотвествия пароля требованиям
		public static bool CheckPassword(string pass)
		{

			return new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])[a-zA-Z0-9]{6,16}$").IsMatch(pass);

		}

		// Проверка имён
		public static bool CheckName(string name)
		{

			return new Regex(@"^[a-zA-zа-яА-Я\s']{4,50}$").IsMatch(name);

		}

		// Проверка телефона
		public static bool CheckPhone(string phone)
		{

			return new Regex(@"^(8|(\+7))\d{10}$").IsMatch(phone);

		}

		// Проверка строки на переводимость в число и на попадание в диапазон
		public static bool CheckNum(string aStr, out int result, int start, int end)
		{

			if (int.TryParse(aStr, out result))
				if (result > start && result <= end)
					return true;

			return false;

		}

		// Проверка уникальности почты
		public static bool CheckEmailUnique(MySqlConnection conn, string tableName, string columnName, string email)
		{

			MySqlCommand FoundEmailQuery = conn.CreateCommand();
			FoundEmailQuery.CommandText = "select id from " + tableName + " where " + columnName + " = @email";

			FoundEmailQuery.Parameters.Add("email", MySqlDbType.VarChar).Value = email;

			bool Result = true;

			conn.Open();

			MySqlDataReader FoundEmailReader = FoundEmailQuery.ExecuteReader();

			if (FoundEmailReader.HasRows)
				Result = false;

			FoundEmailReader.Close();
			conn.Close();

			return Result;
		}

		// Конвертирование изображения в массив байт
		public static byte[] GetImageByteArray(Image img)
		{
			byte[] ImgBytes;

			using (MemoryStream ImgStream = new MemoryStream())
			{
				img.Save(ImgStream, img.RawFormat);
				ImgBytes = ImgStream.ToArray();
			}

			return ImgBytes;
		}

		// Конвертирование массива байт в изображения
		public static Image GetImageFromByteArray(byte[] bytes)
		{

			return (Image)new ImageConverter().ConvertFrom(bytes);

		}

		// Проверка объема картинки
		public static bool CheckImageWeight(string path, int byteNum)
		{

			FileInfo Img = new FileInfo(path);

			if (Img.Length > byteNum)
				return false;

			return true;

		}

		// Переопределение слишком длинных строк
		public static string TrimToLength(string str, int needLength)
		{

			if (str.Length > needLength)
				return str.Substring(0, needLength - 2) + "..";

			return str;

		}

		// Получение последнего добавленного ид 
		public static int GetLastInsertedId(MySqlConnection conn)
		{

			MySqlCommand LastIdSelect = conn.CreateCommand();
			LastIdSelect.CommandText = "select last_insert_id() as id";

			conn.Open();

			int Id = Convert.ToInt32(LastIdSelect.ExecuteScalar());

			conn.Close();

			return Id;

		}
	}
}
