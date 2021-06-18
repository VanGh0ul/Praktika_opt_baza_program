using System;
using System.Drawing;
using System.Windows.Forms;

using Skladik.Utils;
using Skladik.NewComponents;

namespace Skladik.Forms {
	public class DynProductsBandForm : DynForm {

		const TableLayoutPanelCellBorderStyle BorderStyle = TableLayoutPanelCellBorderStyle.Inset; 

													// Категории
		public IdBasedPanelList Categories { get; private set; }
		public Button BCancel { get; private set; }

													// Панель аккаунта
		public Button BExit { get; private set; }
		public Label LName { get; private set; }
													//Заголовок ленты
		public TextBox TbProductSearch { get; private set; }
		public Button BProductSearch { get; private set; }
		public Button BCancelSearch{ get; private set; }
		public ComboBox CbAddedOn { get; private set; }
		public ComboBox CbPrice { get; private set; }
		public Button BAgree { get; private set; }
		public Paginator PageSelector { get; private set; }

													// Лента товаров
		public ProductBand Band { get; private set; }

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

													// Генерация формы
		public override void Generate(Form1 aForm) {
			base.Generate(aForm);

			programForm.Controls.Clear();
			Size FormSize = new Size(1300, 600);
			programForm.MinimumSize = FormSize;
			//aForm.MaximumSize = FormSize;
			programForm.Size = FormSize;
			programForm.Text = "Просмотр товаров";

			TableLayoutPanel FormContent = GenerateAdminForm("Пользователь 2");
			// TableLayoutPanel FormContent = GenerateDetachedUserForm("Пользователь 2");
			// TableLayoutPanel FormContent = GenerateAttachedUserForm("Пользователь 2", "Организация 1", Properties.Resources.no_image);

			programForm.Controls.Add(FormContent);

		}

		#region Рамки
													// Рамка формы пользователя
		private TableLayoutPanel CreateUserFrame() {

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
		private TableLayoutPanel CreateAdminFrame() {
			
			TableLayoutPanel Temp = CreateUserFrame();

			Temp.CellBorderStyle = BorderStyle;

			Temp.ColumnCount = 7;

			Temp.ColumnStyles[3] = new ColumnStyle(SizeType.Percent, 80);
			Temp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
			Temp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));

			return Temp;

		}
				
