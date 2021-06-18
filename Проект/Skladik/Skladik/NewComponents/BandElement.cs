using System.Drawing;
using System.Windows.Forms;

using Skladik.Utils;

namespace Skladik.NewComponents {

													// Класс элемента ленты товаров
	public class BandElement : TableLayoutPanel {

		public int ProductId { get; private set; }
		public PictureBox PbImage { get; private set; }
		public Label LName { get; private set; }
		public Label LPrice { get; private set; }
		public Label LCount { get; private set; }
		public Label LAddDate { get; private set; }
		public Label LOrgName { get; private set; }

		public BandElement(int id, Image img, string name, string price, string count, string addDate, string orgName)
			: this() {
													// Передача значений	
			ProductId = id;

			#region Свойства компонентов
			PbImage.BackgroundImage = img;
			PbImage.BackgroundImageLayout = ImageLayout.Zoom;
			PbImage.Dock = DockStyle.Fill;

			LName.Text = name;
			Styles.TextStyle(LName);

			LPrice.Text = price;
			Styles.TextStyle(LPrice);

			LCount.Text = count;
			Styles.TextStyle(LCount);

			LAddDate.Text = addDate;
			Styles.TextStyle(LAddDate);

			LOrgName.Text = orgName;
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

		public BandElement() {
			
			PbImage = new PictureBox();
			LName = new Label();
			LPrice = new Label();
			LCount = new Label();
			LAddDate = new Label();
			LOrgName = new Label();

		}
	
	}

}
