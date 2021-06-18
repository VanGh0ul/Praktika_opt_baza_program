using System;
using System.Windows.Forms;

namespace Skladik.NewComponents {
	// Класс поля ввода пароля с кнопкой для отображения содержимого
	public class PasswordField : TableLayoutPanel {
		
		public TextBox TextField { get; private set; }
		public Button ViewButton { get; private set; }

		public PasswordField() {

			TextField = new TextBox();
			ViewButton = new Button();
			
			this.AutoSize = true;

			this.Margin = new Padding(0);
			this.Padding = new Padding(0);

			#region Свойства компонентов
			TextField.PasswordChar = '*';
			TextField.Margin = new Padding(3, 0, 0, 0);
			TextField.Dock = DockStyle.Fill;

			ViewButton.Height = TextField.Height;
			ViewButton.Width = 30;
			ViewButton.Margin = new Padding(0);
			ViewButton.Dock = DockStyle.Fill;

			ViewButton.MouseDown += delegate(Object s, MouseEventArgs e) {
				TextField.PasswordChar = '\0';
			};

			ViewButton.MouseUp += delegate(Object s, MouseEventArgs e) {
				TextField.PasswordChar = '*';
			};
			#endregion

			#region Расположение элементов
			this.ColumnCount = 2;
			this.RowCount = 1;

			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

			this.Controls.Add(TextField, 0, 0);
			this.Controls.Add(ViewButton, 1, 0);

			this.Controls.Add(TextField);
			this.Controls.Add(ViewButton);
			#endregion
		}

	}

}
