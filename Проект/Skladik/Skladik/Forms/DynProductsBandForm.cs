using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;

using Skladik.Utils;
using Skladik.NewComponents;
using Skladik.Adapters.BandAdapters;

namespace Skladik.Forms
{
	public class DynProductsBandForm : DynForm
	{

		const TableLayoutPanelCellBorderStyle BorderStyle = TableLayoutPanelCellBorderStyle.Inset;

		#region Объяление
		// Категории
		public IdBasedPanelList Categories { get; private set; }
		public Button BCancel { get; private set; }

		// Панель аккаунта
		public Button BExit { get; private set; }
		public Label LName { get; private set; }
		public Label LOrgName { get; private set; }
		public PictureBox PbAvatar { get; private set; }

		//Заголовок ленты
		public TextBox TbProductSearch { get; private set; }
		public Button BProductSearch { get; private set; }
		public Button BCancelSearch { get; private set; }
		public ComboBox CbAddedOn { get; private set; }
		public ComboBox CbPrice { get; private set; }
		public Button BAgree { get; private set; }
		public Paginator PageSelector { get; private set; }

		// Лента товаров
		public ProductBand Band { get; private set; }
		public ProductBandAdapter PbAdapter { get; private set; }

		// Админская панель пользователей
		public Paginator UsersPaginator { get; private set; }
		public TextBox TbUserSearch { get; private set; }
		public Button BUserSearch { get; private set; }
		public UserIdBasedPanelList UserList { get; private set; }

		// Админская панель организаций
		public Paginator OrgPaginator { get; private set; }
		public TextBox TbOrgSearch { get; private set; }
		public Button BOrgSearch { get; private set; }
		public OrganizationIdBasedPanelList OrgList { get; private set; }

		// Добавление товара
		public Button BAddProduct { get; private set; }

		// Переход на заказы
		public Button BOutgoingOrders { get; private set; }
		public Button BIncomingOrders { get; private set; }

		// Количество важных заказов
		public Label ImportantIncoming { get; private set; }
		public Label ImportantOutgoing { get; private set; }

		// Адаптеры панели
		public FilterableBandAdapter UsersAdapter { get; private set; }
		public FilterableBandAdapter OrganizationsAdapter { get; private set; }
		#endregion
		// Настройка формы
		protected override void SetUpMainForm()
		{
			programForm.Controls.Clear();
			Size FormSize = new Size(1350, 600);
			programForm.MinimumSize = FormSize;
			programForm.MaximumSize = new Size(1500, 800); ;
			programForm.Location = new Point(10, 100);
			programForm.Size = FormSize;
			programForm.Text = "Просмотр товаров";
			programForm.History.Clear();
		}

		// Генерация формы
		public override void Generate(Form1 aForm)
		{

			base.Generate(aForm);

			// Определение формы
			if (programForm.User.Role == "admin")
			{

				// Настройка адаптеров
				PrepareUserAdapter();
				PrepareOrgAdapter();
				// Вывод формы админа
				formContent = GenerateAdminForm(QueryUtils.TrimToLength(programForm.User.Name, 26));

				UsersPaginator.PageChanged += delegate (Object s, EventArgs args) {
					RefreshUserPanel(UsersPaginator.ChoosedPage);
				};

				OrgPaginator.PageChanged += delegate (Object s, EventArgs args) {
					RefreshOrgPanel(OrgPaginator.ChoosedPage);
				};

				// Переходы админской панели
				// Переход на личный кабинет пользователя
				UserList.UserSelected += UserChoosed;

				// Переход на личный кабинет организации
				OrgList.OrgSelected += OrgChoosed;

				// Настройка поиска ползователей
				UsersAdapter.SearchStrategy = UserAndOrgSearchStrategy;
				BUserSearch.Click += UserSearchButtonClick;

				// Настройка поиска организаций
				OrganizationsAdapter.SearchStrategy = UserAndOrgSearchStrategy;
				BOrgSearch.Click += OrgSearchButtonClick;

				// Вывод привязанного пользователя
			}
			else if (programForm.User.Organization != null)
			{

				formContent = GenerateAttachedUserForm(
					QueryUtils.TrimToLength(programForm.User.Name, 19),
					QueryUtils.TrimToLength(programForm.User.Organization.Name, 17),
					programForm.User.Organization.Img
				);

				// Переход на личный кабинет организации
				LOrgName.Click += OrgAccountClicked;
				PbAvatar.Click += OrgAccountClicked;

				// Добавление товара
				BAddProduct.Click += AddProductButtonClick;

				// Формы заказов
				BOutgoingOrders.Click += OutgoingOrderButtonClick;


				// Вывод непривязанного пользователя
			}
			else
				formContent = GenerateDetachedUserForm(QueryUtils.TrimToLength(programForm.User.Name, 26));

			// Заполнение категорий 
			FillCategories();

			Categories.PanelChoosed += CategoryChanged;
			BCancel.Click += CancelCategoryFilter;


			// Лента товаров
			PrepareBandAdapter();
			// Передача алгоритма обработки данных из источника
			Band.ElementCreationStrategy += AddProductStrategy;

			// Настройка пагинатора товаров 
			PageSelector.PageCount = PbAdapter.GetPageCount();

			PageSelector.PageChanged += delegate (Object s, EventArgs args) {
				RefreshBand(PageSelector.ChoosedPage);
			};

			// Выгрузка данных 1 страницы
			RefreshBand(1);

			// Элементы поиска товаров
			BProductSearch.Click += SearchButtonClick;
			BCancelSearch.Click += SearchCancelButtonClick;

			// Подтверждение сортировки
			BAgree.Click += SortAgreeButtonClick;

			// Выход из системы
			BExit.Click += ExitButtonClick;
			// Переход на личный кабинет пользователя
			LName.Click += UserNameClicked;

			Band.ProductChoosed += BandProductChoosed;

			programForm.Controls.Add(formContent);

		}



