using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Skladik.NewComponents;

namespace Skladik.NewComponents {

	public delegate BandElement DElementCreationStrategy(BandElement newElemend, DataRow row);
	
													// Класс ленты товаров
	public class ProductBand : FlowLayoutPanel {

		private DataTable dataSource;
		public DataTable DataSource { 
			get { return dataSource; } 
			set {
				dataSource = value;

				if (dataSource != null && ElementCreationStrategy != null)
					foreach (DataRow Row in dataSource.Rows)
						AddElement(ElementCreationStrategy(new BandElement(), Row));
			}
		}
													// Алгоритм добавления нового элемента
													// на основе строки таблицы из datasource
		public DElementCreationStrategy ElementCreationStrategy { get; set; }

		public event EventHandler ProductChoosed;

		public ProductBand() {

			FlowDirection = FlowDirection.LeftToRight;
			WrapContents = true;
			VerticalScroll.Enabled = true;
			AutoScroll = true;
		}
													// Добавление элемента
		public void AddElement(BandElement elem) {
			elem.ProductClicked += this.ProductClicked;
			Controls.Add(elem);
		}

													// Было произведено нажатие на карточку
		private void ProductClicked(Object s, EventArgs e) {
			if (ProductChoosed != null)
				ProductChoosed(s, e);
		}
	}
}
