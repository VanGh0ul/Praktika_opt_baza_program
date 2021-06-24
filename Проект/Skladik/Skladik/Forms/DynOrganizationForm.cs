using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

using Skladik.Utils;
using Skladik.NewComponents;
using Skladik.Adapters;
using Skladik.Adapters.BandAdapters;

namespace Skladik.Forms
{
	class DynOrganizationForm : DynForm
	{

		#region Объявления
		// Адаптер организации
		private OrganizationDataAdapter Org;
		private BandAdapter UsersAdapter;

		// Кнопки перехода
		public BackMainHeader Header { get; private set; }

		// Картинка организации
		public PictureBox PbOrgImage { get; private set; }

		// Для чтения
		public Label LName { get; private set; }
		public Label LEmail { get; private set; }
		public Label LPhone { get; private set; }
		public Label LRegDate { get; private set; }
		public RichTextBox RtbAbout { get; private set; }

		// Для изменения
		public TextBox TbName { get; private set; }
		public TextBox TbPhone { get; private set; }
		public OpenImageString OiString { get; private set; }
		public Button BSave { get; private set; }
		public Button BCancel { get; private set; }

		// Управление справочником адресов
		public Paginator PagAdrList { get; private set; }
		public IdBasedPanelList IdPlAdrList { get; private set; }
		public Button BDeleteAddress { get; private set; }
		public TextBox TbAddress { get; private set; }
		public Button BAddAddress { get; private set; }

		// Переход на списки заказов
		public Button BIncOrders { get; private set; }
		public Button BOutOrders { get; private set; }

		// Просмотр товаров организации
		public Button BProducts { get; private set; }

		// Управление привязкой сотрудников организации
		public Paginator PagUserList { get; private set; }
		public UserIdBasedPanelList UserList { get; private set; }
		public Button BDetach { get; private set; }
		public TextBox TbEmailAttach { get; private set; }
		public Button BAttach { get; private set; }

		#endregion

		// Настройка формы
		protected override void SetUpMainForm()
		{
			programForm.Controls.Clear();
			Size FormSize = new Size(1000, 600);
			programForm.Location = Styles.CentralizeFormByAnotherOne(FormSize, programForm.Location, programForm.Size);
			programForm.MinimumSize = FormSize;
			programForm.MaximumSize = new Size(1500, 800); ;
			programForm.Size = FormSize;
			programForm.Text = "Личный кабинет организации";
		}

		public void Generate(Form1 aForm, int organizationId)
		{

			base.Generate(aForm);

			// Если пользователь администратор
			if (programForm.User.Role == "admin")
			{

				Org = new OrganizationDataAdapter(programForm.Conn);

				// Если не удалось считывать данные
				if (!Org.GetData(organizationId))
				{
					MessageBox.Show("Произошла ошибка при открытии личного кабинета организации");
					programForm.History.Pop().RegenerateOldForm();
				}

				formContent = CreateEditableForm();
				EditableFormSetUp();
				FillEditableData();

				// Если пользователь не привязан или 
				// открывает личный каб. не своей организации
			}
			else if (programForm.User.Organization == null || organizationId != programForm.User.Organization.Id)
			{

				Org = new OrganizationDataAdapter(programForm.Conn);

				// Если не удалось считывать данные
				if (!Org.GetData(organizationId))
				{
					MessageBox.Show("Произошла ошибка при открытии личного кабинета организации");
					programForm.History.Pop().RegenerateOldForm();
				}

				formContent = CreateReadOnlyForm();
				EventSetUp();
				FillReadOnlyData();

				// Если привязанный пользователь открывает 
				// ЛК своей организации
			}
			else
			{

				Org = programForm.User.Organization;

				formContent = CreateEditableForm();
				EditableFormSetUp();
				FillEditableData();

			}

			programForm.Controls.Add(formContent);

		}

		#region Рамки
		// Создание панели, что можно скроллить
		private TableLayoutPanel CreateScrollableView(TableLayoutPanel mainFrame)
		{

			// formContent типа TableLayoutPanel
			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, BorderStyle);