		#region Рамки
		// Рамка формы пользователя
		private TableLayoutPanel CreateUserFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			Result.CellBorderStyle = BorderStyle;

			Result.Dock = DockStyle.Fill;

			Result.ColumnCount = 5;
			Result.RowCount = 3;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Styles.CategoriesWidth));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));

			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			return Result;
		}

		// Рамка формы администратора
		private TableLayoutPanel CreateAdminFrame()
		{

			TableLayoutPanel Temp = CreateUserFrame();

			Temp.CellBorderStyle = BorderStyle;

			Temp.ColumnCount = 7;

			Temp.ColumnStyles[3] = new ColumnStyle(SizeType.Percent, 80);
			Temp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
			Temp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));

			return Temp;

		}

		// Рамка категорий для неприкрепленных пользователей	
		private TableLayoutPanel CreateDetachedCategoryFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			Result.CellBorderStyle = BorderStyle;

			Result.Margin = new Padding(0, 0, 0, 0);
			Result.Dock = DockStyle.Fill;

			Result.ColumnCount = 1;
			Result.RowCount = 3;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.AccountPanelHeight));

			return Result;
		}

		// Рамка категорий для привязанного пользователя	
		private TableLayoutPanel CreateAttachedCategoryFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			Result.CellBorderStyle = BorderStyle;

			Result.Margin = new Padding(0, 0, 0, 0);
			Result.Dock = DockStyle.Fill;

			Result.ColumnCount = 1;
			Result.RowCount = 7;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.AccountPanelHeight));

			return Result;

		}

		// Рамка для ленты
		private TableLayoutPanel CreateBandFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			Result.CellBorderStyle = BorderStyle;
			Result.Dock = DockStyle.Fill;
			Result.Margin = new Padding(0);

			Result.ColumnCount = 1;
			Result.RowCount = 2;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.ProductsPanelUpperHeight));
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));


			return Result;

		}

		// Рамка админской панели
		private TableLayoutPanel CreateAdminPanelFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			Result.CellBorderStyle = BorderStyle;
			Result.Dock = DockStyle.Fill;
			Result.Margin = new Padding(0);

			Result.ColumnCount = 1;
			Result.RowCount = 3;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));


			// Панель пользователей
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Панель оранизаций
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

			return Result;

		}

		#endregion


		#region Части формы
		// Вывод списка категорий
		private TableLayoutPanel CreateCategoryList()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			Result.Dock = DockStyle.Fill;
			Result.Margin = new Padding(0, 0, 0, 0);

			Result.CellBorderStyle = BorderStyle;

			#region Свойства элементов
			// Заголовок	
			Panel TitlePanel = new Panel();
			TitlePanel.AutoSize = true;

			Label LTitle = new Label();
			LTitle.Text = "Категории";
			LTitle.AutoSize = true;
			LTitle.Font = new Font("Helvetica", 20);
			LTitle.Margin = new Padding(5, 5, 5, 5);
			LTitle.Location = new Point(5, 5);

			TitlePanel.Controls.Add(LTitle);

			// Список категорий
			Categories = new IdBasedPanelList();
			Categories.Font = Styles.TextFont;
			/*
													// Добавление категорий
			Categories.AddAPanel(1, "Мясо");
			Categories.AddAPanel(2, "Овощи");
			Categories.AddAPanel(1, "Мясо");
			*/
			// Кнопка отмены
			BCancel = new Button();

			BCancel.Text = "Отменить";
			BCancel.Margin = new Padding(2, 2, 15, 2);
			BCancel.Size = new Size(80, 25);
			BCancel.Dock = DockStyle.Right;

			#endregion

			#region Таблица

			Result.ColumnCount = 1;
			Result.RowCount = 4;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Styles.CategoriesWidth));

			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			Result.Controls.Add(LTitle, 0, 0);
			Result.Controls.Add(Categories, 0, 2);
			Result.Controls.Add(BCancel, 0, 3);

			#endregion

			return Result;
		}

		// Панель личного кабинета 
		// непривязанного пользователя
		private Panel CreateDetachedAccountPanel(string name)
		{

			// Панель
			Panel Result = new Panel();
			Result.Dock = DockStyle.Fill;

			// Имя пользователя
			LName = new Label();
			LName.Text = name;
			LName.Location = new Point(20, 20);
			Styles.TextStyle(LName);

			// Кнопка выхода
			BExit = new Button();
			BExit.Text = "Выйти";
			BExit.Font = Styles.TextFont;
			BExit.Size = new Size(80, 25);
			BExit.Location = new Point(140, 55);

			// Добавление на панель
			Result.Controls.Add(LName);
			Result.Controls.Add(BExit);

			return Result;

		}

		// Панель личного кабинета 
		// привязанного пользователя
		private Panel CreateAtachedAccountPanel(string name, string orgName, Image img)
		{

			Panel Result = new Panel();
			Result.Dock = DockStyle.Fill;

			// Имя пользователя
			LName = new Label();
			LName.Text = name;
			LName.Location = new Point(80, 25);
			Styles.TextStyle(LName);

			LOrgName = new Label();
			LOrgName.Text = orgName;
			LOrgName.Location = new Point(80, 5);
			LOrgName.AutoSize = true;
			LOrgName.Font = new Font("Helvetica", 12);

			// Кнопка выхода
			BExit = new Button();
			BExit.Text = "Выйти";
			BExit.Font = Styles.TextFont;
			BExit.Size = new Size(80, 25);
			BExit.Location = new Point(140, 55);

			// Аватар
			PbAvatar = new PictureBox();
			PbAvatar.BackgroundImage = img;
			PbAvatar.BackgroundImageLayout = ImageLayout.Zoom;
			PbAvatar.Location = new Point(10, 10);
			PbAvatar.Size = new Size(Styles.AccountPanelHeight - 30, Styles.AccountPanelHeight - 30);

			// Добавление на панель
			Result.Controls.Add(LOrgName);
			Result.Controls.Add(PbAvatar);
			Result.Controls.Add(LName);
			Result.Controls.Add(BExit);

			return Result;

		}

		// Заголовок ленты
		private TableLayoutPanel CreateBandHeader()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			#region Рамка
			Result.Dock = DockStyle.Fill;
			Result.Margin = new Padding(0);
			Result.CellBorderStyle = BorderStyle;

			Result.ColumnCount = 8;
			Result.RowCount = 2;

			// Пустой промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 2));

			// Поле
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

			// Кнопка
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));

			// Кнопка
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));

			// Текст
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

			// Комбобоксы
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

			// Кнопка
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));

			// Пагинатор
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

			#endregion

			#region Компоненты
			// Поле
			TbProductSearch = new TextBox();
			Styles.TextBoxStyle(TbProductSearch);

			// Кнопка
			BProductSearch = new Button();
			BProductSearch.Text = "Поиск";
			BProductSearch.Font = Styles.TextFont;
			BProductSearch.Dock = DockStyle.Fill;
			BProductSearch.Margin = new Padding(5, 1, 5, 1);

			// Кнопка
			BCancelSearch = new Button();
			BCancelSearch.Text = "Отмена";
			BCancelSearch.Font = Styles.TextFont;
			BCancelSearch.Dock = DockStyle.Fill;
			BCancelSearch.Margin = new Padding(5, 1, 5, 1);

			// Текст
			Label LAddedOn = new Label();
			LAddedOn.Text = "Дата доб.";
			LAddedOn.Anchor = AnchorStyles.None;
			Styles.TextStyle(LAddedOn);

			Label LPrice = new Label();
			LPrice.Text = "Цена";
			LPrice.Anchor = AnchorStyles.Right;
			Styles.TextStyle(LPrice);

			// Комбобоксы
			CbAddedOn = new ComboBox();
			CbAddedOn.Font = Styles.TextFont;
			CbAddedOn.Items.Add("по умолчанию");
			CbAddedOn.Items.Add("по возрастанию");
			CbAddedOn.Items.Add("по убыванию");
			CbAddedOn.Dock = DockStyle.Fill;
			CbAddedOn.SelectedIndex = 0;

			CbPrice = new ComboBox();
			CbPrice.Font = Styles.TextFont;
			CbPrice.Items.Add("по умолчанию");
			CbPrice.Items.Add("по возрастанию");
			CbPrice.Items.Add("по убыванию");
			CbPrice.Dock = DockStyle.Fill;
			CbPrice.SelectedIndex = 0;

			BAgree = new Button();
			BAgree.Text = "Прим.";
			BAgree.Font = Styles.TextFont;
			BAgree.Dock = DockStyle.Fill;
			BAgree.Margin = new Padding(5, 1, 5, 1);

			// ### Изменить начальное значение количества страниц ###
			PageSelector = new Paginator(7);
			Styles.PaginatorStyle(PageSelector);

			#endregion

			Result.Controls.Add(TbProductSearch, 1, 1);
			Result.Controls.Add(BProductSearch, 2, 1);
			Result.Controls.Add(BCancelSearch, 3, 1);
			Result.Controls.Add(LAddedOn, 4, 1);
			Result.Controls.Add(LPrice, 4, 0);
			Result.Controls.Add(CbAddedOn, 5, 1);
			Result.Controls.Add(CbPrice, 5, 0);
			Result.Controls.Add(BAgree, 6, 1);
			Result.Controls.Add(PageSelector, 7, 1);

			return Result;

		}

		// Создание ленты товаров
		private TableLayoutPanel CreateProductBand()
		{

			// Рамка для ленты
			TableLayoutPanel BandFrame = CreateBandFrame();

			Band = new ProductBand();
			Band.Dock = DockStyle.Fill;

			BandFrame.Controls.Add(CreateBandHeader(), 0, 0);
			BandFrame.Controls.Add(Band, 0, 1);

			return BandFrame;

		}

		// Создание админской панели пользователя
		private TableLayoutPanel CreateAdminUserPanel()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			#region Рамка
			Result.CellBorderStyle = BorderStyle;
			Result.Dock = DockStyle.Fill;
			Result.Margin = new Padding(0);

			Result.ColumnCount = 1;
			Result.RowCount = 3;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Заголовок
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.AdminPanelHeaderHeight));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Список
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			#endregion

			#region Заголовок
			TableLayoutPanel Header = new TableLayoutPanel();

			Header.ColumnCount = 2;
			Header.RowCount = 2;

			Header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
			Header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

			Header.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
			Header.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

			Label LTitle = new Label();
			LTitle.Text = "Пользователи";
			Styles.TextStyle(LTitle);
			LTitle.Anchor = AnchorStyles.None;

			UsersPaginator = new Paginator(4);
			Styles.PaginatorStyle(UsersPaginator);
			UsersPaginator.Font = new Font("Helvetica", 8);

			TbUserSearch = new TextBox();
			Styles.TextBoxStyle(TbUserSearch);

			BUserSearch = new Button();
			BUserSearch.Text = "Поиск";
			BUserSearch.Font = Styles.TextFont;

			Header.Controls.Add(LTitle, 0, 0);
			Header.Controls.Add(UsersPaginator, 1, 0);
			Header.Controls.Add(TbUserSearch, 0, 1);
			Header.Controls.Add(BUserSearch, 1, 1);
			#endregion

			// Список пользователей
			UserList = new UserIdBasedPanelList();
			UserList.Font = new Font("Helvetica", 8);
			UserList.UserAddStrategy = AddUserToListStrategy;

			UsersPaginator.PageCount = UsersAdapter.GetPageCount();

			// Вывод 1 страницы пользователей
			RefreshUserPanel(1);

			Result.Controls.Add(Header, 0, 0);
			Result.Controls.Add(UserList, 0, 2);

			return Result;

		}

		// Создание админской панели организации
		private TableLayoutPanel CreateAdminOrganizationPanel()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			#region Рамка
			Result.CellBorderStyle = BorderStyle;
			Result.Dock = DockStyle.Fill;
			Result.Margin = new Padding(0);

			Result.ColumnCount = 1;
			Result.RowCount = 3;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Заголовок
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.AdminPanelHeaderHeight));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Список
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Заголовок
			TableLayoutPanel Header = new TableLayoutPanel();

			Header.ColumnCount = 2;
			Header.RowCount = 2;

			Header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
			Header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

			Header.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
			Header.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

			Label LTitle = new Label();
			LTitle.Text = "Организации";
			Styles.TextStyle(LTitle);
			LTitle.Anchor = AnchorStyles.None;

			OrgPaginator = new Paginator(4);
			Styles.PaginatorStyle(OrgPaginator);
			OrgPaginator.Font = new Font("Helvetica", 8);

			TbOrgSearch = new TextBox();
			Styles.TextBoxStyle(TbOrgSearch);

			BOrgSearch = new Button();
			BOrgSearch.Text = "Поиск";
			BOrgSearch.Font = Styles.TextFont;

			Header.Controls.Add(LTitle, 0, 0);
			Header.Controls.Add(OrgPaginator, 1, 0);
			Header.Controls.Add(TbOrgSearch, 0, 1);
			Header.Controls.Add(BOrgSearch, 1, 1);
			#endregion

			OrgList = new OrganizationIdBasedPanelList();
			OrgList.Font = Styles.TextFont;
			OrgList.OrgAddStrategy += AddOrgToListStrategy;

			OrgPaginator.PageCount = OrganizationsAdapter.GetPageCount();

			RefreshOrgPanel(1);

			Result.Controls.Add(Header, 0, 0);
			Result.Controls.Add(OrgList, 0, 2);

			return Result;

		}

		// Создание админской панели
		private TableLayoutPanel CreateAdminPanel()
		{

			TableLayoutPanel Result = CreateAdminPanelFrame();

			Result.Controls.Add(CreateAdminUserPanel(), 0, 0);

			Result.Controls.Add(CreateAdminOrganizationPanel(), 0, 2);

			return Result;

		}


		#endregion


		#region Формы
		// Генерация формы администатора
		private TableLayoutPanel GenerateAdminForm(string name)
		{
			// Создать рамку для админа
			TableLayoutPanel FormFrame = CreateAdminFrame();

			// Создать категории неривязанного
			TableLayoutPanel CategoryFrame = CreateDetachedCategoryFrame();

			CategoryFrame.Controls.Add(CreateCategoryList(), 0, 0);
			CategoryFrame.Controls.Add(CreateDetachedAccountPanel(name), 0, 2);

			FormFrame.Controls.Add(CategoryFrame, 1, 1);

			// Создать ленту
			FormFrame.Controls.Add(CreateProductBand(), 3, 1);

			// Создать админскую панель
			FormFrame.Controls.Add(CreateAdminPanel(), 5, 1);

			return FormFrame;
		}

		// Генерация формы привязанного пользователя
		private TableLayoutPanel GenerateAttachedUserForm(string name, string orgName, Image img)
		{
			// Создать рамку для пользователя
			TableLayoutPanel FormFrame = CreateUserFrame();
			// Создать категориии привязанного
			TableLayoutPanel CategoryFrame = CreateAttachedCategoryFrame();
			// Категории
			CategoryFrame.Controls.Add(CreateCategoryList(), 0, 0);

			// Добавить товар
			BAddProduct = new Button();
			BAddProduct.Text = "Добавить товар";
			BAddProduct.Dock = DockStyle.Fill;
			BAddProduct.Margin = new Padding(20, 0, 20, 0);

			CategoryFrame.Controls.Add(BAddProduct, 0, 2);

			#region Таблица кнопок заказов
			TableLayoutPanel Orders = new TableLayoutPanel();

			Orders.Dock = DockStyle.Fill;
			Orders.CellBorderStyle = BorderStyle;
			Orders.Margin = new Padding(10, 0, 10, 0);

			Orders.ColumnCount = 2;
			Orders.RowCount = 2;

			Orders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
			Orders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));


			Orders.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
			Orders.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
			#endregion

			#region Кнопки заказов
			BOutgoingOrders = new Button();
			//BIncomingOrders = new Button();

			BOutgoingOrders.Text = "Исхоящие заказы";
			//BIncomingOrders.Text = "Входящие заказы";

			BOutgoingOrders.Dock = DockStyle.Fill;
			//BIncomingOrders.Dock = DockStyle.Fill;

			Orders.Controls.Add(BOutgoingOrders, 0, 0);
			//Orders.Controls.Add(BIncomingOrders, 0, 1);
			#endregion

			#region Количество важных заказов
			//ImportantIncoming = new Label();
			ImportantOutgoing = new Label();

			//ImportantIncoming.Text = "0";
			ImportantOutgoing.Text = "0";

			//Styles.TextStyle(ImportantIncoming);
			Styles.TextStyle(ImportantOutgoing);

			//ImportantIncoming.Anchor = AnchorStyles.None;
			ImportantOutgoing.Anchor = AnchorStyles.None;

			Orders.Controls.Add(ImportantOutgoing, 1, 0);
			//Orders.Controls.Add(ImportantIncoming, 1, 1);

			#endregion

			CategoryFrame.Controls.Add(Orders, 0, 4);
			CategoryFrame.Controls.Add(CreateAtachedAccountPanel(name, orgName, img), 0, 6);

			FormFrame.Controls.Add(CategoryFrame, 1, 1);

			// Создать ленту
			FormFrame.Controls.Add(CreateProductBand(), 3, 1);

			return FormFrame;
		}

		// Генерация формы непривязанного пользователя
		private TableLayoutPanel GenerateDetachedUserForm(string name)
		{
			// Создать рамку для пользователя
			TableLayoutPanel FormFrame = CreateUserFrame();
			// Создать категориии непривязанного
			TableLayoutPanel CategoryFrame = CreateDetachedCategoryFrame();

			CategoryFrame.Controls.Add(CreateCategoryList(), 0, 0);
			CategoryFrame.Controls.Add(CreateDetachedAccountPanel(name), 0, 2);

			FormFrame.Controls.Add(CategoryFrame, 1, 1);

			// Создать ленту
			FormFrame.Controls.Add(CreateProductBand(), 3, 1);

			return FormFrame;
		}

		#endregion


		#region Вывод данных
		// Заполнение категорий
		private void FillCategories()
		{
			// Получение данных
			MySqlCommand Query = programForm.Conn.CreateCommand();
			Query.CommandText = "select * from category";

			programForm.Conn.Open();

			MySqlDataReader QueryReader = Query.ExecuteReader();

			// Выгрузка в компонент категорий
			while (QueryReader.Read())
				Categories.AddAPanel(QueryReader.GetInt32("id"), QueryReader.GetString("name"));

			QueryReader.Close();

			programForm.Conn.Close();

		}

		#region Панель пользователей
		// Алгоритм вывода информации на панель
		// пользователя из источника
		private UserPanel AddUserToListStrategy(UserPanel user, DataRow row)
		{

			user.UserId = Convert.ToInt32(row["id"]);
			user.LName.Text = row["name"].ToString();
			user.LEmail.Text = row["email"].ToString();

			return user;

		}

		// Настройка адаптер панели пользователей
		private void PrepareOrgAdapter()
		{
			// Настройка адаптера
			MySqlCommand SelectCommand = programForm.Conn.CreateCommand();
			SelectCommand.CommandText = "select * from user";

			MySqlCommand SelectCount = programForm.Conn.CreateCommand();
			SelectCount.CommandText = "select count(*) from user";


			UsersAdapter = new FilterableBandAdapter(SelectCommand, SelectCount, Styles.AdminPanelListsElemCount, programForm.Conn);

		}

		// Обновление списка пользователей
		private void RefreshUserPanel(int page)
		{
			UserList.Controls.Clear();

			programForm.Conn.Open();

			UserList.DataSource = UsersAdapter.GetData(page);

			programForm.Conn.Close();
		}


		#endregion

		#region Панель организаций
		// Алгоритм вывода информации на панель
		// организации из источника
		private OrgPanel AddOrgToListStrategy(OrgPanel org, DataRow row)
		{

			org.OrgId = Convert.ToInt32(row["id"]);
			org.LName.Text = row["name"].ToString();
			org.PbImage.BackgroundImage = QueryUtils.GetImageFromByteArray((byte[])row["avatar"]);

			return org;

		}

		// Настройка адаптера панели организации
		private void PrepareUserAdapter()
		{

			MySqlCommand SelectCommand = programForm.Conn.CreateCommand();
			SelectCommand.CommandText = "select * from organization";

			MySqlCommand SelectCount = programForm.Conn.CreateCommand();
			SelectCount.CommandText = "select count(*) from organization";


			OrganizationsAdapter = new FilterableBandAdapter(SelectCommand, SelectCount, Styles.AdminPanelListsElemCount, programForm.Conn);

		}

		// Обновление списка организаций
		private void RefreshOrgPanel(int page)
		{
			OrgList.Controls.Clear();

			programForm.Conn.Open();

			OrgList.DataSource = OrganizationsAdapter.GetData(page);

			programForm.Conn.Close();
		}

		#endregion

		#region Лента товаров	
		// Алгоритм добавления информации в карточку
		// товара
		private BandElement AddProductStrategy(BandElement elem, DataRow row)
		{

			elem.ProductId = Convert.ToInt32(row["id"]);
			elem.PbImage.BackgroundImage = QueryUtils.GetImageFromByteArray((byte[])row["img"]);
			elem.MeasureUnit = row["m_unit"].ToString();
			elem.LName.Text = row["prod_name"].ToString();
			elem.LPrice.Text = row["price"].ToString() + " руб. ~ 1 " + elem.MeasureUnit;
			elem.LCount.Text = "В наличии: " + row["quant"].ToString() + " " + elem.MeasureUnit;
			elem.LAddDate.Text = ((DateTime)row["added_on"]).ToShortDateString();


			return elem;
		}



		// Настройка адаптера для ленты
		private void PrepareBandAdapter()
		{

			MySqlCommand SelectQuery = programForm.Conn.CreateCommand();
			SelectQuery.CommandText =
				"select p.id, b.quantity as quant, p.name as prod_name, p.img, p.added_on, p.price, m.name as m_unit " +
				"from product p, measure_unit m, balance b " +
				"where p.measure_unit_id = m.id " +
				"and p.id = b.product_id ";

			// ты забыл про связь с организацией у меня нет seller id, а точно, ты же продаешь, Хм

			MySqlCommand SelectCount = programForm.Conn.CreateCommand();
			//SelectCount.CommandText = "select count(*) from product";

			SelectCount.CommandText =
				"select count(*) " +
				"from product p, organization s, measure_unit m, balance b " +
				"where p.measure_unit_id = m.id " +
				"and p.id = b.product_id ";

			PbAdapter = new ProductBandAdapter(SelectQuery, SelectCount, Styles.ProductPanelElemCount, programForm.Conn);

			PbAdapter.SearchStrategy = BandSearchStrategy;

		}

		// Обновление списка товаров
		private void RefreshBand(int page)
		{
			Band.Controls.Clear();

			programForm.Conn.Open();

			Band.DataSource = PbAdapter.GetData(page);

			programForm.Conn.Close();
		}

		#endregion

		#endregion


		#region Переходы по формам

		// Выход из системы
		private void ExitButtonClick(Object s, EventArgs e)
		{
			MessageBox.Show("Выход из системы");
		}

		// Переход на личный кабинет пользователя
		private void UserNameClicked(Object s, EventArgs e)
		{
			MessageBox.Show("Переход на личный кабинет пользователя");
		}

		// Переход на личный кабинет организации
		private void OrgAccountClicked(Object s, EventArgs e)
		{
			MessageBox.Show("Переход на личный кабинет организации");
		}

		// Переход на форму просмотра товара
		private void BandProductChoosed(Object s, EventArgs e)
		{
			BandElement Product = (BandElement)s;
			//MessageBox.Show("Переход на форму просмотра товара с id - имя = " + Product.ProductId.ToString() + " - " + Product.LName.Text);
			new DynProductForm().Generate(programForm, Product.ProductId);
			programForm.History.Push(this);
		}

		// Переход на форму личного кабинета
		// выбранного в панели администратора пользователя
		private void UserChoosed(Object s, EventArgs e)
		{
			UserPanel ChoosedUser = (UserPanel)s;
			MessageBox.Show("Переход на форму личного кабинета пользователя с id - имя = " + ChoosedUser.UserId.ToString() + " - " + ChoosedUser.LName.Text);
		}

		// Переход на форму личного кабинета
		// выбранного в панели администратора организации
		private void OrgChoosed(Object s, EventArgs e)
		{
			OrgPanel ChoosedOrg = (OrgPanel)s;
			MessageBox.Show("Переход на форму личного кабинета организации с id - имя = " + ChoosedOrg.OrgId.ToString() + " - " + ChoosedOrg.LName.Text);
		}

		// Переход на форму добавления товара
		private void AddProductButtonClick(Object s, EventArgs e)
		{
			new DynProductForm().Generate(programForm);
			programForm.History.Push(this);
			//MessageBox.Show("Переход на форму добавления товара");
		}

		// Переход на форму исходящих заказов
		private void OutgoingOrderButtonClick(Object s, EventArgs e)
		{
			MessageBox.Show("Переход на форму исходящих заказов");
		}

		// Переход на форму входящих заказов
		private void IncomingOrderButtonClick(Object s, EventArgs e)
		{
			MessageBox.Show("Переход на форму входящих заказов");
		}

		#endregion


		#region Поиск, фильтрация по категориям и сортировка ленты товаров

		#region Поиск товаров

		// Алгоритм поиска товаров
		private MySqlCommand BandSearchStrategy(MySqlCommand query, string filterString)
		{

			string CommandText =
				"p.name like @search";

			query.Parameters.Add("search", MySqlDbType.VarChar).Value = "%" + filterString + "%";

			if (!query.CommandText.Contains("where"))
				query.CommandText += " where " + CommandText;
			else
				query.CommandText += " and (" + CommandText + ")";

			return query;

		}

		// Кнопка поиска
		private void SearchButtonClick(Object s, EventArgs e)
		{

			string SearchString = TbProductSearch.Text.Trim();

			if (SearchString != "" && SearchString != PbAdapter.FilterString)
			{
				PbAdapter.FilterString = SearchString;
				PageSelector.PageCount = PbAdapter.GetPageCount();
				RefreshBand(1);
			}
		}

		// Кнопка отмены поиска
		private void SearchCancelButtonClick(Object s, EventArgs e)
		{

			PbAdapter.ResetFilter();
			PageSelector.PageCount = PbAdapter.GetPageCount();
			RefreshBand(1);

		}

		#endregion

		#region Фильтрация по категориям

		// При выборе категории
		private void CategoryChanged(Object s, EventArgs e)
		{

			IdBasedPanelList SenderList = (IdBasedPanelList)s;

			PbAdapter.CategoryFilterId = SenderList.ChoosedPanel.ElementId;

			PageSelector.PageCount = PbAdapter.GetPageCount();
			RefreshBand(1);

		}

		// Сброс категории
		private void CancelCategoryFilter(Object s, EventArgs e)
		{

			PbAdapter.ResetCategory();

			Categories.Reset();

			PageSelector.PageCount = PbAdapter.GetPageCount();
			RefreshBand(1);

		}

		#endregion

		#region Сортировка ленты

		private void SortAgreeButtonClick(Object s, EventArgs e)
		{

			// Определение сортировки по цене
			if (CbPrice.Items[CbPrice.SelectedIndex].ToString() == "по умолчанию")
				PbAdapter.SortByPrice = ProductsSortType.Default;

			else if (CbPrice.Items[CbPrice.SelectedIndex].ToString() == "по возрастанию")
				PbAdapter.SortByPrice = ProductsSortType.Asc;

			else
				PbAdapter.SortByPrice = ProductsSortType.Desc;

			// Определение сортировки по дате добавления
			if (CbAddedOn.Items[CbAddedOn.SelectedIndex].ToString() == "по умолчанию")
				PbAdapter.SortByAddDate = ProductsSortType.Default;

			else if (CbAddedOn.Items[CbAddedOn.SelectedIndex].ToString() == "по возрастанию")
				PbAdapter.SortByAddDate = ProductsSortType.Asc;

			else
				PbAdapter.SortByAddDate = ProductsSortType.Desc;

			RefreshBand(PageSelector.ChoosedPage);

		}

		#endregion

		#endregion


		#region Панель админа - поиск пользователей и поиск организаций

		// Алгоритм поиска пользователей и организаций
		private MySqlCommand UserAndOrgSearchStrategy(MySqlCommand query, string filterString)
		{

			string CommandText =
				"name like @search or " +
				"email like @search ";

			query.Parameters.Add("search", MySqlDbType.VarChar).Value = "%" + filterString + "%";

			if (!query.CommandText.Contains("where"))
				query.CommandText += " where " + CommandText;
			else
				query.CommandText += " and (" + CommandText + ")";


			return query;

		}

		// Кнопка поиска панели пользователей
		private void UserSearchButtonClick(Object s, EventArgs e)
		{

			string SearchString = TbUserSearch.Text.Trim();

			if (SearchString != UsersAdapter.FilterString)
			{

				if (SearchString != "")
					UsersAdapter.FilterString = SearchString;

				else
					UsersAdapter.ResetFilter();

				UsersPaginator.PageCount = UsersAdapter.GetPageCount();
				RefreshUserPanel(1);

			}

		}

		// Кнопка поиска панели организаций
		private void OrgSearchButtonClick(Object s, EventArgs e)
		{

			string SearchString = TbOrgSearch.Text.Trim();

			if (SearchString != OrganizationsAdapter.FilterString)
			{

				if (SearchString != "")
					OrganizationsAdapter.FilterString = SearchString;

				else
					OrganizationsAdapter.ResetFilter();

				OrgPaginator.PageCount = OrganizationsAdapter.GetPageCount();
				RefreshOrgPanel(1);

			}

		}


		#endregion

	}
}