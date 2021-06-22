using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using Skladik.NewComponents;
using Skladik.Utils;
using Skladik.Adapters;

namespace Skladik.Forms
{
	class DynOrgCreationForm : DynForm
	{

		//public TableLayoutPanel formContent { get; private set; }
		public Label LTitle { get; private set; }
		public Label LName { get; private set; }
		public TextBox TbName { get; private set; }
		public Label LEmail { get; private set; }
		public TextBox TbEmail { get; private set; }
		public Label LPhone { get; private set; }
		public TextBox TbPhone { get; private set; }
		public OpenImageString OiString { get; private set; }
		public PictureBox PbPreView { get; private set; }
		public BackNextButtons BnbButtons { get; private set; }

		private DynRegisterForm RegistrationForm;

		public DynOrgCreationForm()
		{
			formContent = new TableLayoutPanel();
			LTitle = new Label();
			LName = new Label();
			TbName = new TextBox();
			LEmail = new Label();
			TbEmail = new TextBox();
			LPhone = new Label();
			TbPhone = new TextBox();
			OiString = new OpenImageString();
			PbPreView = new PictureBox();
			BnbButtons = new BackNextButtons();

			OiString.AfterImgLoad += PictureBoxLoadImage;
			BnbButtons.BackClick += BackButtonClick;
			BnbButtons.NextClick += NextButtonClick;

			#region Свойства компонентов 

			LTitle.Text = "Новая организация";
			LTitle.AutoSize = true;
			LTitle.Anchor = AnchorStyles.None;
			LTitle.Font = new Font("Helvetica", 25);
			LTitle.TextAlign = ContentAlignment.MiddleCenter;

			LName.Text = "Название";
			Styles.TextStyle(LName);

			Styles.TextBoxStyle(TbName);

			LEmail.Text = "Электронный адрес";
			Styles.TextStyle(LEmail);

			Styles.TextBoxStyle(TbEmail);

			LPhone.Text = "Телефон";
			Styles.TextStyle(LPhone);

			Styles.TextBoxStyle(TbPhone);

			Styles.TextStyle(OiString.LComment);
			OiString.BOpenImg.Font = Styles.TextFont;
			OiString.Dock = DockStyle.Fill;

			PbPreView.Size = new Size(180, 180);
			PbPreView.Anchor = AnchorStyles.None;
			PbPreView.BackgroundImageLayout = ImageLayout.Zoom;
			PbPreView.BackgroundImage = Properties.Resources.no_image;

			BnbButtons.BBack.Font = Styles.TextFont;
			BnbButtons.BNext.Font = Styles.TextFont;

			#endregion

			#region Составление таблицы

			// Вся стр
			formContent.Dock = DockStyle.Fill;

			// Столбцы
			formContent.ColumnCount = 3;
			formContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12));
			formContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 76));
			formContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12));

			// Строки
			formContent.RowCount = 15;

			// Пустой промежуток
			formContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

			// Заголовок
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Пустой промежуток
			formContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Название
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Поле
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Электронный адрес
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Поле
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Телефон
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Поле
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Компонент открытия 
			// изображений
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Пустой промежуток
			formContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Картинка
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Пустой промежуток
			formContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Кнопки
			formContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Пустой промежуток
			formContent.RowStyles.Add(new RowStyle(SizeType.Percent, 60));

			// Добавление компонентов
			formContent.Controls.Add(LTitle, 1, 1);
			formContent.Controls.Add(LName, 1, 3);
			formContent.Controls.Add(TbName, 1, 4);
			formContent.Controls.Add(LEmail, 1, 5);
			formContent.Controls.Add(TbEmail, 1, 6);
			formContent.Controls.Add(LPhone, 1, 7);
			formContent.Controls.Add(TbPhone, 1, 8);
			formContent.Controls.Add(OiString, 1, 9);
			formContent.Controls.Add(PbPreView, 1, 11);
			formContent.Controls.Add(BnbButtons, 1, 13);

			#endregion

		}

		protected override void SetUpMainForm()
		{
			programForm.Controls.Clear();
			Size FormSize = new Size(380, 600);
			programForm.MinimumSize = FormSize;
			programForm.MaximumSize = FormSize;
			programForm.Size = FormSize;
			programForm.Text = "Создание организации";
		}


		// Генерация формы
		public override void Generate(Form1 aForm)
		{

			base.Generate(aForm);

			programForm.Controls.Add(formContent);


		}


		public void Generate(Form1 aForm, DynRegisterForm regForm)
		{

			this.Generate(aForm);

			RegistrationForm = regForm;

		}


		// Выгрузка изображения на форму
		private void PictureBoxLoadImage(Object s, EventArgs e)
		{

			// Проверка объема изображения
			if (QueryUtils.CheckImageWeight(OiString.FileName, 4200000))
				PbPreView.BackgroundImage = Image.FromFile(OiString.FileName);

			else
				MessageBox.Show("Объем изображения не должен превышать 4 МБ");

		}

		// Нажатие на кнопку назад
		private void BackButtonClick(Object s, EventArgs e)
		{
			programForm.History.Pop().RegenerateOldForm();
		}

		// Кнопка Далее
		private void NextButtonClick(Object s, EventArgs e)
		{

			string Name = TbName.Text.Trim();
			string Email = TbEmail.Text.Trim();
			string Phone = TbPhone.Text.Trim();
			Image Img = PbPreView.BackgroundImage;

			// Проверка названия организации
			if (!QueryUtils.CheckName(Name))
			{
				MessageBox.Show("Название организации должно содержать от 4 до 50 символов, а так же не может сожержать спец. символы");
				return;
			}

			// Проверка формата электронного адреса
			if (!QueryUtils.CheckEmail(Email))
			{
				MessageBox.Show("Электронный адрес неправильного формата");
				return;
			}

			// Проверка формата телефона
			if (!QueryUtils.CheckPhone(Phone))
			{
				MessageBox.Show("Телефон неправильного формата, формат телефона: +79999999999 или 89999999999");
				return;
			}

			// Проверка уникальности адреса
			if (!QueryUtils.CheckEmailUnique(programForm.Conn, "organization", "email", Email))
			{
				MessageBox.Show("Данный электронный адрес уже есть в системе");
				return;
			}

			// Если передана форма регистрации
			// Регистрация человека
			// Вход в систему
			if (RegistrationForm != null)
			{
				programForm.User.Register(
					RegistrationForm.TbName.Text,
					RegistrationForm.TbEmail.Text,
					RegistrationForm.PfPassword.TextField.Text
				);

				programForm.User.Auth(
					RegistrationForm.TbEmail.Text,
					RegistrationForm.PfPassword.TextField.Text
				);
			}

			// Создание организации
			OrganizationDataAdapter NewOrg = new OrganizationDataAdapter(programForm.User.Conn);

			NewOrg.Register(Name, Email, Phone, Img);

			programForm.User.Organization = NewOrg;

			// Привязка человека
			NewOrg.AttachUser(programForm.User.Id);

			// Если передана форма регистрации
			if (RegistrationForm != null)
				// Переход на форму товаров
				new DynProductsBandForm().Generate(programForm);

			// Если не передана
			else
				// Переход назад
				programForm.History.Pop().RegenerateOldForm();


		}

	}
}
