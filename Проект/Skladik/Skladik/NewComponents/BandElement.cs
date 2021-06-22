using System;
using System.Drawing;
using System.Windows.Forms;

using Skladik.Utils;

namespace Skladik.NewComponents {

													// Класс элемента ленты товаров
	public class BandElement : TableLayoutPanel {

		public int ProductId { get; set; }
		public PictureBox PbImage { get; set; }
		public Label LName { get; set; }
		public Label LPrice { get; set; }
		public Label LCount { get; set; }
		public Label LAddDate { get; set; }
		public Label LOrgName { get; set; }
		public string MeasureUnit { get; set; }

		public event EventHandler ProductClicked;

		public BandElement(int id, Image img, string name, string price, string count, string addDate, string orgName, string measureUnit)
			: this() {

													// Передача значений	
			ProductId = id;
			PbImage.BackgroundImage = img;
			LName.Text = name;
			LPrice.Text = price;
			LCount.Text = count;
			LAddDate.Text = addDate;
			LOrgName.Text = orgName;
			MeasureUnit = measureUnit;

		}

		public BandElement() {
			
			PbImage = new PictureBox();
			LName = new Label();
			LPrice = new Label();
			LCount = new Label();
			LAddDate = new Label();
			LOrgName = new Label();

			this.Click += UnionClick;
			PbImage.Click += UnionClick;
			LName.Click += UnionClick;
			LPrice.Click += UnionClick;
			LCount.Click += UnionClick;
			LAddDate.Click += UnionClick;
			LOrgName.Click += UnionClick;


			#region Свойства компонентов

			PbImage.BackgroundImageLayout = ImageLayout.Zoom;
			PbImage.Dock = DockStyle.Fill;

			// this.BackColor = SystemColors.ActiveCaptionText; Цвет элкментов ленты а поч продублировалось много раз
			
			Styles.TextStyle(LName);

			
			Styles.TextStyle(LPrice);

			
			Styles.TextStyle(LCount);

			
			Styles.TextStyle(LAddDate);

			
			Styles.TextStyle(LOrgName);
			#endregion

			#region Рамка

			Size = Styles.ProductBandElementSize;
			CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
			Margin = new Padding(0);

			ColumnCount = 3;
			RowCount = 9;

			ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));
			ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));

													// Свободный промежуток
			RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

													// Картинка
			RowStyles.Add(new RowStyle(SizeType.Percent, 100));

													// Промежуток
			RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

													// Текст
			RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Текст
			RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Текст
			RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Текст
			RowStyles.Add(new RowStyle(SizeType.AutoSize));
													// Текст
			RowStyles.Add(new RowStyle(SizeType.AutoSize));

													// Свободный промежуток
			RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
			
			#endregion

			Controls.Add(PbImage, 1, 1);
			Controls.Add(LName, 1, 3);
			Controls.Add(LPrice, 1, 4);
			Controls.Add(LCount, 1, 5);
			Controls.Add(LAddDate, 1, 6);
			Controls.Add(LOrgName, 1, 7);


		}

													// Объединение нажатий на элементы карточки
		private void UnionClick(Object s, EventArgs e) {
			if (ProductClicked != null)
				ProductClicked(this, new EventArgs());
		}
	
	}

}
