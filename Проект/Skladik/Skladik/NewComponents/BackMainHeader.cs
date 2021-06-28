using System;
using System.Windows.Forms;

namespace Skladik.NewComponents {
	public class BackMainHeader : TableLayoutPanel {

		public Button BGoBack { get; private set; }
		public Button BGoMain { get; private set; }

		public BackMainHeader() {

			ColumnCount = 3;
			RowCount = 1;

													// Кнопка
			ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
													// Кнопка
			ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
													// Пустое место
			ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			BGoBack = new Button();
			BGoBack.Text = "<";
			// BGoBack.Font = new Font("Comic Sans MS", 8);
			BGoBack.Dock = DockStyle.Fill;
			BGoBack.Margin = new Padding(0);

			BGoMain = new Button();
			BGoMain.Text = "Главная";
			// BGoMain.Font = new Font("Comic Sans MS", 8);
			BGoMain.Dock = DockStyle.Fill;
			BGoMain.Margin = new Padding(0);

			Controls.Add(BGoBack, 0, 0);
			Controls.Add(BGoMain, 1, 0);

		}

	}
}
