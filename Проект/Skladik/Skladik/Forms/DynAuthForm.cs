using System;
using System.Windows.Forms;
using System.Drawing;

using Skladik.NewComponents;
using Skladik.Adapters;
using Skladik.Forms;
using Skladik.Utils;


using Skladik.Adapters.BandAdapters;
using DbExtensions;

namespace Skladik.Forms {
	class DynAuthForm : DynForm{

		// public TableLayoutPanel formContent { get; private set; }
		public Label LTitle { get; private set; }
		public Label LEmail { get; private set; }
		public TextBox TbEmail { get; private set; }
		public Label LPassword { get; private set; }
		public PasswordField PfPassword { get; private set; }
		public Button BAuth { get; private set; }
		public Label LRegister { get; private set; }


		public DynAuthForm() { 
		
			formContent = new TableLayoutPanel();
			LTitle = new Label();
			LEmail = new Label();
			TbEmail = new TextBox();
			LPassword = new Label();
			PfPassword = new PasswordField();
			BAuth = new Button();
			LRegister = new Label();
			
			// formContent.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			BAuth.Click += AuthButtonClick;
			LRegister.Click += RegistrationFormOpenButton;

			// Temporary
			TbEmail.Text = "ThisAdmin@mail.ru";
			PfPassword.TextField.Text = "Rootofsys1";
			// --/

			#region Свойства компонентов

			LTitle.Text = "Вход";
			LTitle.AutoSize = true;
			LTitle.Height = 10;
			LTitle.Width = 10;
			LTitle.Anchor = AnchorStyles.None;
			LTitle.Font = new Font("Comic Sans MS", 25);

			LEmail.Text = "Электронный адрес";
			Styles.TextStyle(LEmail);

			Styles.TextBoxStyle(TbEmail);

			LPassword.Text = "Пароль";
			Styles.TextStyle(LPassword);

			PfPassword.Dock = DockStyle.Fill;
			PfPassword.TextField.Font = Styles.TextFont;
			PfPassword.ViewButton.BackgroundImageLayout = ImageLayout.Zoom;
			PfPassword.ViewButton.BackgroundImage = Properties.Resources.see_password;

			BAuth.Text = "Вход";
			BAuth.Height = 43;
			BAuth.Width = 120;
			BAuth.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
			BAuth.Font = Styles.TextFont;

			LRegister.Text = "Регистрация";
			LRegister.Font = new Font("Comic Sans MS", 10, FontStyle.Underline);
			LRegister.Cursor = Cursors.Hand;
			LRegister.ForeColor = Color.Blue;
			LRegister.Anchor = AnchorStyles.None;
			#endregion

			#region Код таблицы

			// formContent.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			formContent.Dock = DockStyle.Fill;

			formContent.ColumnCount = 3;
			formContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
			formContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
			formContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

			formContent.RowCount = 12;
			// Пустой промежуток
			formContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

			// Вход
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Пустой промежуток
			formContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Электронный адрес
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Поле Электронный адрес
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Пустой промежуток
			formContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Пароль
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Поле Пароль
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Пустой промежуток
			formContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Кнопка Вход
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Регистрация
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Пустой промежуток
			formContent.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

			
			formContent.Controls.Add(LTitle, 1, 1);
			formContent.Controls.Add(LEmail, 1, 3);
			formContent.Controls.Add(TbEmail, 1, 4);
			formContent.Controls.Add(LPassword, 1, 6);
			formContent.Controls.Add(PfPassword, 1, 7);
			formContent.Controls.Add(BAuth, 1, 9);
			formContent.Controls.Add(LRegister, 1, 10);
			
			#endregion

		}

		protected override void SetUpMainForm() {
			programForm.Controls.Clear();
			Size FormSize = new Size(400, 400);
			programForm.Location = Styles.CentralizeFormByDesktop(FormSize);
			programForm.MinimumSize = FormSize;
			programForm.MaximumSize = FormSize;
			programForm.Size = FormSize;
			programForm.Text = "Вход";
		} 
		
		
													// Генерация формы авторизации
		public override void Generate(Form1 aForm) {
			
			base.Generate(aForm);
			programForm.Controls.Add(formContent);

		}
		

													// Вход в систему
		private void AuthButtonClick(Object s, EventArgs e) {

			string Email = TbEmail.Text.Trim();
			string Pass = PfPassword.TextField.Text.Trim();

													// Проверка электронной почты
			if (!QueryUtils.CheckEmail(Email)) {
				MessageBox.Show("Электронная почта имеет неправильный формат");
				return;
			}

			if (QueryUtils.CheckPassword(Pass) && programForm.User.Auth(Email, Pass)) 
													// Переход на форму товаров
				new DynProductsBandForm().Generate(programForm);

			else
				MessageBox.Show("Логин/пароль введен неверно");
		}

													// Перемещение на форму регистрации
		private void RegistrationFormOpenButton(Object s, EventArgs e) {
			new DynRegisterForm().Generate(programForm);
			programForm.History.Push(this);
		}
	

	}
}
