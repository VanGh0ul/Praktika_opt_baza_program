using System.Drawing;
using System.Windows.Forms;

using Skladik.Utils;

namespace Skladik.NewComponents {
													// Элемент списка панелей пользователя
	public class UserPanel : FlowLayoutPanel {
		
		public int UserId { get; private set; }
		public Label LName { get; private set; }
		public Label LEmail { get; private set; }

		public UserPanel(int id, string name, string email) {
			
			LName = new Label();
			LEmail = new Label();

			UserId = id;

			LName.Text = name;
			LName.AutoSize = true;
			LName.Margin = new Padding(5, 3, 3, 3);
			
			LEmail.Text = email;
			LEmail.AutoSize = true;
			LEmail.Margin = new Padding(5, 3, 3, 3);

			this.Height = Styles.PanelListElementHeight;

			this.Controls.Add(LName);
			this.Controls.Add(LEmail);

		}

	}

													// Список панелей для вывода пользователей
	public class UserIdBasedPanelList : PanelList {

		public void AddAPanel(int id, string name, string email) {
			
			UserPanel NewPanel = new UserPanel(id, name, email);
			
			NewPanel.BackColor = SystemColors.ActiveCaptionText;
			NewPanel.Width = Width - 40;
			NewPanel.Height = Styles.PanelListElementHeight;

			Controls.Add(NewPanel);

		}
	
	}

}
