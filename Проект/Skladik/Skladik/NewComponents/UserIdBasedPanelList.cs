using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using Skladik.Utils;

namespace Skladik.NewComponents {
													// Элемент списка панелей пользователя
	public class UserPanel : FlowLayoutPanel {
		
		public int UserId { get; set; }
		public Label LName { get; set; }
		public Label LEmail { get; set; }

		public event EventHandler UserPanelClick;

		public UserPanel() {
			LName = new Label();
			LEmail = new Label();

			this.Click += UnionClick;
			LName.Click += UnionClick;
			LEmail.Click += UnionClick;

			LName.AutoSize = true;
			LName.Margin = new Padding(5, 3, 3, 3);

			LEmail.AutoSize = true;
			LEmail.Margin = new Padding(5, 3, 3, 3);

			this.FlowDirection = FlowDirection.TopDown;
			this.WrapContents = false;

			this.Height = Styles.PanelListElementHeight;

			this.Controls.Add(LName);
			this.Controls.Add(LEmail);
		}

		public UserPanel(int id, string name, string email) : this() {

			UserId = id;
			LName.Text = name;
			LEmail.Text = email;

		}

													// Объединение нажатий
		private void UnionClick(Object s, EventArgs e) {
			if (UserPanelClick != null)
				UserPanelClick(this, new EventArgs());
		}

	}


	public delegate UserPanel DUserAddStrategy(UserPanel newUser, DataRow Row);

													// Список панелей для вывода пользователей
	public class UserIdBasedPanelList : PanelList {

													// Алгоритм добавления нового пользователя
		public DUserAddStrategy UserAddStrategy { get; set; }

		private DataTable dataSource;
		public DataTable DataSource { 
			get { return dataSource; }
			set {
				dataSource = value;

				if (dataSource != null && UserAddStrategy != null)
					foreach (DataRow Row in dataSource.Rows)
						AddAPanel(UserAddStrategy(new UserPanel(), Row));
			}
		}

		public event EventHandler UserSelected;

		public void AddAPanel(UserPanel user) {
		
			user.UserPanelClick += this.UserPanelClick;

			user.BackColor = SystemColors.ActiveCaptionText;
			user.Width = Width - 40;
			
													// Высота панели
			if (user.LName.Text.Length > 40)
				user.Height = Styles.PanelListElementHeight + 20;
			else
				user.Height = Styles.PanelListElementHeight;

			if (user.LEmail.Text.Length > 25)
				user.Height += 25;

			Controls.Add(user);

		}
													
													// Выбор пользователя
		private void UserPanelClick(Object s, EventArgs e) {
			if (UserSelected != null)
				UserSelected(s, e);
		}
	
	}

}
