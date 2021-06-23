using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;

using Skladik.Utils;
using Skladik.Adapters;
using Skladik.NewComponents;


namespace Skladik.Forms
{
	public class DynProductForm : DynForm
	{

		const TableLayoutPanelCellBorderStyle BorderStyle = TableLayoutPanelCellBorderStyle.Inset;

		#region Объявления

		// Адаптер формы
		private ProductDataAdapter Product;

		// Кнопки перехода
		public Button BGoBack { get; private set; }
		public Button BGoMain { get; private set; }

		// Картинка продукта
		public PictureBox ProductImg { get; private set; }

		// Информация о поставщике
		public PictureBox PbOrgAvatar { get; private set; }
		public Label LOrgName { get; private set; }
		public Label LOrgPhone { get; private set; }
		public Label LOrgEmail { get; private set; }

		// Даты добавления и модификации
		public Label LAddDate { get; private set; }
		public Label LModifiedDate { get; private set; }

		// Название и описание продукта readonly
		public Label LProductName { get; private set; }

		// Название и описание продукта writable
		public TextBox TbProductName { get; private set; }

		public RichTextBox RtbAbout { get; private set; }

		// Цена и количество readonly
		public Label LPrice { get; private set; }
		public Label LQuantity { get; private set; }

		// Цена и количество readonly writable
		public TextBox TbPrice { get; private set; }
		public ComboBox CbMeasureUnits { get; private set; }
		public TextBox TbQuantity { get; private set; }

		// Элементы панели добавления к заказу
		public TextBox TbOrderQuantity { get; private set; }
		public Label LMeasureUnit { get; private set; }
		public Label LSum { get; private set; }
		public Button BAddToOrder { get; private set; }

		// Элементы панели изменения/добавления продукта
		public OpenImageString OiString { get; private set; }
		public ComboBox CbCategory { get; private set; }

		// Элементы панели изменения продукта
		public Button BSave { get; private set; }
		public Button BCancel { get; private set; }
		public Button BDelete { get; private set; }

		// Элементы панели добавления продукта
		public Button BAddProduct { get; private set; }
		public Button BCancelAddProduct { get; private set; }

		#endregion

		// Настройка формы
		protected override void SetUpMainForm()
		{
			programForm.Controls.Clear();
			Size FormSize = new Size(1000, 600);
			programForm.MinimumSize = FormSize;
			programForm.MaximumSize = new Size(1500, 800); ;
			programForm.Location = new Point(10, 100);
			programForm.Size = FormSize;
			programForm.Text = "Просмотр товара";
		}

		// Форма добавления нового товара
		public override void Generate(Form1 aForm)
		{
			base.Generate(aForm);

			Product = new ProductDataAdapter(programForm.Conn);

			formContent = CreateAttachedUserInsertForm();
			FillInsertForm();
			InsertFormEventSetUp();

			programForm.Controls.Add(formContent);

		}

		// Формы открытия товара
		public void Generate(Form1 aForm, int productId)
		{

			base.Generate(aForm);

			Product = new ProductDataAdapter(programForm.Conn);

			// Попытка выгрузки данных
			if (!Product.GetData(productId))
			{
				MessageBox.Show("Произошла ошибка при открытии товара, Вы будете перенаправлены на предыдущую форму");
				programForm.History.Pop().RegenerateOldForm();
				return;
			}


			if (programForm.User.Role == "admin")
			{
				// Администраторская форма
				formContent = CreateAttachedUserUpdateForm();
				FillWritableForm();
				EditFormEventSetUp();


			}
			else if (programForm.User.Organization == null)
			{
				// Форма непривязанного пользователя
				formContent = CreateDetachedUserForm();
				FillReadOnlyForm();
				DetachedUserViewSetUp();

			}
			else if (programForm.User.Organization.Id == Product.Seller.Id)
			{
				// Форма изменения привязанного пользователя
				formContent = CreateAttachedUserUpdateForm();
				FillWritableForm();
				EditFormEventSetUp();

			}
			else
			{
				// Форма привязанного пользователя для
				// чужих товаров
				formContent = CreateAttachedUserForeignForm();
				FillReadOnlyForm();
				OrderFormEventSetUp();

			}

			programForm.Controls.Add(formContent);

		}

