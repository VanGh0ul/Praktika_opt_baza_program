using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

using Skladik.Utils;
using Skladik.Forms;
using Skladik.Adapters;

using System.Drawing;
using Skladik.NewComponents;
using Skladik.Adapters.BandAdapters;

namespace Skladik
{
	public partial class Form1 : Form
	{

		public MySqlConnection Conn { get; private set; }

		public UserDataAdapter User { get; private set; }

		public Form1()
		{
			InitializeComponent();

			Conn = QueryUtils.GetConnection();
			User = new UserDataAdapter(Conn);

		}

		private void Form1_Load(object sender, EventArgs e)
		{
			 new DynAuthForm().Generate(this);
			if (!QueryUtils.CheckConnection(Conn))
				MessageBox.Show("Ну удалось установить соединение с БД");

		}

	}
}
