using System;
using System.Windows.Forms;

namespace Skladik.NewComponents {

	// Класс кнопок Далее и Назад на формах регистрации и создания организации
	class BackNextButtons : TableLayoutPanel {

		public Button BBack { get; private set; }
		public Button BNext { get; private set; }
		
		public event EventHandler BackClick;
		public event EventHandler NextClick;
		
		public BackNextButtons() {
			
			BBack = new Button();
			BNext = new Button();

			BBack.Click += delegate(Object s, EventArgs e) {
				BackClick(BBack, new EventArgs());
			};
			BNext.Click += delegate(Object s, EventArgs e) {
				NextClick(BNext, new EventArgs());
			};

			BBack.Text = "Назад";

			BBack.Dock = DockStyle.Fill;
	
			BBack.Height = 55;

			BNext.Text = "Далее";
			BNext.Dock = DockStyle.Fill;
	

			this.AutoSize = true;

			 //this.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			this.ColumnCount = 3;
			this.RowCount = 1;

			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));

			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			this.Controls.Add(BBack, 0, 2);
			this.Controls.Add(BNext, 3, 2);

		}

	}

}
