using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Data;

using Skladik.Utils;

namespace Skladik.NewComponents {
	public class PanelElement : Panel {
		
		public int ElementId { get; set; }

		public Label LName { get; private set; }

		public PanelElement(int id, string name) {
			LName = new Label();
			LName.Margin = new Padding(5, 3, 3, 3);

			ElementId = id;
			LName.Text = name;

			this.Controls.Add(LName);
		}

	}

	public class IdBasedPanelList : PanelList {
		/*
		private DataTable dataSource;
		public DataTable DataSource { 
			get { return dataSource; } 
			set {
				dataSource = value;

				if (ElementCreationStrategy != null)
					foreach (DataRow Row in dataSource.Rows)
						AddElement(ElementCreationStrategy(new BandElement(), Row));
			}
		}
		*/

		public IdBasedPanelList() : base() {
			Margin = new Padding(0, 0, 15, 0);
		}

		public virtual void AddAPanel(int id, string name) {

			PanelElement Temp = new PanelElement(id, name);

			Temp.Width = Width;
			Temp.Height = 30;

			this.Controls.Add(Temp);

		}

	}
}
