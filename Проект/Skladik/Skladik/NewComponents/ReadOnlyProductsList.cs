using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

using Skladik.Utils;

namespace Skladik.NewComponents {
												// Класс элемента списка содержимого заказа
	public class ProductPanel : TableLayoutPanel { 
	
		public int ProductId { get; set; }
		public Label LName { get; set; }
		public Label LQuantityString { get; set; }
		public Label LPriceString { get; set; }
		public Label LSum { get; set; }

		// public event EventHandler OrderClick;

		public ProductPanel() {

			LName = new Label();
			LQuantityString = new Label();
			LPriceString = new Label();
			LSum = new Label();

			#region Компоненты
		
			Styles.TextStyle(LName);
			Styles.TextStyle(LQuantityString);
			Styles.TextStyle(LPriceString);
			Styles.TextStyle(LSum);

			LSum.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			this.AutoSize = true;
			this.MaximumSize = Styles.OrderContentListElementMaximumSize;
			this.MinimumSize = Styles.OrderContentListElementMinimumSize;
			this.BackColor = Color.Aqua;

			#endregion

			#region Рамка

			// this.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			this.ColumnCount = 3;
			this.RowCount = 8;

			this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

													// Промежуток
			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Промежуток
			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			
													// Промежуток
			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Промежуток
			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			#endregion

			this.Controls.Add(LName, 1, 1);
			this.Controls.Add(LQuantityString, 1, 3);
			this.Controls.Add(LPriceString, 1, 4);
			this.Controls.Add(LSum, 1, 6);

		}

		
		public ProductPanel(int id, string name, string quantity, string price, string sum) : this() {
		
			ProductId = id;
			LName.Text = name;
			LQuantityString.Text = quantity;
			LPriceString.Text = price;
			LSum.Text = sum;
		
		}

	}


	public delegate ProductPanel DProductAddStrategy(ProductPanel prodPanel, DataRow row);

												// Список содержимого заказа
	public class ReadOnlyProductsList : PanelList {
	
		public DProductAddStrategy ProductAddStrategy { get; set; }

		private DataTable dataSource;
		public DataTable DataSource { 
			get { return dataSource; }
			set {
				dataSource = value;

				if (dataSource != null && ProductAddStrategy != null) {
					foreach (DataRow Row in dataSource.Rows)
						AddAProduct(ProductAddStrategy(new ProductPanel(), Row));
				}
			}
		}

													// Добавление продукта к списку содержимого заказа
		public void AddAProduct(ProductPanel prodPanel) {

			this.Controls.Add(prodPanel);

		}

	}

}
