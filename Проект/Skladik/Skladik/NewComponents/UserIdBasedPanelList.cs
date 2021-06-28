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
		public event EventHandler UserPanelDoubleClick;

		public UserPanel() {
			LName = new Label();
			LEmail = new Label();

			this.Click += UnionClick;
			LName.Click += UnionClick;
			LEmail.Click += UnionClick;

			this.DoubleClick += UnionDoubleClick;
			LName.DoubleClick += UnionDoubleClick;
			LEmail.DoubleClick += UnionDoubleClick;

			LName.AutoSize = true;
			LName.Margin = new Padding(5, 3, 3, 3);

			LEmail.AutoSize = true;
			LEmail.Margin = new Padding(5, 3, 3, 3);

			this.FlowDirection = FlowDirection.TopDown;
			this.WrapContents = false;

			// this.Height = Styles.PanelListElementHeight;

			this.AutoSize = true;

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

													// Объединение двойных нажатий
		private void UnionDoubleClick(Object s, EventArgs e) {
			if (UserPanelDoubleClick != null)
				UserPanelDoubleClick(this, new EventArgs());
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

				if (dataSource != null && UserAddStrategy != null) {
					foreach (DataRow Row in dataSource.Rows)
						AddAPanel(UserAddStrategy(new UserPanel(), Row));

					Redraw();
				}
			}
		}

		public event EventHandler UserClick;
		public event EventHandler UserDoubleClick;

		public UserPanel SelectedUser { get; private set; }

		public int PanelsWidth { get; set;}

		public UserIdBasedPanelList() {
			SelectedUser = null;
			PanelsWidth = Width - 40;
		}

		public void AddAPanel(UserPanel user) {
		
			user.UserPanelClick += this.UserPanelClick;
			user.UserPanelDoubleClick += this.UserPanelDoubleClick;

			user.BackColor = Color.Aqua;
			user.Width = PanelsWidth;
			
													// Высота панели
			Size ElementsSize = new Size(PanelsWidth, 1000);
			user.MaximumSize = ElementsSize;
			user.MinimumSize = new Size(PanelsWidth, 30);
			user.LName.MaximumSize = ElementsSize;
			user.LEmail.MaximumSize = ElementsSize;

			Controls.Add(user);

		}
						
													// Перерисовка цветов
		private void Redraw() {

			if (SelectedUser != null)
				foreach (Control elem in this.Controls) {

					UserPanel UPanel = (UserPanel)elem;

					if (UPanel.UserId == SelectedUser.UserId) {
					
						SelectedUser = UPanel;
						SelectedUser.BackColor = Color.Yellow;
						return;

					}
				}
		}

													// Выбор пользователя
		private void UserPanelClick(Object s, EventArgs e) {

			UserPanel NewSelectedUser = (UserPanel)s;

			 if (SelectedUser == null || SelectedUser.UserId != NewSelectedUser.UserId) {
					
				if (SelectedUser != null)
					SelectedUser.BackColor = Color.Aqua;

				NewSelectedUser.BackColor = Color.Yellow;

				SelectedUser = NewSelectedUser;
				if (UserClick != null)
					UserClick(s, e);

			}
			 
			 
		}

		private void UserPanelDoubleClick(Object s, EventArgs e) {
			if (UserDoubleClick != null)
				UserDoubleClick(s, e);
		}
	
	}

}
