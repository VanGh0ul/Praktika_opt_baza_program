using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

using Skladik.Utils;

namespace Skladik.NewComponents {

													// Класс элемента списка заказов
	public class OrderPanel : TableLayoutPanel { 
	
		public int OrderId { get; set; }
		public Label LName { get; set; }
		public Label LSeller { get; set; }
		public Label LSentOn { get; set; }
		public Label LDeliverOn { get; set; }
		public Label LDeliverAddress { get; set; }
		public Label LStatus { get; set; }
		public Label LSum { get; set; }

		public event EventHandler OrderClick;

		public OrderPanel() {

			LName = new Label();
			LSeller = new Label();
			LSentOn = new Label();
			LDeliverOn = new Label();
			LDeliverAddress = new Label();
			LStatus = new Label();
			LSum = new Label();

			#region Компоненты
			
			this.Click += UnionClick;
			LName.Click += UnionClick;
			LSeller.Click += UnionClick;
			LSentOn.Click += UnionClick;
			LDeliverOn.Click += UnionClick;
			LDeliverAddress.Click += UnionClick;
			LStatus.Click += UnionClick;
			LSum.Click += UnionClick;

			Styles.TextStyle(LName);
			Styles.TextStyle(LSeller);
			Styles.TextStyle(LSentOn);
			Styles.TextStyle(LDeliverOn);
			Styles.TextStyle(LDeliverAddress);
			Styles.TextStyle(LStatus);
			Styles.TextStyle(LSum);

			LSum.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			LStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;

			this.AutoSize = true;
			this.MaximumSize = Styles.OrderListElementMaximumSize;
			this.MinimumSize = Styles.OrderListElementMinimumSize;
			this.BackColor = Color.Aqua;

			#endregion

			#region Рамка

			// this.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			this.ColumnCount = 5;
			this.RowCount = 7;

			this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));

													// Промежуток
			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Промежуток
			this.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));

			#endregion

			this.Controls.Add(LName, 1, 1);
			this.Controls.Add(LSeller, 1, 2);
			this.Controls.Add(LSentOn, 1, 3);
			this.Controls.Add(LDeliverOn, 1, 4);
			this.Controls.Add(LDeliverAddress, 1, 5);

			this.Controls.Add(LStatus, 3, 1);
			this.Controls.Add(LSum, 3, 5);

		}

		public OrderPanel(int id, string name, string seller, string sentOn, string deliverOn, string deliverAddress, string status, string sum) : this() {
		
			OrderId = id;
			LName.Text = name;
			LSeller.Text = seller;
			LSentOn.Text = sentOn;
			LDeliverOn.Text = deliverOn;
			LDeliverAddress.Text = deliverAddress;
			LStatus.Text = status;
			LSum.Text = sum;
		
		}
	
													// Объединение событий
		private void UnionClick(Object s, EventArgs e) {
			if (OrderClick != null)
				OrderClick(this, new EventArgs());
		}

	}


	public delegate OrderPanel DOrderAddStrategy(OrderPanel ordPanel, DataRow row);

													// Класс списка заказов
	public class OrdersList : PanelList {

		public DOrderAddStrategy OrderAddStrategy { get; set; }

		private DataTable dataSource;
		public DataTable DataSource { 
			get { return dataSource; }
			set {
				dataSource = value;

				if (dataSource != null && OrderAddStrategy != null) {
					foreach (DataRow Row in dataSource.Rows)
						AddAOrder(OrderAddStrategy(new OrderPanel(), Row));
				}
			}
		}

													// При выборе заказа
		public event EventHandler OrderSelected;

													// Добавление заказа
		public void AddAOrder(OrderPanel ordPanel) {
			
			ordPanel.OrderClick += OrderClicked;

			this.Controls.Add(ordPanel);

		}
			
													// Вызов события списка
		private void OrderClicked(Object s, EventArgs e) {
			
			if (OrderSelected != null)
				OrderSelected(s, e);

		}


	}

}