		#region Рамки
		// Основная рамка программы
		private TableLayoutPanel CreateMainFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Result.CellBorderStyle = BorderStyle;
			Result.Dock = DockStyle.Fill;

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
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Нижняя часть
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 60));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));


			return Result;

		}

		// Рамка для информации о продукте
		private TableLayoutPanel CreateProductInfoFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, BorderStyle);

			Result.ColumnCount = 1;
			Result.RowCount = 3;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			return Result;

		}

		// Создание двустрочной рамки для дат или цен
		private TableLayoutPanel CreateDoubleRowedForLabelFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);
			Result.Padding = new Padding(10, 0, 0, 0);

			#region Таблица
			Result.ColumnCount = 1;
			Result.RowCount = 2;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

			// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

			#endregion

			return Result;

		}

		#endregion


		#region Части формы

		// Кнопки перехода		
		private TableLayoutPanel CreateFormHeader()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			Styles.ContentFramesStyle(Result, BorderStyle);

			Result.ColumnCount = 3;
			Result.RowCount = 1;

			// Кнопка
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
			// Кнопка
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
			// Пустое место
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			BGoBack = new Button();
			BGoBack.Text = "<";
			BGoBack.Font = new Font("Helvetica", 8);
			BGoBack.Dock = DockStyle.Fill;
			BGoBack.Margin = new Padding(0);

			BGoMain = new Button();
			BGoMain.Text = "Главная";
			BGoMain.Font = new Font("Helvetica", 8);
			BGoMain.Dock = DockStyle.Fill;
			BGoMain.Margin = new Padding(0);

			Result.Controls.Add(BGoBack, 0, 0);
			Result.Controls.Add(BGoMain, 1, 0);

			return Result;

		}

		// Верхняя часть формы
		private TableLayoutPanel CreateTopPartContent(TableLayoutPanel ProductInfo)
		{

			TableLayoutPanel TopPartContent = new TableLayoutPanel();

			Styles.ContentFramesStyle(TopPartContent, BorderStyle);

			TopPartContent.ColumnCount = 3;
			TopPartContent.RowCount = 1;

			TopPartContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
			TopPartContent.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			TopPartContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));

			TopPartContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			// Картинка 
			ProductImg = new PictureBox();
			Styles.ImgStyle(ProductImg);

			// Добавление содержимого верхней части
			TopPartContent.Controls.Add(ProductImg, 0, 0);
			TopPartContent.Controls.Add(ProductInfo, 2, 0);

			return TopPartContent;

		}



		// Информация о продукте только для чтения
		private TableLayoutPanel ReadOnlyProductInfo()
		{

			TableLayoutPanel Result = CreateProductInfoFrame();

			// Название продукта
			LProductName = new Label();
			Styles.TextStyle(LProductName, 15);
			LProductName.Margin = new Padding(5, 10, 5, 10);

			// Описание продукта
			RtbAbout = new RichTextBox();
			Styles.RichTextBoxStyle(RtbAbout);
			RtbAbout.ReadOnly = true;

			Result.Controls.Add(LProductName, 0, 0);
			Result.Controls.Add(RtbAbout, 0, 2);

			return Result;

		}


		private TableLayoutPanel WritableProductInfo()
		{

			TableLayoutPanel Result = CreateProductInfoFrame();

			// Название продукта
			TbProductName = new TextBox();
			Styles.TextBoxStyle(TbProductName);

			// Описание продукта
			RtbAbout = new RichTextBox();
			Styles.RichTextBoxStyle(RtbAbout);

			Result.Controls.Add(TbProductName, 0, 0);
			Result.Controls.Add(RtbAbout, 0, 2);

			return Result;

		}

		// Нижняя часть формы
		private TableLayoutPanel CreateBottomPartContent(TableLayoutPanel PriceFrame, TableLayoutPanel CartFrame)
		{

			#region Рамка нижней части
			// Содержимое нижней части формы
			TableLayoutPanel BottomPartContent = new TableLayoutPanel();

			Styles.ContentFramesStyle(BottomPartContent, BorderStyle);

			BottomPartContent.ColumnCount = 3;
			BottomPartContent.RowCount = 1;

			// Организация и даты
			BottomPartContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

			// Промежуток
			BottomPartContent.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

			// Правая часть
			BottomPartContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

			BottomPartContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Рамка левой ниженей части
			// Содержимое нижней левой части формы
			TableLayoutPanel OrgAndDatesFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(OrgAndDatesFrame, BorderStyle);

			OrgAndDatesFrame.ColumnCount = 1;
			OrgAndDatesFrame.RowCount = 3;

			OrgAndDatesFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Организация
			OrgAndDatesFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 80));

			// Промежуток
			OrgAndDatesFrame.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Даты
			OrgAndDatesFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

			OrgAndDatesFrame.Controls.Add(CreateOrgContent(), 0, 0);
			OrgAndDatesFrame.Controls.Add(CreateDatesContent(), 0, 2);

			#endregion

			#region Рамка правой нижней части

			// Содержимое нижней правой части формы
			TableLayoutPanel PriceAndCartFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(PriceAndCartFrame, BorderStyle);

			PriceAndCartFrame.ColumnCount = 1;
			PriceAndCartFrame.RowCount = 3;

			PriceAndCartFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Организация
			PriceAndCartFrame.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

			// Промежуток
			PriceAndCartFrame.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Даты
			PriceAndCartFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			PriceAndCartFrame.Controls.Add(PriceFrame, 0, 0);
			if (CartFrame != null)
				PriceAndCartFrame.Controls.Add(CartFrame, 0, 2);

			#endregion

			BottomPartContent.Controls.Add(OrgAndDatesFrame, 0, 0);
			BottomPartContent.Controls.Add(PriceAndCartFrame, 2, 0);

			return BottomPartContent;

		}




		#region Левая часть нижней части формы
		// Содержимое панели с информацией о организации
		private TableLayoutPanel CreateOrgContent()
		{

			TableLayoutPanel Result = new TableLayoutPanel();

			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			#region Таблица
			Result.ColumnCount = 3;
			Result.RowCount = 7;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Картинка текст
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			#endregion

			#region Компоненты

			// Постоянный текст "Поставщик"
			Label LSellerText = new Label();
			LSellerText.Text = "Поставщик:";
			Styles.TextStyle(LSellerText, 15);
			LSellerText.Margin = new Padding(0, 0, 0, 10);

			#region Картинка и название организации
			// Контейнер картинки и названия оргизации
			TableLayoutPanel OrgAvatarAndName = new TableLayoutPanel();
			Styles.ContentFramesStyle(OrgAvatarAndName, TableLayoutPanelCellBorderStyle.None);
			OrgAvatarAndName.ColumnCount = 2;
			OrgAvatarAndName.RowCount = 1;
			OrgAvatarAndName.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
			OrgAvatarAndName.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));

			OrgAvatarAndName.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			// Картинка организации
			PbOrgAvatar = new PictureBox();
			Styles.ImgStyle(PbOrgAvatar);
			PbOrgAvatar.Margin = new Padding(10, 0, 10, 0);

			// Название организации
			LOrgName = new Label();
			Styles.TextStyle(LOrgName);
			LOrgName.Anchor = AnchorStyles.Left;

			#endregion

			LOrgPhone = new Label();
			Styles.TextStyle(LOrgPhone);

			LOrgEmail = new Label();
			Styles.TextStyle(LOrgEmail);

			// Добавление элементов в контейнер
			OrgAvatarAndName.Controls.Add(PbOrgAvatar, 0, 0);
			OrgAvatarAndName.Controls.Add(LOrgName, 1, 0);


			Result.Controls.Add(LSellerText, 1, 1);
			Result.Controls.Add(OrgAvatarAndName, 1, 2);
			Result.Controls.Add(LOrgPhone, 1, 4);
			Result.Controls.Add(LOrgEmail, 1, 5);

			#endregion

			return Result;

		}

		// Содержимое панель дат
		private TableLayoutPanel CreateDatesContent()
		{

			TableLayoutPanel Result = CreateDoubleRowedForLabelFrame();

			LAddDate = new Label();
			Styles.TextStyle(LAddDate);
			LAddDate.Anchor = AnchorStyles.Left;

			LModifiedDate = new Label();
			Styles.TextStyle(LModifiedDate);
			LModifiedDate.Anchor = AnchorStyles.Left;

			Result.Controls.Add(LAddDate, 0, 0);
			Result.Controls.Add(LModifiedDate, 0, 1);

			return Result;

		}

		#endregion

		#region Правая часть нижней части формы

		// Цена товара только для просмотра
		private TableLayoutPanel ReadOnlyPriceFrame()
		{

			TableLayoutPanel Result = CreateDoubleRowedForLabelFrame();

			LPrice = new Label();
			Styles.TextStyle(LPrice);
			LPrice.Anchor = AnchorStyles.Left;

			LQuantity = new Label();
			Styles.TextStyle(LQuantity);
			LQuantity.Anchor = AnchorStyles.Left;

			Result.Controls.Add(LPrice, 0, 0);
			Result.Controls.Add(LQuantity, 0, 1);

			return Result;

		}


		// Цена товара для редактирования
		private TableLayoutPanel WritablePriceFrame()
		{

			TableLayoutPanel Result = CreateDoubleRowedForLabelFrame();

			#region Всё касаемо цены (1 строки панели)

			TableLayoutPanel PriceFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(PriceFrame, TableLayoutPanelCellBorderStyle.None);

			PriceFrame.ColumnCount = 5;
			PriceFrame.RowCount = 1;

			// "Цена: "
			PriceFrame.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

			// Поле
			PriceFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			// " руб. ~ 1 "
			PriceFrame.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

			// Комбобокс
			PriceFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			// Пустой промежуток
			PriceFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			PriceFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			// Текст
			Label LPriceText = new Label();
			Styles.TextStyle(LPriceText);
			LPriceText.Text = "Цена:";
			LPriceText.Anchor = AnchorStyles.Left;

			// Поле ввода
			TbPrice = new TextBox();
			Styles.TextBoxStyle(TbPrice);

			// Текст
			Label LAfterText = new Label();
			Styles.TextStyle(LAfterText);
			LAfterText.Text = "руб. ~ 1";
			LAfterText.Anchor = AnchorStyles.Left;

			// Комбобокс
			CbMeasureUnits = new ComboBox();
			Styles.ComboBoxStyle(CbMeasureUnits);

			PriceFrame.Controls.Add(LPriceText, 0, 0);
			PriceFrame.Controls.Add(TbPrice, 1, 0);
			PriceFrame.Controls.Add(LAfterText, 2, 0);
			PriceFrame.Controls.Add(CbMeasureUnits, 3, 0);

			#endregion

			#region Всё касаемо количества

			TableLayoutPanel QuantityFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(QuantityFrame, TableLayoutPanelCellBorderStyle.None);

			QuantityFrame.ColumnCount = 3;
			QuantityFrame.RowCount = 1;

			// "В наличии: "
			QuantityFrame.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

			// Поле
			QuantityFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			// Промежуток
			QuantityFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));


			QuantityFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			Label LQuantityText = new Label();
			Styles.TextStyle(LQuantityText);
			LQuantityText.Text = "В наличии:";
			LQuantityText.Anchor = AnchorStyles.Left;

			TbQuantity = new TextBox();
			Styles.TextBoxStyle(TbQuantity);


			QuantityFrame.Controls.Add(LQuantityText);
			QuantityFrame.Controls.Add(TbQuantity);

			#endregion

			Result.Controls.Add(PriceFrame, 0, 0);
			Result.Controls.Add(QuantityFrame, 0, 1);

			return Result;

		}

		// Панель добавления в заказ
		private TableLayoutPanel CartBottomFrame()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			#region Таблица

			Result.ColumnCount = 3;
			Result.RowCount = 9;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Текст
			Result.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Поле ввода количества
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Вывод суммы
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Кнопка добавления
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Компоненты

			// "Добавить к заказу:"
			Label LAddToOrderText = new Label();
			LAddToOrderText.Text = "Добавить к заказу:";
			Styles.TextStyle(LAddToOrderText, 15);
			LAddToOrderText.Margin = new Padding(0, 0, 0, 10);

			// Поле ввода
			#region Ввод количества

			TableLayoutPanel InputQuantFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(InputQuantFrame, TableLayoutPanelCellBorderStyle.None);

			InputQuantFrame.ColumnCount = 2;
			InputQuantFrame.RowCount = 1;

			InputQuantFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
			InputQuantFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			InputQuantFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			TbOrderQuantity = new TextBox();
			Styles.TextBoxStyle(TbOrderQuantity);

			LMeasureUnit = new Label();
			Styles.TextStyle(LMeasureUnit);
			LMeasureUnit.Anchor = AnchorStyles.Left;

			InputQuantFrame.Controls.Add(TbOrderQuantity, 0, 0);
			InputQuantFrame.Controls.Add(LMeasureUnit, 1, 0);

			#endregion

			// Вывод суммы
			#region Вывод суммы

			TableLayoutPanel SumFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(SumFrame, TableLayoutPanelCellBorderStyle.None);
			// TableLayoutPanelCellBorderStyle.None
			SumFrame.ColumnCount = 2;
			SumFrame.RowCount = 1;

			SumFrame.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			SumFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			SumFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			// Надпись
			Label LSumText = new Label();
			Styles.TextStyle(LSumText);
			LSumText.Text = "Сумма:";
			LSumText.Anchor = AnchorStyles.Left;

			// Вывод саомй суммы
			LSum = new Label();
			Styles.TextStyle(LSum);
			LSum.Anchor = AnchorStyles.Left;
			LSum.Text = "";

			SumFrame.Controls.Add(LSumText, 0, 0);
			SumFrame.Controls.Add(LSum, 1, 0);

			#endregion

			// Кнопка добавления
			BAddToOrder = new Button();
			BAddToOrder.Size = new Size(100, 25);
			BAddToOrder.Font = Styles.TextFont;
			BAddToOrder.Text = "Добавить";
			BAddToOrder.Margin = new Padding(10, 0, 0, 0);

			Result.Controls.Add(LAddToOrderText, 1, 1);
			Result.Controls.Add(InputQuantFrame, 1, 3);
			Result.Controls.Add(SumFrame, 1, 5);
			Result.Controls.Add(BAddToOrder, 1, 7);

			#endregion

			return Result;

		}

		// Панель изменения товара
		private TableLayoutPanel WritableBottomFrame(TableLayoutPanel buttons)
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			#region Рамка

			Result.ColumnCount = 3;
			Result.RowCount = 7;

			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			// Выбор картинки
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			// Выбор категории
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			// Кнопки
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

			// Промежуток
			Result.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

			#endregion

			#region Компоненты
			// Компонент для открытия изображения
			OiString = new OpenImageString();
			Styles.TextStyle(OiString.LComment);
			OiString.LComment.Text = "Изображение товара: ";
			OiString.BOpenImg.Font = Styles.TextFont;

			// Выбор категории товаров
			#region Категория товаров

			TableLayoutPanel CategoryFrame = new TableLayoutPanel();
			Styles.ContentFramesStyle(CategoryFrame, TableLayoutPanelCellBorderStyle.None);

			CategoryFrame.ColumnCount = 3;
			CategoryFrame.RowCount = 1;

			CategoryFrame.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			CategoryFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
			CategoryFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			CategoryFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			// Текст "Категория товаров: "
			Label LCategoryText = new Label();
			Styles.TextStyle(LCategoryText);
			LCategoryText.Text = "Категория товара:";
			LCategoryText.Anchor = AnchorStyles.Left;
			LCategoryText.Margin = new Padding(0);

			// Выбор категории
			CbCategory = new ComboBox();
			Styles.ComboBoxStyle(CbCategory);

			CategoryFrame.Controls.Add(LCategoryText, 0, 0);
			CategoryFrame.Controls.Add(CbCategory, 1, 0);

			#endregion

			Result.Controls.Add(OiString, 1, 1);
			Result.Controls.Add(CategoryFrame, 1, 3);

			#endregion

			// Добавление кнопок
			Result.Controls.Add(buttons, 1, 5);

			return Result;

		}

		// Сохранть, отменить и удалить товар для панели изменения товара
		private TableLayoutPanel ProductUpdateButtons()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			#region Рамка

			Result.ColumnCount = 4;
			Result.RowCount = 1;

			// Промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Кнопка
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			// Кнопка
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			// Кнопка
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Кнопки

			// Кнопки управления изменяемым товаром
			BSave = new Button();
			Styles.ButtonStyle(BSave);
			BSave.Text = "Сохранить изменения";
			BSave.Margin = new Padding(2);

			BCancel = new Button();
			Styles.ButtonStyle(BCancel);
			BCancel.Text = "Отменить изменения";
			BCancel.Margin = new Padding(2);

			BDelete = new Button();
			Styles.ButtonStyle(BDelete);
			BDelete.Text = "Удалить товар";
			BDelete.Margin = new Padding(2);


			Result.Controls.Add(BSave, 1, 0);
			Result.Controls.Add(BCancel, 2, 0);
			Result.Controls.Add(BDelete, 3, 0);

			#endregion

			return Result;

		}

		// Добавить товар, отменить добавление для панели изменения товара
		private TableLayoutPanel ProductInsertButtons()
		{

			TableLayoutPanel Result = new TableLayoutPanel();
			Styles.ContentFramesStyle(Result, TableLayoutPanelCellBorderStyle.None);

			#region Рамка

			Result.ColumnCount = 3;
			Result.RowCount = 1;

			// Промежуток
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			// Кнопка
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			// Кнопка
			Result.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));


			Result.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			#endregion

			#region Кнопки

			// Кнопки управления изменяемым товаром
			BAddProduct = new Button();
			Styles.ButtonStyle(BAddProduct);
			BAddProduct.Text = "Добавить товар";
			BAddProduct.Margin = new Padding(2);

			BCancelAddProduct = new Button();
			Styles.ButtonStyle(BCancelAddProduct);
			BCancelAddProduct.Text = "Отменить добавление";
			BCancelAddProduct.Margin = new Padding(2);


			Result.Controls.Add(BAddProduct, 1, 0);
			Result.Controls.Add(BCancelAddProduct, 2, 0);

			#endregion

			return Result;

		}


		#endregion

		#endregion


		#region Формы

		// Создание формы - основы для остальных
		private TableLayoutPanel CreateMainForm(TableLayoutPanel ProductInfoFrame, TableLayoutPanel PriceFrame, TableLayoutPanel CartFrame)
		{

			TableLayoutPanel Foundation = CreateMainFrame();

			Foundation.Controls.Add(CreateFormHeader(), 1, 1);
			Foundation.Controls.Add(CreateTopPartContent(ProductInfoFrame), 1, 3);
			Foundation.Controls.Add(CreateBottomPartContent(PriceFrame, CartFrame), 1, 5);


			return Foundation;

		}

		// Создание формы для просмотра товаров 
		// от лица непривязанного пользователя
		private TableLayoutPanel CreateDetachedUserForm()
		{

			return CreateMainForm(
				ReadOnlyProductInfo(),
				ReadOnlyPriceFrame(),
				null
			);

		}

		// Создание формы для просмотра товаров 
		// от лица привязанного пользователя
		private TableLayoutPanel CreateAttachedUserForeignForm()
		{

			return CreateMainForm(
				ReadOnlyProductInfo(),
				ReadOnlyPriceFrame(),
				CartBottomFrame()
			);

		}

		// Создание формы для изменения товаров 
		// привязанными пользователями
		private TableLayoutPanel CreateAttachedUserUpdateForm()
		{

			return CreateMainForm(
				WritableProductInfo(),
				WritablePriceFrame(),
				WritableBottomFrame(ProductUpdateButtons())
			);

		}

		// Создание формы для добавления товаров 
		// привязанными пользователями
		private TableLayoutPanel CreateAttachedUserInsertForm()
		{

			return CreateMainForm(
				WritableProductInfo(),
				WritablePriceFrame(),
				WritableBottomFrame(ProductInsertButtons())
			);

		}

		#endregion


		#region Вывод данных

		// Заполнение комбобокса с единицами измерения
		private void FillMeasureUnitComboBox()
		{

			// Получение данных из БД
			MySqlCommand Query = programForm.Conn.CreateCommand();
			Query.CommandText = "select * from measure_unit";

			MySqlDataAdapter MeasureUnitAdapter = new MySqlDataAdapter(Query);

			DataSet DSet = new DataSet();

			MeasureUnitAdapter.Fill(DSet);
			// programForm.Conn.Close()

			// Указание источника CB
			CbMeasureUnits.DataSource = DSet.Tables[0];
			CbMeasureUnits.DisplayMember = "name";
			CbMeasureUnits.ValueMember = "id";
			CbMeasureUnits.BindingContext = new BindingContext();

		}

		// Заполнение комбобокса с категориями
		private void FillCategoryComboBox()
		{

			// Получение данных из БД
			MySqlCommand Query = programForm.Conn.CreateCommand();
			Query.CommandText = "select * from category";

			MySqlDataAdapter CategoryAdapter = new MySqlDataAdapter(Query);

			DataSet DSet = new DataSet();

			CategoryAdapter.Fill(DSet);
			// programForm.Conn.Close()

			// Указание источника CB
			CbCategory.DataSource = DSet.Tables[0];
			CbCategory.DisplayMember = "name";
			CbCategory.ValueMember = "id";
			CbCategory.BindingContext = new BindingContext();

		}

		// Заполнение данных о поставщике
		private void FillOrganizationAndDates()
		{
			PbOrgAvatar.BackgroundImage = Product.Seller.Img;
			LOrgName.Text = Product.Seller.Name;
			LOrgPhone.Text = "Телефон: " + Product.Seller.Phone;
			LOrgEmail.Text = "Эл. адрес: " + Product.Seller.Email;
		}

		// Заполнение формы только для чтения
		private void FillReadOnlyForm()
		{

			ProductImg.BackgroundImage = Product.Img;
			LProductName.Text = Product.Name;

			if (Product.About != null)
				RtbAbout.Text = Product.About;

			FillOrganizationAndDates();

			LPrice.Text = "Цена: " + Product.Price.ToString() + " руб. ~ 1 " + Product.MeasureUnit;
			LQuantity.Text = "В наличии: " + Product.Quantity.ToString() + " " + Product.MeasureUnit;

			LAddDate.Text = "Дата добавления: " + Product.AddedOn.ToShortDateString();
			LModifiedDate.Text = "Дата модификации: " + Product.ModifiedOn.ToShortDateString();

		}

		// Заполнение формы для изменения
		private void FillWritableForm()
		{

			ProductImg.BackgroundImage = Product.Img;
			TbProductName.Text = Product.Name;

			if (Product.About != null)
				RtbAbout.Text = Product.About;

			FillOrganizationAndDates();
			TbPrice.Text = Product.Price.ToString();
			FillMeasureUnitComboBox();
			CbMeasureUnits.SelectedValue = Product.MeasureUnitId;
			TbQuantity.Text = Product.Quantity.ToString();
			FillCategoryComboBox();
			CbCategory.SelectedValue = Product.CategoryId;

			LAddDate.Text = "Дата добавления: " + Product.AddedOn.ToShortDateString();
			LModifiedDate.Text = "Дата модификации: " + Product.ModifiedOn.ToShortDateString();
		}

		// Заполнение формы для добавления
		private void FillInsertForm()
		{

			ProductImg.BackgroundImage = Properties.Resources.no_image;
			TbProductName.Text = "";

			RtbAbout.Text = "";

			Product.Seller = programForm.User.Organization;

			FillOrganizationAndDates();

			TbPrice.Text = "0";
			FillMeasureUnitComboBox();
			TbQuantity.Text = "0";
			FillCategoryComboBox();

			LAddDate.Text = "Дата добавления: " + DateTime.Now.ToShortDateString();
			LModifiedDate.Text = "Дата модификации: " + DateTime.Now.ToShortDateString();

		}

		#endregion


		#region Другие события компонентов формы

		// После выбора изображения, 
		// её перенос в компонент
		private void AfterImageChoosed(Object s, EventArgs e)
		{

			if (QueryUtils.CheckImageWeight(OiString.FileName, 4200000))
			{
				ProductImg.BackgroundImage = Image.FromFile(OiString.FileName);

				Product.ImageChanged = true;

			}
			else
				MessageBox.Show("Размер изображения не должен превышать 4 МБ");

		}

		// Вычисление конечной суммы позиции
		// при изменении текста поля количества
		private void CalculateSumOnQuantityChange(Object s, EventArgs e)
		{

			// Проверка количества
			int Temp;
			if (QueryUtils.CheckNum(TbOrderQuantity.Text, out Temp, 0, QueryUtils.MaxQuantityInOrder))
				if (Temp > Product.Quantity)
					LSum.Text = "Недостаточно товара";
				else
					LSum.Text = (Product.Price * Temp).ToString() + " руб.";

			else
				LSum.Text = "";


		}

		// Открытие личного кабинета 
		// организации при нажатии на её картинку
		// или название
		private void OpenOrganizationAccount(Object s, EventArgs e)
		{

			// programForm.History.Push(this);
			MessageBox.Show("Переход к личному кабинету организации");

		}

		#endregion


		#region Установка событий

		// Общие для всех форм события					
		private void EventSetUp()
		{
			// Переход назад
			BGoBack.Click += ProductCancelButtonClick;

			// Переход на главн. форму
			BGoMain.Click += delegate (Object s, EventArgs args) {
				new DynProductsBandForm().Generate(programForm);
			};

		}

		// Открытие личного кабинета организации
		private void OrganizationAccountOpenSetUp()
		{

			PbOrgAvatar.Click += OpenOrganizationAccount;
			LOrgName.Click += OpenOrganizationAccount;

		}

		// События формы добавления
		private void InsertFormEventSetUp()
		{

			EventSetUp();

			// Перенос картинки
			OiString.AfterImgLoad += AfterImageChoosed;

			// Добавление товара
			BAddProduct.Click += ProductAddButtonClick;

			// Отмена добавления товара
			BCancelAddProduct.Click += ProductCancelButtonClick;

		}

		// События формы изменения товара
		private void EditFormEventSetUp()
		{

			EventSetUp();

			// Перенос картинки
			OiString.AfterImgLoad += AfterImageChoosed;

			// События кнопок управления товаром
			BSave.Click += SaveButtonClick;
			BCancel.Click += CancelButtonClick;
			BDelete.Click += DeleteButtonClick;

			OrganizationAccountOpenSetUp();

		}

		// События формы добавления товара к заказу
		private void OrderFormEventSetUp()
		{

			EventSetUp();

			// Вычисление суммы позиции при вводе 
			TbOrderQuantity.TextChanged += CalculateSumOnQuantityChange;

			// Добавление к заказу
			BAddToOrder.Click += AddToOrderButtonClick;

			OrganizationAccountOpenSetUp();

		}


		private void DetachedUserViewSetUp()
		{

			EventSetUp();
			OrganizationAccountOpenSetUp();

		}

		#endregion


		#region Управление товарами

		#region Добавление товара

		// Добавление товара					
		private void ProductAddButtonClick(Object s, EventArgs e)
		{

			string Name = TbProductName.Text.Trim();
			string About = RtbAbout.Text.Trim();
			Image Img = ProductImg.BackgroundImage;

			int Price;
			int Quantity;


			// Проверка данных

			// Проверка названия
			if (!QueryUtils.CheckName(Name))
			{
				MessageBox.Show("Название должно содержать от 4 до 50 букв");
				return;
			}

			// Проверка цены
			if (!QueryUtils.CheckNum(TbPrice.Text.Trim(), out Price, 0, QueryUtils.MaxPrice))
			{
				MessageBox.Show("Цена недопустима, она должна содержать значение в диапазоне от 1 до " + QueryUtils.MaxPrice.ToString());
				return;
			}

			// Проверка цены
			if (!QueryUtils.CheckNum(TbQuantity.Text.Trim(), out Quantity, 0, QueryUtils.MaxQuantity))
			{
				MessageBox.Show("Количество недопустимо, оно должно содержать значение в диапазоне от 1 до " + QueryUtils.MaxQuantity.ToString());
				return;
			}

			// Проверка существования единицы измерения
			if (CbMeasureUnits.SelectedValue == null)
			{
				MessageBox.Show("Выбранная единица измерения не существует");
				return;
			}

			int MeasureUnitId = Convert.ToInt32(CbMeasureUnits.SelectedValue);

			// Проверка существования категории
			if (CbCategory.SelectedValue == null)
			{
				MessageBox.Show("Выбранная категория не существует");
				return;
			}

			int CategoryId = Convert.ToInt32(CbCategory.SelectedValue);


			// Отправка запроса добавления товара
			int Id = Product.Insert(
				programForm.User.Id,
				programForm.User.Organization.Id,
				Name,
				About,
				Price,
				MeasureUnitId,
				Quantity,
				CategoryId,
				Img
			);

			new DynProductForm().Generate(programForm, Id);

			MessageBox.Show("Товар был успешно добавлен");
		}

		// Отмена добавления товара
		private void ProductCancelButtonClick(Object s, EventArgs e)
		{
			programForm.History.Pop().RegenerateOldForm();
		}

		#endregion

		#region Изменение товара
		// Кнопка сохранения изменений товара
		private void SaveButtonClick(Object s, EventArgs e)
		{

			string Name = TbProductName.Text.Trim();
			string About = RtbAbout.Text.Trim();
			Image Img = ProductImg.BackgroundImage;

			int Price;
			int Quantity;


			// Проверка данных

			// Проверка названия
			if (!QueryUtils.CheckName(Name))
			{
				MessageBox.Show("Название должно содержать от 4 до 50 букв");
				return;
			}

			// Проверка цены
			if (!QueryUtils.CheckNum(TbPrice.Text.Trim(), out Price, 0, QueryUtils.MaxPrice))
			{
				MessageBox.Show("Цена недопустима, она должна содержать значение в диапазоне от 1 до " + QueryUtils.MaxPrice.ToString());
				return;
			}

			// Проверка цены
			if (!QueryUtils.CheckNum(TbQuantity.Text.Trim(), out Quantity, 0, QueryUtils.MaxQuantity))
			{
				MessageBox.Show("Количество недопустимо, оно должно содержать значение в диапазоне от 1 до " + QueryUtils.MaxQuantity.ToString());
				return;
			}

			// Проверка существования единицы измерения
			if (CbMeasureUnits.SelectedValue == null)
			{
				MessageBox.Show("Выбранная единица измерения не существует");
				return;
			}

			int MeasureUnitId = Convert.ToInt32(CbMeasureUnits.SelectedValue);

			// Проверка существования категории
			if (CbCategory.SelectedValue == null)
			{
				MessageBox.Show("Выбранная категория не существует");
				return;
			}

			int CategoryId = Convert.ToInt32(CbCategory.SelectedValue);


			// Отправка
			if (
				Product.Update(
					programForm.User.Id,
					Name,
					About,
					Price,
					MeasureUnitId,
					Quantity,
					CategoryId,
					Img
				)
			)
			{
				// Обновление формы
				Product.GetData(Product.Id);
				FillWritableForm();

				MessageBox.Show("Товар был успешно изменен");

			}

		}

		// Кнопка отмены изменений товара
		private void CancelButtonClick(Object s, EventArgs e)
		{

			FillWritableForm();

		}


		#endregion
		// Удаление товара
		private void DeleteButtonClick(Object s, EventArgs e)
		{

			// Удаление товара
			//Product.Delete();

			// Переход на форму просмотра товаров
			new DynProductsBandForm().Generate(programForm);

		}

		// Кнопка добавления 
		// товара в заказ
		private void AddToOrderButtonClick(Object s, EventArgs e)
		{

			int Quantity;

			// Проверка количества 
			if (!QueryUtils.CheckNum(TbOrderQuantity.Text, out Quantity, 0, QueryUtils.MaxQuantityInOrder))
			{
				MessageBox.Show("Количество может быть числом от 1 до " + QueryUtils.MaxQuantityInOrder.ToString());
				return;
			}

			if (Quantity > Product.Quantity)
			{
				MessageBox.Show("Указанное число превышает количество товара, что есть у продавца");
				return;
			}

			// Добавление в заказ
			Product.AddToOrder(programForm.User.Organization.Id, Quantity);

			MessageBox.Show("Товар добавлен в заказ");

		}

		#endregion

	}
}