			Result.ColumnCount = 1;
			Result.RowCount = 1;

			Result.AutoScroll = true;
			Result.VerticalScroll.Enabled = true;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Для расширения площади формы
			Panel Content = new Panel();
			Content.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			Content.Size = new Size(Result.Width, Styles.OrganizationFormScrollPanelHeight);

			Content.Margin = new Padding(0);

			Content.AutoScroll = true;
			Content.AutoSize = false;


			Content.Controls.Add(mainFrame);

			Result.Controls.Add(Content, 0, 0);

			return Result;

		}

		// Создание основной рамки формы
		private TableLayoutPanel CreateMainFrame()
		{

			// Сама рамка, что будет содержать элементы
			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, BorderStyle);

			Result.ColumnCount = 3;
			Result.RowCount = 7;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Кнопки перехода
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Верхняя часть
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 0.4f * programForm.Height));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Нижняя часть
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 60));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			return Result;

		}

		// Создание рамки для списка
		private TableLayoutPanel CreateListFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			#region Рамка

			Result.ColumnCount = 3;
			Result.RowCount = 8;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Пагинатор
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Список
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 70));

			// Кнопка
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

			// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Поле и кнопка
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

			#endregion

			return Result;

		}


		#region Части формы

		#region Верхняя часть формы
		// Составление верхней части формы
		private TableLayoutPanel CreateTopPartContent(TableLayoutPanel orgInfoPanel)
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, BorderStyle);

			Result.ColumnCount = 3;
			Result.RowCount = 1;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));

			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			// Картинка 
			PbOrgImage = new PictureBox();
			Styles.ImgStyle(PbOrgImage);

			// Добавление содержимого верхней части
			Result.Controls.Add(PbOrgImage, 0, 0);
			Result.Controls.Add(orgInfoPanel, 2, 0);

			return Result;

		}

		// Информация об организации только для чтения
		private TableLayoutPanel ReadOnlyOrgInfo()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, BorderStyle);

			#region Рамка

			Result.ColumnCount = 3;
			Result.RowCount = 7;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

			// Название
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Пустой промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));
			// Эл адрес
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Телефон
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Дата регистрации
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Пустой промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Компоненты

			LName = new Label();
			Styles.TextStyle(LName, 15);
			LName.Margin = new Padding(5, 10, 5, 10);

			LEmail = new Label();
			Styles.TextStyle(LEmail);

			LPhone = new Label();
			Styles.TextStyle(LPhone);

			LRegDate = new Label();
			Styles.TextStyle(LRegDate);

			Result.Controls.Add(LName, 1, 0);
			Result.Controls.Add(LEmail, 1, 2);
			Result.Controls.Add(LPhone, 1, 3);
			Result.Controls.Add(LRegDate, 1, 4);

			#endregion

			return Result;

		}

		// Информация об организации, редактируемая
		private TableLayoutPanel EditableOrgInfo()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, BorderStyle);

			#region Рамка

			Result.ColumnCount = 3;
			Result.RowCount = 9;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

			// Название
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Пустой промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));
			// Эл адрес
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Телефон
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
			// Дата регистрации
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Изменить изображение
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Пустой промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			// Кнопки
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
			// Пустой промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			#endregion

			#region Компоненты

			// Название
			TbName = new TextBox();
			Styles.TextBoxStyle(TbName);

			// Эл адрес
			LEmail = new Label();
			Styles.TextStyle(LEmail);

			#region Телефон

			TableLayoutPanel PhoneFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(PhoneFrame, TableLayoutPanelCellBorderStyle.None);

			PhoneFrame.ColumnCount = 3;
			PhoneFrame.RowCount = 1;

			// Текст
			PhoneFrame.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			// Поле
			PhoneFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
			// Пустой промежуток
			PhoneFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			PhoneFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			Label LPhoneText = new Label();
			Styles.TextStyle(LPhoneText);
			//LPhoneText.Margin = new Padding(0);
			LPhoneText.Anchor = AnchorStyles.Left;
			LPhoneText.Text = "Телефон: ";

			TbPhone = new TextBox();
			Styles.TextBoxStyle(TbPhone);

			PhoneFrame.Controls.Add(LPhoneText, 0, 0);
			PhoneFrame.Controls.Add(TbPhone, 1, 0);

			#endregion

			// Дата регистрации
			LRegDate = new Label();
			Styles.TextStyle(LRegDate);

			// Выбор изображения
			OiString = new OpenImageString();
			Styles.TextStyle(OiString.LComment);
			OiString.LComment.Margin = new Padding(2, 0, 0, 0);
			OiString.LComment.Text = "Изменить изоражение: ";
			OiString.BOpenImg.Font = Styles.TextFont;

			#region Кнопки

			TableLayoutPanel ButtonsFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(ButtonsFrame, TableLayoutPanelCellBorderStyle.None);

			ButtonsFrame.ColumnCount = 3;
			ButtonsFrame.RowCount = 1;

			// Текст
			ButtonsFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			// Поле
			ButtonsFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
			// Пустой промежуток
			ButtonsFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			ButtonsFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			BSave = new Button();
			Styles.ButtonStyle(BSave);
			BSave.Text = "Сохранить";
			BSave.Margin = new Padding(2);

			BCancel = new Button();
			Styles.ButtonStyle(BCancel);
			BCancel.Text = "Отменить";
			BCancel.Margin = new Padding(2);

			ButtonsFrame.Controls.Add(BSave, 1, 0);
			ButtonsFrame.Controls.Add(BCancel, 2, 0);

			#endregion

			Result.Controls.Add(TbName, 1, 0);
			Result.Controls.Add(LEmail, 1, 2);
			Result.Controls.Add(PhoneFrame, 1, 3);
			Result.Controls.Add(LRegDate, 1, 4);
			Result.Controls.Add(OiString, 1, 5);
			Result.Controls.Add(ButtonsFrame, 1, 7);

			#endregion

			return Result;

		}

		#endregion

		#region Нижняя часть формы

		// Составление нижней части формы
		private TableLayoutPanel CreateBottomPartContent()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, BorderStyle);

			#region Рамка

			Result.ColumnCount = 3;
			Result.RowCount = 1;

			// Левая часть
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
			// Промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			// Правая часть
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Левая часть

			#region Рамка

			TableLayoutPanel LeftSide = new TableLayoutPanel();
			Styles.ContentFramesStyle(LeftSide, BorderStyle);

			LeftSide.ColumnCount = 1;
			LeftSide.RowCount = 3;

			LeftSide.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Описание
			LeftSide.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
			// Промежуток
			LeftSide.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));
			// Адреса
			LeftSide.RowStyles.Add(new RowStyle(SizeType.Percent, 70));

			#endregion

			#region Компоненты

			// Описание
			RtbAbout = new RichTextBox();
			Styles.RichTextBoxStyle(RtbAbout);

			LeftSide.Controls.Add(RtbAbout, 0, 0);
			LeftSide.Controls.Add(CreateAddressListFrame(), 0, 2);

			#endregion

			#endregion

			#region Правая часть

			#region Рамка

			TableLayoutPanel RightSide = new TableLayoutPanel();
			Styles.ContentFramesStyle(RightSide, BorderStyle);

			RightSide.ColumnCount = 1;
			RightSide.RowCount = 5;

			RightSide.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Переходы к заказам
			RightSide.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

			// Промежуток
			RightSide.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Товары компании
			RightSide.RowStyles.Add(new RowStyle(SizeType.Percent, 15));

			// Промежуток
			RightSide.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Сотрудникни
			RightSide.RowStyles.Add(new RowStyle(SizeType.Percent, 60));

			#endregion

			#region Компоненты

			RightSide.Controls.Add(CreateOrdersFrame(), 0, 0);
			RightSide.Controls.Add(CreateProductsFrame(), 0, 2);
			RightSide.Controls.Add(CreateWorkersListFrame(), 0, 4);

			#endregion

			#endregion

			Result.Controls.Add(LeftSide, 0, 0);
			Result.Controls.Add(RightSide, 2, 0);

			return Result;

		}


		// Часть формы, что касается адресов
		private TableLayoutPanel CreateAddressListFrame()
		{

			TableLayoutPanel Result = CreateListFrame();

			#region Компоненты

			// Текст
			Label LAdrListText = new Label();
			Styles.TextStyle(LAdrListText, 12);
			LAdrListText.Text = "Список адресов организации";

			// Пагинатор
			PagAdrList = new Paginator(Styles.PaginatorLabelCount);
			Styles.PaginatorStyle(PagAdrList);
			PagAdrList.Anchor = AnchorStyles.Right;

			// Список
			IdPlAdrList = new IdBasedPanelList();
			IdPlAdrList.Font = Styles.TextFont;
			IdPlAdrList.PanelsWidth = IdPlAdrList.Width * 2;

			#region Кнопка удаления адреса

			TableLayoutPanel DeleteButtonFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(DeleteButtonFrame, TableLayoutPanelCellBorderStyle.None);

			DeleteButtonFrame.ColumnCount = 2;
			DeleteButtonFrame.RowCount = 1;

			// Промежуток
			DeleteButtonFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Кнопка
			DeleteButtonFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			DeleteButtonFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			BDeleteAddress = new Button();
			Styles.ButtonStyle(BDeleteAddress);
			BDeleteAddress.Font = new Font("Helvetica", 9);
			BDeleteAddress.Text = "Удалить адрес";

			DeleteButtonFrame.Controls.Add(BDeleteAddress, 1, 0);

			#endregion

			// Текст
			Label LAddAddressText = new Label();
			Styles.TextStyle(LAddAddressText, 12);
			LAddAddressText.Margin = new Padding(0, 0, 0, 5);
			LAddAddressText.Text = "Добавить адрес доставки: ";

			#region Поле и кнопка добавления адреса

			TableLayoutPanel AddAddressFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(AddAddressFrame, TableLayoutPanelCellBorderStyle.None);

			AddAddressFrame.ColumnCount = 3;
			AddAddressFrame.RowCount = 1;

			// Поле ввода
			AddAddressFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));

			// Кнопка
			AddAddressFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			// Промежуток
			AddAddressFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			AddAddressFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			// Поле ввода адреса
			TbAddress = new TextBox();
			Styles.TextBoxStyle(TbAddress);

			// Кнопка
			BAddAddress = new Button();
			Styles.ButtonStyle(BAddAddress);
			BAddAddress.Text = "Добавить";
			BAddAddress.Margin = new Padding(5, 0, 0, 0);

			AddAddressFrame.Controls.Add(TbAddress, 0, 0);
			AddAddressFrame.Controls.Add(BAddAddress, 1, 0);

			#endregion

			Result.Controls.Add(LAdrListText, 1, 1);
			Result.Controls.Add(PagAdrList, 1, 2);
			Result.Controls.Add(IdPlAdrList, 1, 3);
			Result.Controls.Add(DeleteButtonFrame, 1, 4);
			Result.Controls.Add(LAddAddressText, 1, 5);
			Result.Controls.Add(AddAddressFrame, 1, 6);

			#endregion

			return Result;

		}

		// Часть формы, что касается заказов
		private TableLayoutPanel CreateOrdersFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			#region Рамка

			Result.ColumnCount = 4;
			Result.RowCount = 7;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Текст и кнопка
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Текст и кнопка
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Компоненты
			// Текст
			Label LOrderText = new Label();
			Styles.TextStyle(LOrderText, 12);
			LOrderText.Text = "Заказы";

			// Текст
			Label LIncText = new Label();
			Styles.TextStyle(LIncText);
			LIncText.Anchor = AnchorStyles.Left;
			LIncText.Text = "Ваши заказы";

			// Текст
			Label LOutText = new Label();
			Styles.TextStyle(LOutText);
			LOutText.Anchor = AnchorStyles.Left;
			LOutText.Text = "Входящие заказы";

			// Кнопка перехода на
			// форму исходящих заказов
			BIncOrders = new Button();
			Styles.ButtonStyle(BIncOrders);
			BIncOrders.Text = "Список";

			// Кнопка перехода на
			// форму входящих заказов
			BOutOrders = new Button();
			Styles.ButtonStyle(BOutOrders);
			BOutOrders.Text = "Список";

			Result.Controls.Add(LOrderText, 1, 1);
			Result.Controls.Add(LIncText, 1, 3);
			Result.Controls.Add(LOutText, 1, 5);
			Result.Controls.Add(BOutOrders, 2, 3);
			Result.Controls.Add(BIncOrders, 2, 5);

			#endregion

			return Result;

		}

		// Часть формы, что касается товаров организации
		private TableLayoutPanel CreateProductsFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			#region Рамка

			Result.ColumnCount = 4;
			Result.RowCount = 5;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Текст и кнопка
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Компоненты

			// Текст
			Label LProductText = new Label();
			Styles.TextStyle(LProductText, 12);
			LProductText.Text = "Товары";

			// Текст
			Label LYourProductsText = new Label();
			Styles.TextStyle(LYourProductsText);
			LYourProductsText.Anchor = AnchorStyles.Left;
			LYourProductsText.Text = "Ваши товары";

			// Кнопка перехода на
			// форму просмотра товаров
			BProducts = new Button();
			Styles.ButtonStyle(BProducts);
			BProducts.Text = "Список";

			Result.Controls.Add(LProductText, 1, 1);
			Result.Controls.Add(LYourProductsText, 1, 3);
			Result.Controls.Add(BProducts, 2, 3);

			#endregion

			return Result;

		}

		// Часть формы, что касается работников организации
		private TableLayoutPanel CreateWorkersListFrame()
		{

			TableLayoutPanel Result = CreateListFrame();

			#region Компоненты
			// Текст
			Label LWorkersListText = new Label();
			Styles.TextStyle(LWorkersListText, 12);
			LWorkersListText.Text = "Сотрудники организации";

			// Пагинатор пользователей
			PagUserList = new Paginator(Styles.PaginatorLabelCount);
			Styles.PaginatorStyle(PagUserList);
			PagUserList.Anchor = AnchorStyles.Right;

			// Список
			UserList = new UserIdBasedPanelList();
			UserList.Font = new Font("Helvetica", 8);
			// UserList.Font = Styles.TextFont;
			UserList.PanelsWidth = UserList.Width * 2;

			#region Кнопка отвязки пользователя

			TableLayoutPanel DetachButtonFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(DetachButtonFrame, TableLayoutPanelCellBorderStyle.None);

			DetachButtonFrame.ColumnCount = 2;
			DetachButtonFrame.RowCount = 1;

			// Промежуток
			DetachButtonFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Кнопка
			DetachButtonFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			DetachButtonFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			BDetach = new Button();
			Styles.ButtonStyle(BDetach);
			BDetach.Font = new Font("Helvetica", 9);
			BDetach.Text = "Отвязать";

			DetachButtonFrame.Controls.Add(BDetach, 1, 0);

			#endregion

			// Текст
			Label LAddEmailText = new Label();
			Styles.TextStyle(LAddEmailText, 12);
			LAddEmailText.Margin = new Padding(0, 0, 0, 5);
			LAddEmailText.Text = "Электронный адрес пользователя: ";

			#region Поле и кнопка привязки пользователя 

			TableLayoutPanel AddUserFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(AddUserFrame, TableLayoutPanelCellBorderStyle.None);

			AddUserFrame.ColumnCount = 3;
			AddUserFrame.RowCount = 1;

			// Поле ввода
			AddUserFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));

			// Кнопка
			AddUserFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			// Промежуток
			AddUserFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			AddUserFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			// Поле ввода адреса
			TbEmailAttach = new TextBox();
			Styles.TextBoxStyle(TbEmailAttach);

			// Кнопка
			BAttach = new Button();
			Styles.ButtonStyle(BAttach);
			BAttach.Text = "Привязать";
			BAttach.Margin = new Padding(5, 0, 0, 0);

			AddUserFrame.Controls.Add(TbEmailAttach, 0, 0);
			AddUserFrame.Controls.Add(BAttach, 1, 0);

			#endregion

			Result.Controls.Add(LWorkersListText, 1, 1);
			Result.Controls.Add(PagUserList, 1, 2);
			Result.Controls.Add(UserList, 1, 3);
			Result.Controls.Add(DetachButtonFrame, 1, 4);
			Result.Controls.Add(LAddEmailText, 1, 5);
			Result.Controls.Add(AddUserFrame, 1, 6);

			#endregion

			return Result;

		}

		#endregion

		#endregion

		#region Формы

		// Форма - основа
		private TableLayoutPanel CreateMainForm()
		{

			TableLayoutPanel Result = CreateMainFrame();

			// Кнопки перехода
			Header = new BackMainHeader();

			Styles.ContentFramesStyle(Header, BorderStyle);
			Header.BGoBack.Font = new Font("Helvetica", 8);
			Header.BGoMain.Font = new Font("Helvetica", 8);

			Result.Controls.Add(Header, 1, 1);

			return Result;

		}

		// Создание формы только для чтения
		private TableLayoutPanel CreateReadOnlyForm()
		{

			TableLayoutPanel Result = CreateMainForm();

			Result.Controls.Add(CreateTopPartContent(ReadOnlyOrgInfo()), 1, 3);

			RtbAbout = new RichTextBox();
			Styles.RichTextBoxStyle(RtbAbout);
			RtbAbout.ReadOnly = true;

			Result.Controls.Add(RtbAbout, 1, 5);

			return Result;

		}

		// Саоздание редактируемой формы
		private TableLayoutPanel CreateEditableForm()
		{

			TableLayoutPanel MainFrame = CreateMainForm();

			MainFrame.Controls.Add(CreateTopPartContent(EditableOrgInfo()), 1, 3);
			MainFrame.Controls.Add(CreateBottomPartContent(), 1, 5);

			TableLayoutPanel ScrollableView = CreateScrollableView(MainFrame);

			return ScrollableView;
		}

		#endregion

		#region Вывод данных

		// Заполнение формы только для чтения
		private void FillReadOnlyData()
		{

			PbOrgImage.BackgroundImage = Org.Img;
			LName.Text = Org.Name;
			LEmail.Text = "Эл.адрес: " + Org.Email;
			LPhone.Text = "Телефон: " + Org.Phone;
			LRegDate.Text = "Дата регистрации: " + Org.RegDate.ToShortDateString();
			RtbAbout.Text = Org.About;

		}

		// Заполнение данных, что меняются 
		private void OrgDataFill()
		{

			PbOrgImage.BackgroundImage = Org.Img;
			TbName.Text = Org.Name;
			LEmail.Text = "Эл.адрес: " + Org.Email;
			TbPhone.Text = Org.Phone;
			LRegDate.Text = "Дата регистрации: " + Org.RegDate.ToShortDateString();
			RtbAbout.Text = Org.About;

		}

		// Заполнение формы для редактирования
		private void FillEditableData()
		{

			OrgDataFill();

			#region Пользователи
			// Настройка вывода пользователей
			UserList.UserAddStrategy = AddUserToListStrategy;

			// Получение адаптера пользователей
			UsersAdapter = Org.GetUsers();

			// Настройка пагинатора пользователей
			PagUserList.PageCount = UsersAdapter.GetPageCount();

			RefreshUserPanel(1);

			// Событие при изменении страницы пагинатора
			PagUserList.PageChanged += delegate (Object s, EventArgs args) {
				RefreshUserPanel(PagUserList.ChoosedPage);
			};

			#endregion

		}


		#endregion

		#region Работники организации

		// Алгоритм вывода пользователей
		private UserPanel AddUserToListStrategy(UserPanel user, DataRow row)
		{

			user.UserId = Convert.ToInt32(row["id"]);
			user.LName.Text = row["name"].ToString();
			user.LEmail.Text = row["email"].ToString();

			return user;

		}

		// Обновление списка пользователей
		private void RefreshUserPanel(int page)
		{
			UserList.Controls.Clear();

			programForm.Conn.Open();

			UserList.DataSource = UsersAdapter.GetData(page);

			programForm.Conn.Close();
		}


		#region События

		#region Изменение данных

		// Сохранение изменений
		private void SaveButtonClick(Object s, EventArgs e)
		{

			string Name = TbName.Text.Trim();
			string Phone = TbPhone.Text.Trim();
			string About = RtbAbout.Text.Trim();
			Image Img = PbOrgImage.BackgroundImage;

			// Проверка значений
			if (!QueryUtils.CheckName(Name))
			{
				MessageBox.Show("Название должно содержать от 4 до 50 букв");
				return;
			}

			if (!QueryUtils.CheckPhone(Phone))
			{
				MessageBox.Show("Телефон имеет неправильный формат");
				return;
			}

			// Изменение
			if (Org.Update(Name, Phone, Img, About))
			{

				Org.GetData(Org.Id);
				OrgDataFill();

				MessageBox.Show("Информация была успешно обновлена");

			}

		}

		// Отмена несохраненных изменений
		private void CancelButtonClick(Object s, EventArgs e)
		{

			OrgDataFill();

		}

		// После выбора изображения, 
		// её перенос в компонент
		private void AfterImageChoosed(Object s, EventArgs e)
		{

			if (QueryUtils.CheckImageWeight(OiString.FileName, 4200000))
			{
				PbOrgImage.BackgroundImage = Image.FromFile(OiString.FileName);

				Org.ImgChanged = true;

			}
			else
				MessageBox.Show("Размер изображения не должен превышать 4 МБ");

		}

		#endregion

		#endregion

		#region Привязка и отвязка пользователей
		// Привязка пользователя
		private void AttachUserButtonClick(Object s, EventArgs e)
		{

			string Email = TbEmailAttach.Text.Trim();
			// Проверки
			// Является ли текст эл. адресом
			if (!QueryUtils.CheckEmail(Email))
			{
				MessageBox.Show("Введенный эл. адрес имеет неправильный формат");
				return;
			}


			MySqlCommand Query = programForm.Conn.CreateCommand();
			Query.CommandText =
				"select u.id as u_id, r.name as role_name, oc.organization_id as org_id " +
				"from " +
					"user u inner join user_role r on u.role_id = r.id " +
 					"left join organization_content oc on u.id = oc.user_id " +
				"where u.email = @email";

			Query.Parameters.Add("email", MySqlDbType.VarChar).Value = Email;

			int UserId;
			int? OrgId;
			string UserRole;

			programForm.Conn.Open();

			MySqlDataReader QueryReader = Query.ExecuteReader();

			try
			{
				// Существует ли пользователь
				if (QueryReader.Read())
				{

					UserId = QueryReader.GetInt32("u_id");

					if (QueryReader.IsDBNull(2))
						OrgId = null;
					else
						OrgId = QueryReader.GetInt32("org_id");

					UserRole = QueryReader.GetString("role_name");

				}
				else
				{
					MessageBox.Show("Пользователь не найден");
					return;
				}

			}
			finally
			{
				QueryReader.Close();
				programForm.Conn.Close();
			}
			// Не админ ли пользователь
			if (UserRole == "admin")
			{
				MessageBox.Show("Администраторы не могут быть привязаны к организациям");
				return;
			}
			// Не привязан ли уже пользователь 
			if (OrgId != null)
			{
				MessageBox.Show("Данный пользователь уже привязан к организации");
				return;
			}
			// Привязка
			Org.AttachUser(UserId);

			RefreshUserPanel(1);

			PagUserList.PageCount = UsersAdapter.GetPageCount();

		}

		// Отвязка пользователя
		private void DetachUserButtonClick(Object s, EventArgs e)
		{

			UserPanel SelectedUser = UserList.SelectedUser;

			// Выбрана ли панель
			if (SelectedUser == null)
			{
				MessageBox.Show("Вы не выбрали пользователя");
				return;
			}

			// Нельзя отвязать себя
			if (SelectedUser.UserId == programForm.User.Id)
			{
				MessageBox.Show("Вы не можете отвязать самого себя");
				return;
			}

			MySqlCommand Query = programForm.Conn.CreateCommand();
			Query.CommandText =
				"select user_id from organization_content where user_id = @u_id and organization_id = @org_id";

			Query.Parameters.Add("u_id", MySqlDbType.Int32).Value = SelectedUser.UserId;
			Query.Parameters.Add("org_id", MySqlDbType.Int32).Value = Org.Id;

			programForm.Conn.Open();

			MySqlDataReader QueryReader = Query.ExecuteReader();

			try
			{
				// Привязан ли пользователь к данной организации
				if (!QueryReader.HasRows)
				{
					MessageBox.Show("Данный пользователь не привязан к данной организации");
					return;
				}

			}
			finally
			{
				QueryReader.Close();
				programForm.Conn.Close();
			}
			// Количество привязанных пользователей > 1
			Query = programForm.Conn.CreateCommand();
			Query.CommandText =
				"select count(*) from organization_content where organization_id = @id";

			Query.Parameters.Add("id", MySqlDbType.Int32).Value = Org.Id;

			programForm.Conn.Open();

			try
			{

				if (Convert.ToInt32(Query.ExecuteScalar()) == 1)
				{
					MessageBox.Show("Организации не могут оставаться без привязанных пользователей");
					return;
				}


			}
			finally
			{
				QueryReader.Close();
				programForm.Conn.Close();
			}

			// Отвязка
			Org.DetachUser(UserList.SelectedUser.UserId);

			RefreshUserPanel(1);

			PagUserList.PageCount = UsersAdapter.GetPageCount();

		}

		#endregion

		// Общие для всех форм события					
		private void EventSetUp()
		{
			// Переход назад
			Header.BGoBack.Click += delegate (Object s, EventArgs args) {
				programForm.History.Pop().RegenerateOldForm();
			};

			// Переход на главн. форму
			Header.BGoMain.Click += delegate (Object s, EventArgs args) {
				new DynProductsBandForm().Generate(programForm);
			};

		}

		// События формы обновления
		private void EditableFormSetUp()
		{
			EventSetUp();
			// Изменение данных
			OiString.AfterImgLoad += AfterImageChoosed;
			BSave.Click += SaveButtonClick;
			BCancel.Click += CancelButtonClick;

			// Работа с адресами


			// Работа с пользователями
			BAttach.Click += AttachUserButtonClick;
			BDetach.Click += DetachUserButtonClick;

			// Переходы

			// Переход в личный кабинет пользователя 
			// из списка пользователей
			UserList.UserDoubleClick += delegate (Object s, EventArgs e) {
				UserPanel ChoosedUser = (UserPanel)s;
				// programForm.History.Push(this);
				// new DynUserForm().Generate(...)
				MessageBox.Show("Переход к личному кабинету пользователя " + ChoosedUser.UserId);
			};

			// Переход к списку исходящих заказов организации
			BOutOrders.Click += delegate (Object s, EventArgs e) {
				// programForm.History.Push(this);
				// new DynOrdersForm().Generate(...)
				MessageBox.Show("Переход к списку исходящих заказов организации");
			};

			// Переход к списку исходящих заказов организации
			BIncOrders.Click += delegate (Object s, EventArgs e) {
				// programForm.History.Push(this);
				// new DynOrdersForm().Generate(...)
				MessageBox.Show("Переход к списку входящих заказов организации");
			};


			BProducts.Click += delegate (Object s, EventArgs e) {
				DynProductsBandForm PBForm = new DynProductsBandForm();
				PBForm.Generate(programForm);
				PBForm.PbAdapter.FilterString = Org.Name;
				PBForm.TbProductSearch.Text = Org.Name;
				PBForm.PageSelector.PageCount = PBForm.PbAdapter.GetPageCount();
				PBForm.RefreshBand(1);
			};
		}



	}
	#endregion
}

#endregion