													// Рамка категорий для неприкрепленных пользователей	
		private TableLayoutPanel CreateDetachedCategoryFrame() {
		
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
		private TableLayoutPanel CreateAttachedCategoryFrame() {
			
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
		private TableLayoutPanel CreateBandFrame() {
			
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
		private TableLayoutPanel CreateAdminPanelFrame() {
			
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
		private TableLayoutPanel CreateCategoryList() {

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
			LTitle.Font = new Font("Comic Sans MS", 20);
			LTitle.Margin = new Padding(5, 5, 5, 5);
			LTitle.Location = new Point(5, 5);

			TitlePanel.Controls.Add(LTitle);

													// Список категорий
			Categories = new IdBasedPanelList();
			Categories.Font = Styles.TextFont;

													// Добавление категорий
			Categories.AddAPanel(1, "Мясо");
			Categories.AddAPanel(2, "Овощи");
			Categories.AddAPanel(1, "Мясо");

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
		private Panel CreateDetachedAccountPanel(string name)  {
			
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
		private Panel CreateAtachedAccountPanel(string name, string orgName, Image img) {
		
			Panel Result = new Panel();
			Result.Dock = DockStyle.Fill;
			
													// Имя пользователя
			LName = new Label();
			LName.Text = name;
			LName.Location = new Point(80, 25);
			Styles.TextStyle(LName);

			Label LOrgName = new Label();
			LOrgName.Text = orgName;
			LOrgName.Location = new Point(80, 5);
			LOrgName.AutoSize = true;
			LOrgName.Font = new Font("Comic Sans MS", 12);

													// Кнопка выхода
			BExit = new Button();
			BExit.Text = "Выйти";
			BExit.Font = Styles.TextFont;
			BExit.Size = new Size(80, 25);
			BExit.Location = new Point(140, 55);
			
													// Аватар
			PictureBox PbAvatar = new PictureBox();
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
		private TableLayoutPanel CreateBandHeader() {
		
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

			CbPrice = new ComboBox();
			CbPrice.Font = Styles.TextFont;
			CbPrice.Items.Add("по умолчанию");
			CbPrice.Items.Add("по возрастанию");
			CbPrice.Items.Add("по убыванию");
			CbPrice.Dock = DockStyle.Fill;

			BAgree = new Button();
			BAgree.Text = "Прим.";
			BAgree.Font = Styles.TextFont;
			BAgree.Dock = DockStyle.Fill;
			BAgree.Margin = new Padding(5, 1, 5, 1);

			// ### Изменить начальное значение количества страниц ###
			PageSelector = new Paginator(1, Styles.PaginatorLabelCount);
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

			PageSelector.PageChanged += delegate(Object s, EventArgs e) {
				
				int PageNum = ((Paginator)s).ChoosedPage;

				MessageBox.Show(PageNum.ToString());

			};

			return Result;

		}

													// Создание ленты товаров
		private TableLayoutPanel CreateProductBand() {
			
													// Рамка для ленты
			TableLayoutPanel BandFrame = CreateBandFrame();

			Band = new ProductBand();
			Band.Dock = DockStyle.Fill;

			BandFrame.Controls.Add(CreateBandHeader(), 0, 0);
			BandFrame.Controls.Add(Band, 0, 1);

			return BandFrame;

		}

													// Создание админской панели пользователя
		private TableLayoutPanel CreateAdminUserPanel() {
		
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

			UsersPaginator = new Paginator(1, 4);
			Styles.PaginatorStyle(UsersPaginator);
			UsersPaginator.Font = new Font("Comic Sans MS", 8);

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
			UserList.Font = Styles.TextFont;

			Result.Controls.Add(Header, 0, 0);
			Result.Controls.Add(UserList, 0, 2);

			return Result;

		}
													
													// Создание админской панели организации
		private TableLayoutPanel CreateAdminOrganizationPanel() {
		
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

			OrgPaginator = new Paginator(1, 4);
			Styles.PaginatorStyle(OrgPaginator);
			OrgPaginator.Font = new Font("Comic Sans MS", 8);

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
			OrgList.Font= Styles.TextFont;

			Result.Controls.Add(Header, 0, 0);
			Result.Controls.Add(OrgList, 0, 2);

			return Result;

		}

													// Создание админской панели
		private TableLayoutPanel CreateAdminPanel() {
			
			TableLayoutPanel Result = CreateAdminPanelFrame();

			Result.Controls.Add(CreateAdminUserPanel(), 0, 0);

			Result.Controls.Add(CreateAdminOrganizationPanel(), 0, 2);

			return Result;

		}
		

		#endregion

		#region Формы
													// Генерация формы администатора
		private TableLayoutPanel GenerateAdminForm(string name) { 
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
		private TableLayoutPanel GenerateAttachedUserForm(string name, string orgName, Image img) { 
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
			BIncomingOrders = new Button();

			BOutgoingOrders.Text = "Исхоящие заказы";
			BIncomingOrders.Text = "Входящие заказы";
 
			BOutgoingOrders.Dock = DockStyle.Fill;
			BIncomingOrders.Dock = DockStyle.Fill;

			Orders.Controls.Add(BOutgoingOrders, 0, 0);
			Orders.Controls.Add(BIncomingOrders, 0, 1);
			#endregion

			#region Количество важных заказов
			ImportantIncoming = new Label();
			ImportantOutgoing = new Label();

			ImportantIncoming.Text = "0";
			ImportantOutgoing.Text = "0";

			Styles.TextStyle(ImportantIncoming);
			Styles.TextStyle(ImportantOutgoing);

			ImportantIncoming.Anchor = AnchorStyles.None;
			ImportantOutgoing.Anchor = AnchorStyles.None;

			Orders.Controls.Add(ImportantOutgoing, 1, 0);
			Orders.Controls.Add(ImportantIncoming, 1, 1);

			#endregion

			CategoryFrame.Controls.Add(Orders, 0, 4);
			CategoryFrame.Controls.Add(CreateAtachedAccountPanel(name, orgName, img), 0, 6);

			FormFrame.Controls.Add(CategoryFrame, 1, 1);

			// Создать ленту
			FormFrame.Controls.Add(CreateProductBand(), 3, 1);
			
			return FormFrame;
		}

													// Генерация формы непривязанного пользователя
		private TableLayoutPanel GenerateDetachedUserForm(string name) { 
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

	}
}