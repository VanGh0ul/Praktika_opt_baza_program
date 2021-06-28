using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using Skladik.Utils;
using Skladik.NewComponents;
using Skladik.Adapters;
using Skladik.Adapters.BandAdapters;

namespace Skladik.Forms {
													// Форма просмотра содержимого заказа
	class DynOrderContentForm : DynForm {

		#region Объявления

													// Кнопки перехода	
		private BackMainHeader Header;

													// Адаптер списка товаров в содержимом заказа
		private BandAdapter ProductListAdapter;

													// Отображение содержимого заказа в виде
													// списка, только для чтения
		private ReadOnlyProductsList RoProductList;

													// Отображение содержимого заказа в виде
													// списка, что можно редактировать
		private EditableProductsList EdProductList;

													// Адаптер заказа
		private OrderDataAdapter Order;

													// Общая информация заказа
		public Label LStatus { get; private set; }
		public Label LSum { get; private set; }

													// Заголовок списка
		public Label LTitle { get; private set; }
		public Paginator PagOrderContent { get; private set; }
		
													// Роль пользователя в заказе (Поставщик или Покупатель)
		public Label LUserRoleInOrder { get; private set; }

													// Информация о заказе
		public Label LOrgName { get; private set; }
		public Label LSentOn { get; private set; }

		public Label LDeliverOn { get; private set; }
		public Label LDeliverAddress { get; private set; }

		public DateTimePicker DtpDeliverOn { get; private set; }
		public ComboBox CbDeliverAddress { get; private set; } 

													// Кнопки управления заказом
		public Button BCreateDocument { get; private set; }
		public Button BAccept { get; private set; }
		public Button BDecline { get; private set; }

		public Button BSave { get; private set; }
		public Button BCancel { get; private set; }
		public Button BSend { get; private set; }
		public Button BDelete { get; private set; }

		public Button BDelivered { get; private set; }

		public Button BCancelOrder { get; private set; }

		#endregion

		protected override void SetUpMainForm() {
			programForm.Controls.Clear();
			Size FormSize = new Size(900, 600);
			programForm.Location = Styles.CentralizeFormByAnotherOne(FormSize, programForm.Location, programForm.Size);
			programForm.MinimumSize = FormSize;
			programForm.MaximumSize = new Size(900, 800);;
			programForm.Size = FormSize;
			programForm.Text = "Просмотр заказов";
		}

		public void Generate(Form1 aForm, int orderId, int orgId) {

			base.Generate(aForm);

			Order = new OrderDataAdapter(programForm.Conn);

			if (!Order.GetData(orderId)) {
				MessageBox.Show("Произошла ошибка при открытии содержимого заказа");
				programForm.History.Pop().RegenerateOldForm();
			}

			// Форма администратора
			if (programForm.User.Role == "usual") {
				
				formContent = CanceledOrderForm();
				FillAdminForm(orgId);

			} if (Order.Status != "canceled" && Order.Status != "declined") {
				
													// Если ид организации пользователя и ид покупателя заказа
													// совпадают, заказ исходящий
				if (programForm.User.Organization.Id == Order.BuyerId) {
												
													// Заказы исх. в процессе оформления
					if (Order.Status == "editing") {
						
													// Форма что позволяет редактировать данные заказа
						formContent = EditingOutOrderForm();
						FillEditableForm();
						EditingOutEventSetUp();

					} else {
						
													// Заказы исх. "отправлен на рассмотрение"
						if (Order.Status == "waiting") {
							formContent = WaitingOutOrderForm();
							WaitingOutEventSetUp();

													// Заказы исх. "принят"
						} else if (Order.Status == "accepted") {
							formContent = AcceptedOutOrderForm();
							AcceptedIncEventSetUp();

						} else 
							formContent = DeliveredOutOrderForm();
						
						FillUserReadOnlyForm(orgId);

					}
								
													// иначе входящий
				} else { 
				
													// заказы вход. "в процессе оформления"
					if (Order.Status == "waiting") {
						formContent = WaitingIncOrderForm();
						WaitingIncEventSetUp();

													// заказы вход. "принятые" или "доставленные"
					} else 
						formContent = AcceptedOrDeliveredIncOrderForm();

					FillUserReadOnlyForm(orgId);
				
				}
				
													// отмененные и отклоненные заказы
			} else {

				formContent = CanceledOrderForm();
				FillUserReadOnlyForm(orgId);

			}

			EventSetUp();


			programForm.Controls.Add(formContent);

		}


		#region Рамки
		
													// Базисная рамка окна
		private TableLayoutPanel CreateMainFrame() { 
			
			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, BorderStyle);

