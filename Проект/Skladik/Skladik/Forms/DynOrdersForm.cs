using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

using Skladik.Forms;
using Skladik.Adapters;
using Skladik.Adapters.BandAdapters;
using Skladik.Utils;
using Skladik.NewComponents;

namespace Skladik.Forms {
													// Форма вывода списков заказов
	class DynOrdersForm : DynForm{

		#region Объявления
		 
		private BackMainHeader Header;

		public FilterableBandAdapter OrderAdapter { get; private set; }

		public Button BOk { get; private set; }
		public Button BCancel { get; private set; }

		public ComboBox CbStatus { get; private set; }
		public Paginator PagOrdersList { get; private set; }

		public OrdersList Orders { get; private set; }

		private int OrganizationId;

		#endregion

		protected override void ContentToUpdate() {
			RefreshOrderList(PagOrdersList.ChoosedPage);
		}

													// Настройка формы
		protected override void SetUpMainForm() {
			programForm.Controls.Clear();
			Size FormSize = new Size(800, 600);
			programForm.Location = Styles.CentralizeFormByAnotherOne(FormSize, programForm.Location, programForm.Size);
			programForm.MinimumSize = FormSize;
			programForm.MaximumSize = new Size(800, 800);;
			programForm.Size = FormSize;
			programForm.Text = "Просмотр заказов";
		}

													// Генерация формы
		public void Generate(Form1 aForm, int organizationId, OrdersType type) {
			
			base.Generate(aForm);

			OrganizationId = organizationId;

													// Формирование формы
			formContent = CreateForm(type);
											
			if (type == OrdersType.Outgoing) 
				FillOutgoingOrderForm(organizationId);

			else 
				FillIncomingOrderForm(organizationId);
			
			FillStatusComboBox(type);

			EventSetUp();

			programForm.Controls.Add(formContent);

		}

		#region Рамки
													// Основная рамка	
		private TableLayoutPanel CreateMainFrame() {
			
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

													// Заголовок
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

													// Список
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			return Result;

		}

		#endregion

		#region Части формы
		
													// Заголовок списка заказов
		private TableLayoutPanel CreateOrdersHeader(OrdersType type) {
						
			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);
			
			#region Рамка

			Result.ColumnCount = 7;
			Result.RowCount = 1;

													// Промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));

													// Текст
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

													// Промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));
													
