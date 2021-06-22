using System;
using System.Windows.Forms;

namespace Skladik.NewComponents {

													// Класс используемый для выбора изображений
	public class OpenImageString : TableLayoutPanel {

		public Label LComment { get; private set; }
		public Button BOpenImg { get; private set; }
		public string FileName { get; private set; }

		public event EventHandler AfterImgLoad;

		public OpenImageString() {
			
			LComment = new Label();
			BOpenImg = new Button();

			BOpenImg.Click += delegate(Object s, EventArgs e) {
				
				OpenFileDialog Ofd = new OpenFileDialog();
				Ofd.Filter = "Изображения (*.png, *.jpg, *.bmp)|*.png;*jpg;*bmp";
				Ofd.FilterIndex = 0;

				if (Ofd.ShowDialog() == DialogResult.OK) { 
					FileName = Ofd.FileName;
					AfterImgLoad(this, new EventArgs());
				}

			};

			LComment.Text = "Изображение организации:";
			LComment.AutoSize = true;
			LComment.Margin = new Padding(0);
			LComment.Anchor = AnchorStyles.Right;

			BOpenImg.Text = "Открыть";
			BOpenImg.Dock = DockStyle.Fill;

			this.AutoSize = true;
			this.Padding = new Padding(0);
			this.Margin = new Padding(0);

			// this.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			this.ColumnCount = 2;
			this.RowCount = 1;

			// this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, ));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			this.Controls.Add(LComment, 0, 0);
			this.Controls.Add(BOpenImg, 1, 0);

		}

	}
}
