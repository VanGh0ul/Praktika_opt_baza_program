using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

using Skladik.Utils;

namespace Skladik.NewComponents {
												// Класс элемента списка содержимого заказа
	public class EditableProductPanel : TableLayoutPanel { 
	
		public int ProductId { get; set; }
		
		private int price;
		public int Price { 
			get { return price; } 
			set  {
				price = value;

				LPriceString.Text = "Цена за 1 " + measureUnit + " ~ " + value.ToString() + " руб.";
			}
		}

		private string measureUnit;
		public string MeasureUnit {
			get { return measureUnit; }
			set {
				measureUnit = value;

				LMeasureUnit.Text = value;
				LPriceString.Text = "Цена за 1 " + value + " ~ " + price.ToString() + " руб.";
			}
		}

		private Label LMeasureUnit;
		private Label LPriceString;

		public Label LName { get; set; }
		public TextBox TbQuantity { get; set; }
		public Label LSum { get; set; }
		public Button BDelete { get; set; }
		public Button BSave { get; set; }

		public event EventHandler SaveProductClick;
		public event EventHandler DeleteProductClick;

		public EditableProductPanel() {

			LName = new Label();
			TbQuantity = new TextBox();
			LMeasureUnit = new Label();
			LPriceString = new Label();
			LSum = new Label();
			BDelete = new Button();
			BSave = new Button();

			#region Компоненты
			/*
			this.Click += UnionClick;
			LName.Click += UnionClick;
			LSeller.Click += UnionClick;
			LSentOn.Click += UnionClick;
			LDeliverOn.Click += UnionClick;
			LDeliverAddress.Click += UnionClick;
			LStatus.Click += UnionClick;
			LSum.Click += UnionClick;
			*/

													// Вычисление суммы позиции при изменении поля
			TbQuantity.TextChanged += QuantityOnChange;

			BSave.Click += delegate(Object s, EventArgs e) {
				if (SaveProductClick != null)
					SaveProductClick(this, new EventArgs());
			};

			BDelete.Click += delegate(Object s, EventArgs e) {
				if (DeleteProductClick != null)
					DeleteProductClick(this, new EventArgs());
			};

			Styles.TextStyle(LName);
			Styles.TextBoxStyle(TbQuantity);
			Styles.TextStyle(LMeasureUnit);
			Styles.TextStyle(LPriceString);
			Styles.TextStyle(LSum);
			
			Styles.ButtonStyle(BSave);
			BSave.Text = "Сохранить";
			BSave.Margin = new Padding(1);
			BSave.BackColor = SystemColors.Control;

			Styles.ButtonStyle(BDelete);
			BDelete.Text = "Удалить";
			BDelete.Margin = new Padding(1);
			BDelete.BackColor = SystemColors.Control;

			LMeasureUnit.Anchor = AnchorStyles.Left;

			this.AutoSize = true;
			this.MaximumSize = Styles.OrderContentListElementMaximumSize;
			this.MinimumSize = Styles.OrderContentListElementMinimumSize;
			this.BackColor = Color.Aqua;


			#region Создание строки изменения количества

			TableLayoutPanel QuantityString = new TableLayoutPanel();
			Styles.ContentFramesStyle(QuantityString, TableLayoutPanelCellBorderStyle.None);

			QuantityString.ColumnCount = 4;
			QuantityString.RowCount = 1;

													// Текст
			QuantityString.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

													// Поле
			QuantityString.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
			
													// Единица измерения
			QuantityString.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

													// Промежуток
			QuantityString.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
				

			QuantityString.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

													// Текст
			Label LOrderedText = new Label();
			Styles.TextStyle(LOrderedText);
			LOrderedText.Text = "Заказано: ";
			LOrderedText.Anchor = AnchorStyles.Left;

			QuantityString.Controls.Add(LOrderedText, 0, 0);
			QuantityString.Controls.Add(TbQuantity, 1, 0);
			QuantityString.Controls.Add(LMeasureUnit, 2, 0);

			Label LSumText = new Label();
			Styles.TextStyle(LSumText);
			LSumText.Text = "Общая сумма: ";
			LSumText.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		 
			#endregion

			#endregion

			#region Рамка

			// this.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			this.ColumnCount = 4;
			this.RowCount = 8;

			this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

													// Промежуток
			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Промежуток
			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			
													// Промежуток
			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Промежуток
			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			#endregion

			this.Controls.Add(LName, 1, 1);
			this.Controls.Add(QuantityString, 1, 3);
			this.Controls.Add(LPriceString, 1, 4);
			this.Controls.Add(LSumText, 1, 6);
			this.Controls.Add(LSum, 2, 6);
			this.Controls.Add(BDelete, 2, 1);
			this.Controls.Add(BSave, 2, 3);

		}

		
		public EditableProductPanel(int id, string name, string quantity, string measureUnit, int price, string sum) : this() {
		
			ProductId = id;
			Price = price;
			MeasureUnit = measureUnit;
			LName.Text = name;
			TbQuantity.Text = quantity;
			LSum.Text = sum;
		
		}

		public void QuantityOnChange(Object s, EventArgs e) {
			
			int Temp;
			if (QueryUtils.CheckNum(TbQuantity.Text, out Temp, 0, QueryUtils.MaxQuantityInOrder)) 
				LSum.Text = (Price * Temp).ToString() + " руб.";

			else
				LSum.Text = "";

		} 
	
	}


	public delegate EditableProductPanel DEditableProductAddStrategy(EditableProductPanel prodPanel, DataRow row);

												// Список содержимого заказа
	public class EditableProductsList : PanelList {
	
		public DEditableProductAddStrategy ProductAddStrategy { get; set; }

		private DataTable dataSource;
		public DataTable DataSource { 
			get { return dataSource; }
			set {
				dataSource = value;

				if (dataSource != null && ProductAddStrategy != null) {
					foreach (DataRow Row in dataSource.Rows)
						AddAProduct(ProductAddStrategy(new EditableProductPanel(), Row));
				}
			}
		}

		public event EventHandler ProductSaved;
		public event EventHandler ProductDeleted;

													// Добавление продукта к списку содержимого заказа
		public void AddAProduct(EditableProductPanel prodPanel) {

			prodPanel.SaveProductClick += delegate(Object s, EventArgs e) {
			
				if (ProductSaved != null)
					ProductSaved(s, e);

			};

			prodPanel.DeleteProductClick += delegate(Object s, EventArgs e) {
			
				if (ProductDeleted != null)
					ProductDeleted(s, e);

			};

			this.Controls.Add(prodPanel);

		}

	}

}