													// Текст и комбобокс
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 390));

													// Промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

													// Пагинатор
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

													// Промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));


			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Компоненты
			
													// Заголовок
			Label LTitle = new Label();
			Styles.TextStyle(LTitle, 14);
			LTitle.Anchor = AnchorStyles.None;
			if (type == OrdersType.Incoming)
				LTitle.Text = "Входящие заказы";

			else 
				LTitle.Text = "Мои заказы";

			#region Текст и комбобокс

			TableLayoutPanel TextCbFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(TextCbFrame, TableLayoutPanelCellBorderStyle.None);

			#region Рамка

			TextCbFrame.ColumnCount = 2;
			TextCbFrame.RowCount = 2;

			TextCbFrame.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			TextCbFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
			
			TextCbFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
			TextCbFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

			#endregion

			#region Компоненты
			
			Label LText = new Label();
			Styles.TextStyle(LText);
			LText.Anchor = AnchorStyles.None;
			LText.Text = "Показать заказы со статусом: ";

			CbStatus = new ComboBox();

			Styles.ComboBoxStyle(CbStatus);

			#region Применить/отменить фильтр

			TableLayoutPanel FilterButtons = new TableLayoutPanel();
			Styles.ContentFramesStyle(FilterButtons, TableLayoutPanelCellBorderStyle.None);

			FilterButtons.ColumnCount = 2;
			FilterButtons.RowCount = 1;

			FilterButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
			FilterButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

			FilterButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			BOk = new Button();
			Styles.ButtonStyle(BOk);
			BOk.Font = new Font("Comic Sans MS", 9);
			BOk.Text = "Ок";
			BOk.Margin = new Padding(1);

			BCancel = new Button();
			Styles.ButtonStyle(BCancel);
			BCancel.Font = new Font("Comic Sans MS", 9);
			BCancel.Text = "Отменить";
			BCancel.Margin = new Padding(1);

			FilterButtons.Controls.Add(BOk, 0, 0);
			FilterButtons.Controls.Add(BCancel, 1, 0);

			#endregion

			#endregion

			TextCbFrame.Controls.Add(LText, 0, 0);
			TextCbFrame.Controls.Add(CbStatus, 1, 0);
			TextCbFrame.Controls.Add(FilterButtons, 1, 1);

			#endregion

													// Пагинатор
			PagOrdersList = new Paginator(Styles.PaginatorLabelCount);
			Styles.PaginatorStyle(PagOrdersList);

			Result.Controls.Add(LTitle, 1, 0);
			Result.Controls.Add(TextCbFrame, 3, 0);
			Result.Controls.Add(PagOrdersList, 5, 0);

			#endregion

			return Result;

		}

		#endregion

		#region Формы
		
		private TableLayoutPanel CreateForm(OrdersType type) {
			
			TableLayoutPanel Result = CreateMainFrame();

			Header = new BackMainHeader();
			
			Styles.ContentFramesStyle(Header, BorderStyle);
			Header.BGoBack.Font = new Font("Comic Sans MS", 8);
			Header.BGoMain.Font = new Font("Comic Sans MS", 8);

			Result.Controls.Add(Header, 1, 1);
			Result.Controls.Add(CreateOrdersHeader(type), 1, 3);

			Orders = new OrdersList();

			Result.Controls.Add(Orders, 1, 5);

			return Result;

		}

		#endregion

		#region Вывод данных

													// Заполнение комбобокса статусов
		public void FillStatusComboBox(OrdersType type) {

													// Получение статусов
			MySqlCommand Query = programForm.Conn.CreateCommand();
			Query.CommandText = "select * from order_status";

			if (type == OrdersType.Incoming) 
				Query.CommandText += " where not name = 'editing'";
			

			MySqlDataAdapter QueryAdapter = new MySqlDataAdapter(Query);

			DataSet DSet = new DataSet();

			QueryAdapter.Fill(DSet);
			programForm.Conn.Close();

													// Передача в источник КБ-а
			CbStatus.DataSource = DSet.Tables[0];
			CbStatus.DisplayMember = "rus_name";
			CbStatus.ValueMember = "id";
			CbStatus.BindingContext = new BindingContext();
			
		}

													// Обновление списка заказов
		public void RefreshOrderList(int page) {
			Orders.Controls.Clear();

			Orders.DataSource = OrderAdapter.GetData(page);
		}
		

		#region Исходящие заказы
		
													// Алгоритм отображения данных исходящих заказов
		private OrderPanel OutgoingOrderAddStrategy(OrderPanel ordPanel, DataRow row) {

													// Данные что достаются без проблем
			ordPanel.OrderId = Convert.ToInt32(row["ord_id"]);
			ordPanel.LName.Text = "Заказ №" + (ordPanel.OrderId + 2000).ToString();
			ordPanel.LSeller.Text = "Поставщик: SKLADIK";
			//ordPanel.LDeliverAddress.Text = "Адрес доставки: " + row["adr_name"].ToString();
			ordPanel.LStatus.Text = "Статус: " + row["st_rus_name"].ToString();

													// Даты могут отсутствовать, при попытке

													// перевода пустых дат в DateTime застывает программа

			ordPanel.LSum.Text = "Общая сумма: ";
			
													// Если статус заказа "в процессе оформления", нужно
													// посчитать его общую сумму, все остальные статусы
													// хранят в себя уже зафиксированную суммму

			if (row["status"].ToString() == "editing") {
	
				MySqlCommand Query = programForm.Conn.CreateCommand();
				Query.CommandText = "select sum(oc.quantity * p.price) from order_content oc, product p where oc.order_id = @order_id and oc.product_id = p.id";
				
				Query.Parameters.Add("order_id", MySqlDbType.Int32).Value = ordPanel.OrderId;
				
				programForm.Conn.Open();
				
				ordPanel.LSum.Text += Query.ExecuteScalar().ToString() + " руб.";
				
				programForm.Conn.Close();

			} else
				ordPanel.LSum.Text += row["order_sum"].ToString() + " руб.";

			return ordPanel;
			
		}

													// Подготовка адаптера к работе с исходящими заказами
		private void PrepareOutgoingOrdersAdapter(int orgId) {
			
			MySqlCommand SelectCommand = programForm.Conn.CreateCommand();
			SelectCommand.CommandText =
				"select bo.id as ord_id, bo.sent_on, bo.deliver_date, a.name as adr_name, st.name as status, bo.order_sum, bo.status_id, st.rus_name as st_rus_name " +
				"from " +
					"buy_order bo inner join order_status st on bo.status_id = st.id " +
 					"inner join organization o on bo.buyer_id = o.id " +
					"left join address a on organization_id = a.id " +
					//"inner join address ad on status_id = ad.id " +
					"where bo.buyer_id = @org_id";

			SelectCommand.Parameters.Add("org_id", MySqlDbType.Int32).Value = orgId;

			MySqlCommand SelectCount = programForm.Conn.CreateCommand();
			SelectCount.CommandText =
				"select count(*) " + 
				"from buy_order bo " +
				"where buyer_id = @org_id";

			SelectCount.Parameters.Add("org_id", MySqlDbType.Int32).Value = orgId;

			OrderAdapter = new FilterableBandAdapter(SelectCommand, SelectCount, Styles.OrderListElemCount, programForm.Conn);

		}

													// Выгрузка данных о исходящих заказах на форму
		private void FillOutgoingOrderForm(int orgId) {
			
													// Передача алгоритма 
			Orders.OrderAddStrategy += OutgoingOrderAddStrategy;

													// Подготовка адаптера
			PrepareOutgoingOrdersAdapter(orgId);

													// Подготовка пагинатора
			PagOrdersList.PageCount = OrderAdapter.GetPageCount();

													// Выгрузка данных
			RefreshOrderList(1);

													// Событие при изменении страницы пагинатора
			PagOrdersList.PageChanged += delegate(Object s, EventArgs args) {
				RefreshOrderList(PagOrdersList.ChoosedPage);
			};

		}
		
		
		#endregion

		#region Входящие заказы

													// Алгоритм отображения данных входящих заказов
		private OrderPanel IncomingOrderAddStrategy(OrderPanel ordPanel, DataRow row) {
		
													// Данные что достаются без проблем
			ordPanel.OrderId = Convert.ToInt32(row["ord_id"]);
			ordPanel.LName.Text = "Заказ №" + (ordPanel.OrderId + 2000).ToString();
			ordPanel.LSeller.Text = "Покупатель: " + row["org_name"].ToString();

			//ordPanel.LDeliverAddress.Text = "Адрес доставки: " + row["adr_name"].ToString();

			ordPanel.LStatus.Text = "Статус: " + row["st_rus_name"].ToString();

			//ordPanel.LSentOn.Text = "Дата отправки: " + QueryUtils.TryGetTheDate(row["sent_on"]);
			//ordPanel.LDeliverOn.Text = "Дата доставки: " + QueryUtils.TryGetTheDate(row["deliver_date"]);

			ordPanel.LSum.Text = "Общая сумма: " + row["order_sum"].ToString();

			return ordPanel;

		}
		
													// Подготовка адаптера к работе со входящими заказами
		private void PrepareIncomingOrdersAdapter(int orgId) {
			
			MySqlCommand SelectCommand = programForm.Conn.CreateCommand();
			SelectCommand.CommandText =
				"select bo.id as ord_id, o.name as org_name, bo.sent_on, a.name as adr_name, bo.deliver_date, st.name as status, bo.order_sum, bo.status_id, st.rus_name as st_rus_name " + 
				"from " +
					"buy_order bo inner join order_status st on bo.status_id = st.id " +
 					"inner join organization o on bo.buyer_id = o.id " +
					"left join address a on organization_id = a.id " +
				"where " +
					"not st.name = 'editing'";

			SelectCommand.Parameters.Add("org_id", MySqlDbType.Int32).Value = orgId;

			MySqlCommand SelectCount = programForm.Conn.CreateCommand();
			SelectCount.CommandText =
				"select count(*) " +
				"from buy_order bo, order_status s " +
				"where " +
					"bo.status_id = s.id and " +
					"not s.name = 'editing'";

			SelectCount.Parameters.Add("org_id", MySqlDbType.Int32).Value = orgId;

			OrderAdapter = new FilterableBandAdapter(SelectCommand, SelectCount, Styles.OrderListElemCount, programForm.Conn);

		}

													// Выгрузка данных о входящих заказах на форму
		private void FillIncomingOrderForm(int orgId) {
			
													// Передача алгоритма 
			Orders.OrderAddStrategy += IncomingOrderAddStrategy;

													// Подготовка адаптера
			PrepareIncomingOrdersAdapter(orgId);

													// Подготовка пагинатора
			PagOrdersList.PageCount = OrderAdapter.GetPageCount();
			
													// Выгрузка данных
			RefreshOrderList(1);
			
													// Событие при изменении страницы пагинатора
			PagOrdersList.PageChanged += delegate(Object s, EventArgs args) {
				RefreshOrderList(PagOrdersList.ChoosedPage);
			};
			
		}

		#endregion

		#region Фильтрация по статусу

													// Алгоритм фильтрации заказов
		private MySqlCommand FilterOrdersStrategy(MySqlCommand query, string filterString) {
		
			string CommandText = 
				"bo.status_id = @search";

			query.Parameters.Add("search", MySqlDbType.Int32).Value = filterString;

			if (!query.CommandText.Contains("where"))
				query.CommandText += " where " + CommandText;
			else
				query.CommandText += " and " + CommandText;

			return query;

		}
		
													// Кнопка активации фильтрации
		private void BFilterOkClick(Object s, EventArgs e) {
			if (CbStatus.SelectedValue != null) {
				OrderAdapter.FilterString = CbStatus.SelectedValue.ToString();
				PagOrdersList.PageCount = OrderAdapter.GetPageCount();
				RefreshOrderList(1);
			} else 
				MessageBox.Show("Выберите статус из представленного списка");
		
		}

													// Кнопка деактивации фильтрации
		private void BFilterCancelClick(Object s, EventArgs e) {
		
			OrderAdapter.ResetFilter();
			PagOrdersList.PageCount = OrderAdapter.GetPageCount();
			RefreshOrderList(1);

		}

		#endregion

		#endregion

		#region События
		
													// Общие для всех форм события					
		private void EventSetUp() {
													// Переход назад
			Header.BGoBack.Click += delegate(Object s, EventArgs args) {
				programForm.History.Pop().RegenerateOldForm();
			};

													// Переход на главн. форму
			Header.BGoMain.Click += delegate(Object s, EventArgs args) { 
				new DynProductsBandForm().Generate(programForm);
			};

			OrderAdapter.SearchStrategy = FilterOrdersStrategy;

			BOk.Click += BFilterOkClick;
			BCancel.Click += BFilterCancelClick;

			Orders.OrderSelected += GoToOrderContent;

		}

													// Открытие содержимого заказа
		private void GoToOrderContent(Object s, EventArgs e) {
			
			new DynOrderContentForm().Generate(programForm, ((OrderPanel)s).OrderId, OrganizationId);
			programForm.History.Push(this);

			// MessageBox.Show("Переход на форму содержимого заказа " + ChoosedOrder.OrderId.ToString());
		}

		#endregion

	}

}