			Result.ColumnCount = 3;
			Result.RowCount = 5;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

													// Кнопки перехода
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

													// Содержимое
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			return Result;
		
		}

													// Рамка информативной панели заказа
		private TableLayoutPanel CreateOrderInfoPanelFrame() {
		
			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);
			Result.AutoSize = true;

			Result.ColumnCount = 2;
			Result.RowCount = 4;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			return Result;

		}
		
													// Рамка кнопок 
		private TableLayoutPanel CreateButtonsFrame() {
		
			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			Result.ColumnCount = 3;
			Result.RowCount = 7;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

													// Кнопка
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.OrderContentFormButtonsHeight));

													// Кнопка
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.OrderContentFormButtonsHeight));

													// Кнопка
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.OrderContentFormButtonsHeight));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

													// Кнопка
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.OrderContentFormButtonsHeight));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			return Result;

		}


		#endregion

		#region Части форм
													// Создание заголовка списка
		private TableLayoutPanel CreateListHeader() { 
		
			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			#region Рамка
			
			Result.ColumnCount = 5;
			Result.RowCount = 1;

													// Промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

													// Заголовк
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

													// Промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

													// Пагинатор
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

													// Промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

			
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Компоненты
													// Заголовок
			LTitle = new Label();
			Styles.TextStyle(LTitle, 14);
			LTitle.Anchor = AnchorStyles.None;

													// Пагинатор
			PagOrderContent = new Paginator(Styles.PaginatorLabelCount);
			Styles.PaginatorStyle(PagOrderContent);
			
			#endregion

			Result.Controls.Add(LTitle, 1, 0);
			Result.Controls.Add(PagOrderContent, 3, 0);


			return Result;

		}

													// Создание панели списка
		private TableLayoutPanel CreateListFrame(FlowLayoutPanel list) {
		
			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, BorderStyle);

			Result.ColumnCount = 1;
			Result.RowCount = 3;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			
													// Заголовок списка
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

													// Список
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			Result.Controls.Add(CreateListHeader(), 0, 0);
			Result.Controls.Add(list, 0, 2);

			return Result;

		}

													// Создание панели с информацией о заказе
		private TableLayoutPanel CreateOrderInfoPanelFrame(TableLayoutPanel orderInfoPanel, TableLayoutPanel buttonsPanel) {
		
			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			#region Рамка

			Result.ColumnCount = 3;
			Result.RowCount = 8;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

													// Статус
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

													// Информативная панель
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

													// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Сумма
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Кнопки
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));

			#endregion

			#region Компоненты
													// Статус заказа
			LStatus = new Label();
			Styles.TextStyle(LStatus);
			
													// Текст
			Label LSumText = new Label();
			Styles.TextStyle(LSumText);

													// Сумма
			LSum = new Label();


			#endregion

			Result.Controls.Add(LStatus, 1, 1);
			Result.Controls.Add(LSumText, 1, 5);
			Result.Controls.Add(LSum, 1, 6);

			Result.Controls.Add(orderInfoPanel, 1, 3);
			 
			if (buttonsPanel != null)
				Result.Controls.Add(buttonsPanel, 1, 7);

			return Result;

		}

													// Создание панели содержимого формы
		private TableLayoutPanel CreateContentFrame(TableLayoutPanel orderInfoPanel, TableLayoutPanel buttonsPanel, FlowLayoutPanel list) { 
		
			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, BorderStyle);

			Result.ColumnCount = 3;
			Result.RowCount = 1;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));

			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			Result.Controls.Add(CreateListFrame(list), 0, 0);
			Result.Controls.Add(CreateOrderInfoPanelFrame(orderInfoPanel, buttonsPanel), 2, 0);


			return Result;

		}


		#region Информативные панели заказа

		// Общая компонентная часть для следующих 2 панелей
		private TableLayoutPanel CreateOrderInfoPanel()
		{

			TableLayoutPanel Result = CreateOrderInfoPanelFrame();

			// Надписи с левой стороны
			LUserRoleInOrder = new Label();
			Styles.TextStyle(LUserRoleInOrder);


			// Информация на правой строне
			LOrgName = new Label();
			Styles.TextStyle(LOrgName);

			//LSentOn = new Label();
			//Styles.TextStyle(LSentOn);


			Result.Controls.Add(LUserRoleInOrder, 0, 0);


			Result.Controls.Add(LOrgName, 1, 0);
			//Result.Controls.Add(LSentOn, 1, 1);


			return Result;

		}

		// Панель только для чтения
		private TableLayoutPanel CreateReadOnlyOrderInfoPanel() {
			
			TableLayoutPanel Result = CreateOrderInfoPanel();

			LDeliverOn = new Label();
			Styles.TextStyle(LDeliverOn);

			LDeliverAddress = new Label();
			Styles.TextStyle(LDeliverAddress);

			Result.Controls.Add(LDeliverOn, 1, 2);
			Result.Controls.Add(LDeliverAddress, 1, 3);

			return Result;

		}

													// Панель для редактирования
		private TableLayoutPanel CreateEditableOrderInfoPanel() {
			
			TableLayoutPanel Result = CreateOrderInfoPanel();

			DtpDeliverOn = new DateTimePicker();
			DtpDeliverOn.Font = Styles.TextFont;
			DtpDeliverOn.Margin = new Padding(0);
			DtpDeliverOn.Dock = DockStyle.Fill;

			CbDeliverAddress = new ComboBox();
			Styles.ComboBoxStyle(CbDeliverAddress);

			Result.Controls.Add(DtpDeliverOn, 1, 2);
			Result.Controls.Add(CbDeliverAddress, 1, 3);

			return Result;

		}

		#endregion


		#region Панели кнопок

		#region Исходящие заказы
		
													// Создать панель кнопок для изменяемого заказа
		private TableLayoutPanel CreateEditableOrderButtonsPanel() {
		
			TableLayoutPanel Result = CreateButtonsFrame();

													// Кнопка сохранения изменений заказа
													// (дата доставки и адрес доставки)
			BSave = new Button();
			Styles.ButtonStyle(BSave);
			BSave.Margin = new Padding(2);
			BSave.Text = "Сохранить изменения";

													// Кнопка отмены изменений заказа
			BCancel = new Button();
			Styles.ButtonStyle(BCancel);
			BCancel.Margin = new Padding(2);
			BCancel.Text = "Отменить изменения";

													// Кнопка отправки заказа на рассмотрение
			BSend = new Button();
			Styles.ButtonStyle(BSend);
			BSend.Margin = new Padding(2);
			BSend.Text = "Отправить заказ";

													// Кнопка удаления заказа
			BDelete = new Button();
			Styles.ButtonStyle(BDelete);
			BDelete.Margin = new Padding(2);
			BDelete.Text = "Удалить заказ";


			Result.Controls.Add(BSave, 1, 1);
			Result.Controls.Add(BCancel, 1, 2);
			Result.Controls.Add(BSend, 1, 3);
			Result.Controls.Add(BDelete, 1, 5);

			return Result;

		}


													// Создать панель кнопки для получения накладной
		private TableLayoutPanel CreateWaybillButtonPanel() {
		
			TableLayoutPanel Result = CreateButtonsFrame();

			//BCreateDocument = new Button();
			//Styles.ButtonStyle(BCreateDocument);
			//BCreateDocument.Margin = new Padding(2);
			//BCreateDocument.Text = "Составить накладную";

			//Result.Controls.Add(BCreateDocument, 1, 1);

			return Result;

		}


													// Создать панель кнопок для принятого заказа
		private TableLayoutPanel CreateAcceptedOutOrderButtonsPanel() {
		
			TableLayoutPanel Result = CreateWaybillButtonPanel();

			BDelivered = new Button();
			Styles.ButtonStyle(BDelivered);
			BDelivered.Margin = new Padding(2);
			BDelivered.Text = "Доставлен";

			Result.Controls.Add(BDelivered, 1, 2);

			return Result;

		}


													// Создать панель с кнопкой отмены заказа
		private TableLayoutPanel CreateCancelButtonPanel() {
		
			TableLayoutPanel Result = CreateButtonsFrame();

			BCancelOrder = new Button();
			Styles.ButtonStyle(BCancelOrder);
			BCancelOrder.Margin = new Padding(2);
			BCancelOrder.Text = "Отменить заказ";

			Result.Controls.Add(BCancelOrder, 1, 1);
			

			return Result;

		}

		#endregion


		#region Входящие заказы
		
													// Создать панель с кнопкой составления содержания заказа
		private TableLayoutPanel CreateOrderContentDocumentButtonPanel() {
			
			TableLayoutPanel Result = CreateButtonsFrame();

			//													// Кнопка составления содержания заказа
			//BCreateDocument = new Button();
			//Styles.ButtonStyle(BCreateDocument);
			//BCreateDocument.Margin = new Padding(2);
			//BCreateDocument.Text = "Составить содержание заказа";

			//Result.Controls.Add(BCreateDocument, 1, 1);

			return Result;

		}

													// Создать панель кнопок для заказа "отправлен на рассмотрение"
		private TableLayoutPanel CreateWaitingIncOrderButtonsPanel() {
			
			TableLayoutPanel Result = CreateOrderContentDocumentButtonPanel();

													// Кнопка принятия заказа
			BAccept = new Button();
			Styles.ButtonStyle(BAccept);
			BAccept.Margin = new Padding(2);
			BAccept.Text = "Принять";
			
													// Кнопка отклонения заказа
			BDecline = new Button();
			Styles.ButtonStyle(BDecline);
			BDecline.Margin = new Padding(2);
			BDecline.Text = "Отклонить";

			//Result.Controls.Add(BCreateDocument, 1, 1);
			Result.Controls.Add(BAccept, 1, 2);
			Result.Controls.Add(BDecline, 1, 3);

			return Result;

		}


		#endregion


		#endregion


		#endregion

		#region Формы

													// Создание базисной формы
		private TableLayoutPanel CreateMainForm(TableLayoutPanel orderInfoPanel, TableLayoutPanel buttonsPanel, FlowLayoutPanel list) {
		
			TableLayoutPanel Result = CreateMainFrame();
			
			Header = new BackMainHeader();
			
			Styles.ContentFramesStyle(Header, BorderStyle);
			Header.BGoBack.Font = new Font("Comic Sans MS", 8);
			Header.BGoMain.Font = new Font("Comic Sans MS", 8);

			Result.Controls.Add(Header, 1, 1);
			
			Result.Controls.Add(CreateContentFrame(orderInfoPanel, buttonsPanel, list), 1, 3);

			return Result;

		}

													// Создание формы редактирования исходящего заказа
													// что имеет статус "в процессе оформления"
		private TableLayoutPanel EditingOutOrderForm() {

			EdProductList = new EditableProductsList();

			return CreateMainForm(CreateEditableOrderInfoPanel(), CreateEditableOrderButtonsPanel(), EdProductList);

		}

													// Создание формы просмотра исходящего заказа
													// что имеет статус "принят"
		private TableLayoutPanel AcceptedOutOrderForm() {
		
			RoProductList = new ReadOnlyProductsList();

			return CreateMainForm(CreateReadOnlyOrderInfoPanel(), CreateAcceptedOutOrderButtonsPanel(), RoProductList);

		}

													// Создание формы просмотра исходящего заказа
													// что имеет статус "доставлен"
		private TableLayoutPanel DeliveredOutOrderForm() {
		
			RoProductList = new ReadOnlyProductsList();

			return CreateMainForm(CreateReadOnlyOrderInfoPanel(), CreateWaybillButtonPanel(), RoProductList);

		}

													// Создание формы просмотра исходящего заказа
													// что имеет статус "отправлен на рассмотрение"
		private TableLayoutPanel WaitingOutOrderForm() {
		
			RoProductList = new ReadOnlyProductsList();

			return CreateMainForm(CreateReadOnlyOrderInfoPanel(), CreateCancelButtonPanel(), RoProductList);

		}


													// Создание формы просмотра исходящего или входящего заказа
													// что имеет статус "отменен"
		private TableLayoutPanel CanceledOrderForm() {
		
			RoProductList = new ReadOnlyProductsList();

			return CreateMainForm(CreateReadOnlyOrderInfoPanel(), null, RoProductList);

		}


													// Создание формы просмотра входящего заказа
													// что имеет статус "доставлен" или "принят"
		private TableLayoutPanel AcceptedOrDeliveredIncOrderForm() {
		
			RoProductList = new ReadOnlyProductsList();

			return CreateMainForm(CreateReadOnlyOrderInfoPanel(), CreateOrderContentDocumentButtonPanel(), RoProductList);

		}

													// Создание формы просмотра входящего заказа
													// что имеет статус "отправлен на рассмотрение"
		private TableLayoutPanel WaitingIncOrderForm() {
		
			RoProductList = new ReadOnlyProductsList();

			return CreateMainForm(CreateReadOnlyOrderInfoPanel(), CreateWaitingIncOrderButtonsPanel(), RoProductList);

		}

		#endregion

		#region Вывод данных

		// Заполнение КБ адресов
		private void FillAddressComboBox() {
			
			MySqlCommand Query = programForm.Conn.CreateCommand();
			Query.CommandText = "select id, name from address where organization_id = @org_id and not is_deleted";

			Query.Parameters.Add("org_id", MySqlDbType.Int32).Value = programForm.User.Organization.Id;

			MySqlDataAdapter QueryAdapter = new MySqlDataAdapter(Query);

			DataSet DSet = new DataSet();

			QueryAdapter.Fill(DSet);
			programForm.Conn.Close();

			CbDeliverAddress.DataSource = DSet.Tables[0];
			CbDeliverAddress.ValueMember = "id";
			CbDeliverAddress.DisplayMember = "name";
			CbDeliverAddress.BindingContext = new BindingContext();

		}

													// Общие для всех форм данные
		private void FillForm() {
			LStatus.Text = "Статус: " + Order.RusStatus;
			//LSum.Text = Order.OrderSum + " руб.";
			LTitle.Text = "Заказ №" + (2000 + Order.Id).ToString();
			
			if (Order.SentOn != null)
				LSentOn.Text = ((DateTime)Order.SentOn).ToShortDateString();
			
		}

													// Вывод данных для форм только для чтения (общая)
		private void FillReadOnlyForm(int orgId) {
	
			FillForm();
													// Если организация пользователя - покупатель открытого заказа
													// выводим Ид поставщика, так как открывается исходящий заказ
			
			if (orgId == Order.BuyerId) {
				LUserRoleInOrder.Text += "Поставщик: SKLADIK";
				LOrgName.Text = Order.Seller;

													// Если он не покупатель, значит поставщик, и открывается
													// входящий заказ, а значит передаем покупателя
			} else {
				LUserRoleInOrder.Text += "Покупатель: ";
				LOrgName.Text += Order.Buyer;

			}

													// Дата доставки
			if (Order.DeliverOn != null)
				LDeliverOn.Text = ((DateTime)Order.DeliverOn).ToShortDateString();

													// Адрес доставки
			if (Order.Address != null)
				LDeliverAddress.Text = Order.Address;

		}
		
													// Вывод данных для форм только для чтения (привязанный пользователь)
		private void FillUserReadOnlyForm(int orgId) {
			
			FillReadOnlyForm(orgId);

			#region Настройка списка и пагинатора

			RoProductList.ProductAddStrategy = ReadOnlyProductAddStrategy;

			ProductListAdapter = Order.GetProducts();
			
			PagOrderContent.PageCount = ProductListAdapter.GetPageCount();

			RefreshReadOnlyProductList(1);

			PagOrderContent.PageChanged += delegate(Object s, EventArgs args) {
				RefreshReadOnlyProductList(PagOrderContent.ChoosedPage);
			};

			#endregion

		}

													// Вывод данных для форм только для чтения (администратор)
		private void FillAdminForm(int orgId) {
			
			FillReadOnlyForm(orgId);

													
			if (Order.Status == "editing") 
													// Алгоритм вывода данных из "editing" заказов
				RoProductList.ProductAddStrategy = AdminEditableProductAddStrategy;

			else
													// Алгоритм вывода данных из остальных заказов
				RoProductList.ProductAddStrategy = AdminReadOnlyProductAddStrategy;

			

			ProductListAdapter = Order.GetProducts();
			
			PagOrderContent.PageCount = ProductListAdapter.GetPageCount();

			RefreshReadOnlyProductList(1);

			PagOrderContent.PageChanged += delegate(Object s, EventArgs args) {
				RefreshReadOnlyProductList(PagOrderContent.ChoosedPage);
			};

		}

													// Вывод данных для формы редактирования
		private void FillEditableForm() {
		
			FillForm();

													// Для редактирования можно открывать только свои заказы
			LUserRoleInOrder.Text = "Поставщик: ";
			LOrgName.Text = Order.Seller;

													// Дата доставки
			if (Order.DeliverOn != null)
				DtpDeliverOn.Value = (DateTime)Order.DeliverOn;

			else
				DtpDeliverOn.Value = DateTime.Now;

													// Заполнение КБ адресов
			FillAddressComboBox();

													// Адрес доставки


			if (Order.AddressId != null)
				CbDeliverAddress.SelectedValue = (int)Order.AddressId;



			#region Настройка списка и пагинатора

			EdProductList.ProductAddStrategy = EditableProductAddStrategy;

			ProductListAdapter = Order.GetProducts();
			
			PagOrderContent.PageCount = ProductListAdapter.GetPageCount();

			RefreshEditableProductList(1);

			PagOrderContent.PageChanged += delegate(Object s, EventArgs args) {
				RefreshEditableProductList(PagOrderContent.ChoosedPage);
			};

			#endregion

		}


		#region Список товаров

		#region Список для чтения
		
													// Алгоритм выгрузки данных в элементы списка товаров (только для чтения)
		private ProductPanel ReadOnlyProductAddStrategy(ProductPanel prodPanel, DataRow row) {
		
			prodPanel.ProductId = Convert.ToInt32(row["product_id"]);
			prodPanel.LName.Text = row["product_name"].ToString();

			string MeasureUnit = row["product_measure_unit"].ToString();

			prodPanel.LQuantityString.Text = "Заказано: " + row["quantity"].ToString() + " " + MeasureUnit;

			prodPanel.LPriceString.Text = "Цена за 1 " + MeasureUnit + " ~ " + row["product_price"].ToString() + " руб.";

			prodPanel.LSum.Text = "Общая сумма: " + row["pos_sum"].ToString() + " руб.";

			return prodPanel;

		}

													// Обновление списка только для чтения
		private void RefreshReadOnlyProductList(int page) {
			
			RoProductList.Controls.Clear();

			RoProductList.DataSource = ProductListAdapter.GetData(page);

		}


		#endregion

		#region Список для редактирования

													// Алгоритм выгрузки данных в элементы списка товаров (для редактирования)
		private EditableProductPanel EditableProductAddStrategy(EditableProductPanel prodPanel, DataRow row) {
		
			prodPanel.ProductId = Convert.ToInt32(row["product_id"]);
			prodPanel.Price = Convert.ToInt32(row["price"]);
			prodPanel.MeasureUnit = row["mu_name"].ToString();
			prodPanel.LName.Text = row["name"].ToString();
			prodPanel.TbQuantity.Text = row["quantity"].ToString();
			prodPanel.LSum.Text = row["pos_sum"].ToString() + " руб.";

			return prodPanel;

		}

													// Обновление списка для редактирования
		private void RefreshEditableProductList(int page) {
			
			EdProductList.Controls.Clear();

			EdProductList.DataSource = ProductListAdapter.GetData(page);

		}

		#endregion

		#region Список администратора
		
													// Алгоритм выгрузки данных "editing" заказоа для администратора
		private ProductPanel AdminEditableProductAddStrategy(ProductPanel prodPanel, DataRow row) {
		
			prodPanel.ProductId = Convert.ToInt32(row["product_id"]);
			prodPanel.LName.Text = row["name"].ToString();

			string MeasureUnit =row["mu_name"].ToString();

			prodPanel.LQuantityString.Text = "Заказано: " + row["quantity"].ToString() + " " + MeasureUnit;
			prodPanel.LPriceString.Text = "Цена за 1 " + MeasureUnit + " ~ " + row["price"].ToString() + " руб.";
			prodPanel.LSum.Text = "Общая сумма: " + row["pos_sum"].ToString() + " руб.";

			return prodPanel;

		}

													// Алгоритм выгрузки данных остальных заказов для администратора
		private ProductPanel AdminReadOnlyProductAddStrategy(ProductPanel prodPanel, DataRow row) {
		
			prodPanel.ProductId = Convert.ToInt32(row["product_id"]);
			prodPanel.LName.Text = row["product_name"].ToString();

			string MeasureUnit = row["product_measure_unit"].ToString();

			prodPanel.LQuantityString.Text = "Заказано: " + row["quantity"].ToString() + " " + MeasureUnit;
			prodPanel.LPriceString.Text = "Цена за 1 " + MeasureUnit + " ~ " + row["product_price"].ToString() + " руб.";
			prodPanel.LSum.Text = "Общая сумма: " + row["pos_sum"].ToString() + " руб.";

			return prodPanel;

		}

													// Обновление списка для редактирования
		private void RefreshAdminProductList(int page) {
			
			RoProductList.Controls.Clear();

			RoProductList.DataSource = ProductListAdapter.GetData(page);

		}


		#endregion

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

		}

													// События входящих заказов "в процессе оформления"
		private void EditingOutEventSetUp() {
		
			BSave.Click += OrderSaveButtonClick;	// Сохранение изменений
													// Отмена изменений 
			BCancel.Click += OrderCancelChangesButtonClick;

													// Удаление товара
			EdProductList.ProductDeleted += ProductDeleteButtonClick; 

													// Удаление заказа
			BDelete.Click += DeleteOrderButtonClick;

													// Изменение количества товара
			EdProductList.ProductSaved += ProductSaveButtonClick;

													// Отправка заказа
			BSend.Click += SendOrderButtonClick;


		}

													// События исходящих заказов "отправлен на рассмотрение"
		private void WaitingOutEventSetUp() {
		
			BCancelOrder.Click += CancelOrderButtonClick;

		}

													// События входящих заказов "отправлен на рассмотрение"
		private void WaitingIncEventSetUp() {
			
			BAccept.Click += AcceptOrderButtonClick;
			BDecline.Click += DeclineOrderButtonClick;

		}
		
													// События входящих заказов "принят"
		private void AcceptedIncEventSetUp() {
		
			BDelivered.Click += MarkAsDeliveredOrderButtonClick;

		}

		#region Изменение заказов

													// Отправка изменений заказа
		private bool OrderUpdated() {
		
													// Проверка статуса заказа
			if (!Order.StatusIsSame()) {
				MessageBox.Show("Статус заказа не позволяет ему получить изменения, возможно статус был изменен другим работником");
				new DynOrderContentForm().Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
				return false;
			}


			DateTime DeliverOn = DtpDeliverOn.Value;

													// Проверка даты доставки
			if (DeliverOn.Date < DateTime.Now.Date) {
				MessageBox.Show("Вы не можете заказывать в прошлое");
				return false;
			}

			int SelectedAddressId;
			
													// Если введено новое значение
			if (CbDeliverAddress.SelectedValue == null) {

				string CustomAddress = CbDeliverAddress.Text;

													// Проверка на пустоту
				if (CustomAddress.Length == 0 ) {
					MessageBox.Show("Введите адрес");
					return false;
				}

													// Подтверждение пользователя
				DialogResult UserAnswer = MessageBox.Show(
					"Адрес \"" + CustomAddress + "\" отсутствует в справочнике организации, желаете его добавить?", 
					"Подтвердите действие",
					MessageBoxButtons.YesNo
				);
				
													// В случае отказа
				if (UserAnswer == DialogResult.No) {
					MessageBox.Show("Сохранение заказа отменено");
					return false;
				} 

													// Проверка на длину
				if (CustomAddress.Length > 50 ) {
					MessageBox.Show("Вы ввели слишком длинный адрес");
					return false;
				}

													// В случае соглашения
				MySqlCommand Query = programForm.Conn.CreateCommand();
				Query.CommandText = 
					"insert into address (organization_id, name) values (@org_id, @name)";

				Query.Parameters.Add("org_id", MySqlDbType.Int32).Value = programForm.User.Organization.Id;
				Query.Parameters.Add("name", MySqlDbType.VarChar).Value = CustomAddress;

													// Добавление адреса в справочник
				programForm.Conn.Open();

				Query.ExecuteNonQuery();

													// Получение его ид
				SelectedAddressId = QueryUtils.GetLastInsertedId(programForm.Conn);

				programForm.Conn.Close();

				FillAddressComboBox();

			} else 
				SelectedAddressId = Convert.ToInt32(CbDeliverAddress.SelectedValue);


			Order.Update(SelectedAddressId, DeliverOn);

			return true;

		}

													// Кнопка сохранения изменений заказа
		private void OrderSaveButtonClick(Object s, EventArgs e) {

			if (OrderUpdated()) {
				
				this.Generate(programForm, this.Order.Id, programForm.User.Organization.Id);

				MessageBox.Show("Заказ был успешно изменен");

			}

		} 

													// Кнопка отмены изменений заказа
		private void OrderCancelChangesButtonClick(Object s, EventArgs e) {
			
			this.FillEditableForm();

		}

													// Кнопка удаления товара из заказа
		private void ProductDeleteButtonClick(Object s, EventArgs e) {
		
													// Проверка статуса заказа
			if (!Order.StatusIsSame()) {
				MessageBox.Show("Статус заказа не позволяет ему получить изменения, возможно статус был изменен другим работником");
				new DynOrderContentForm().Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
				return;
			}
													// Получение ид товара
			int ProductId = ((EditableProductPanel)s).ProductId;

			MySqlCommand Query = programForm.Conn.CreateCommand();
			Query.CommandText = "delete from order_content where order_id = @ord_id and product_id = @p_id";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = this.Order.Id;
			Query.Parameters.Add("p_id", MySqlDbType.Int32).Value = ProductId;

													// Удаление
			programForm.Conn.Open();

			Query.ExecuteNonQuery();
			
			programForm.Conn.Close();

			this.RefreshEditableProductList(1);
			this.PagOrderContent.PageCount = ProductListAdapter.GetPageCount();
			
			this.Order.UpdateOrderSum();
			LSum.Text = Order.OrderSum + " руб.";
		}

													// Кнопка сохранения нового количества продукта в заказе
		private void ProductSaveButtonClick(Object s, EventArgs e) {

													// Проверка статуса заказа
			if (!Order.StatusIsSame()) {
				MessageBox.Show("Статус заказа не позволяет ему получить изменения, возможно статус был изменен другим работником");
				new DynOrderContentForm().Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
				return;
			}

			EditableProductPanel EdPanel = (EditableProductPanel)s;

			int Quantity;

													// Проверка количества
			if (!QueryUtils.CheckNum(EdPanel.TbQuantity.Text.Trim(), out Quantity, 0, QueryUtils.MaxQuantity)) {
				MessageBox.Show("Количество недопустимо, оно должно содержать значение в диапазоне от 1 до " + QueryUtils.MaxQuantity.ToString());
				return;
			}

													// Проверка наличия такого количества

													// Получение количества из БД
			MySqlCommand Query = programForm.Conn.CreateCommand();
			Query.CommandText = "select quantity from balance where product_id = @p_id";

			Query.Parameters.Add("p_id", MySqlDbType.VarChar).Value = EdPanel.ProductId;

			programForm.Conn.Open();

			int QuantityInStock = Convert.ToInt32(Query.ExecuteScalar());

			programForm.Conn.Close();


			if (Quantity > QuantityInStock) {
				MessageBox.Show("У поставщика нет товара в введенном количестве, в наличии: " + QuantityInStock.ToString());
				return;
			}

													// Обновление количества
			Query = programForm.Conn.CreateCommand();
			Query.CommandText = "update order_content set quantity = @quant where order_id = @ord_id and product_id = @p_id";

			Query.Parameters.Add("ord_id", MySqlDbType.Int32).Value = this.Order.Id;
			Query.Parameters.Add("p_id", MySqlDbType.Int32).Value = EdPanel.ProductId;
			Query.Parameters.Add("quant", MySqlDbType.Int32).Value = Quantity;

													// Обновление
			programForm.Conn.Open();

			Query.ExecuteNonQuery();
			
			programForm.Conn.Close();

			this.Order.UpdateOrderSum();
			LSum.Text = Order.OrderSum + " руб.";

			RefreshEditableProductList(PagOrderContent.ChoosedPage);

		}

													// Кнопка удаления заказа 
		private void DeleteOrderButtonClick(Object s, EventArgs e) {
		
													// Проверка статуса заказа
			if (!Order.StatusIsSame()) {
				MessageBox.Show("Статус заказа не позволяет ему получить изменения, возможно статус был изменен другим работником");
				new DynOrderContentForm().Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
				return;
			}

			this.Order.Delete();

			programForm.History.Pop().RegenerateOldForm();

		}

		#endregion

		#endregion


		#region Изменение статусов заказов
		
													// Отправка заказа
		private void SendOrderButtonClick(Object s, EventArgs e) {

													// Отправка даты доставки и адреса доставки
			if (!this.OrderUpdated())
				return;

													// Проверка на пустой заказ
			if (Order.IsEmpty()) {
				MessageBox.Show("Заказ пуст");
				return;
			}
				

			Order.Send();

			this.Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
			
			MessageBox.Show("Заказ отправлен");

		}

													// Отмена заказа
		private void CancelOrderButtonClick(Object s, EventArgs e) {
			
													// Проверка статуса заказа
			if (!Order.StatusIsSame()) {
				MessageBox.Show("Статус заказа не позволяет ему получить изменения, возможно статус был изменен другим работником");
				new DynOrderContentForm().Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
				return;
			}

													// Изменение статуса заказа
			Order.Cancel(); 

			this.Generate(programForm, this.Order.Id, programForm.User.Organization.Id);

			MessageBox.Show("Заказ отменен");
		}

													// Принятие заказа
		private void AcceptOrderButtonClick(Object s, EventArgs e) {
			
			if (!Order.StatusIsSame()) {
				MessageBox.Show("Статус заказа не позволяет ему получить изменения, возможно статус был изменен другим работником");
				new DynOrderContentForm().Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
				return;
			}

			

			Order.Accept(programForm.User.Id);

			this.Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
			
			MessageBox.Show("Заказ принят");

		}

													// Отклонение заказа
		private void DeclineOrderButtonClick(Object s, EventArgs e) {
			
													// Проверка статуса																
			if (!Order.StatusIsSame()) {
				MessageBox.Show("Статус заказа не позволяет ему получить изменения, возможно статус был изменен другим работником");
				new DynOrderContentForm().Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
				return;
			}

			Order.Decline();

			this.Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
			
			MessageBox.Show("Заказ отклонен");

		}

													// Пометить доставленным
		private void MarkAsDeliveredOrderButtonClick(Object s, EventArgs e) {
			
			if (!Order.StatusIsSame()) {
				MessageBox.Show("Статус заказа не позволяет ему получить изменения, возможно статус был изменен другим работником");
				new DynOrderContentForm().Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
				return;
			}

			Order.MarkAsDelivered();

			this.Generate(programForm, this.Order.Id, programForm.User.Organization.Id);
			
			MessageBox.Show("Заказ был помечен как доставленный");

		}

		#endregion

	}
}
