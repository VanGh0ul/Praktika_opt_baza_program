using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using Skladik.Utils;

namespace Skladik.NewComponents {
													// Панель организации
	public class OrgPanel : TableLayoutPanel {
		
		public int OrgId { get; set; }
		public Label LName { get; set; }
		public PictureBox PbImage { get; set; }

		public event EventHandler OrgClick;

		public OrgPanel() {
		
			LName = new Label();
			PbImage = new PictureBox();

			this.Click += UnionClick;
			LName.Click += UnionClick;
			PbImage.Click += UnionClick;

			LName.AutoSize = true;
			//LName.Margin = new Padding(5, 3, 3, 3);
			LName.Anchor = AnchorStyles.None;
		
			PbImage.BackgroundImageLayout = ImageLayout.Zoom;
			// PbImage.Size = new Size(25, 5);
			PbImage.Dock = DockStyle.Fill;
			PbImage.Margin = new Padding(5, 5, 5, 5);

			this.Height = Styles.PanelListElementHeight;

			this.ColumnCount = 2;
			this.RowCount = 1;

			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

			this.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.PanelListElementHeight));

			
			this.Controls.Add(PbImage, 0, 0);
			this.Controls.Add(LName, 1, 0);

		}

		public OrgPanel(int id, string name, Image img) : this() {

			OrgId = id;
													// Название
			LName.Text = name;
													// Картинка
			PbImage.BackgroundImage = img;

		}

													// Объединение нажатий
		private void UnionClick(Object s, EventArgs e) {
			if (OrgClick != null)
				OrgClick(this, new EventArgs());
		}

	}

	public delegate OrgPanel DOrgsAddStrategy(OrgPanel newOrg, DataRow Row);

													// Список организаций
	public class OrganizationIdBasedPanelList : PanelList {

													// Алгоритм добавления новой организации
		public DOrgsAddStrategy OrgAddStrategy { get; set; }

		private DataTable dataSource;
		public DataTable DataSource { 
			get { return dataSource; }
			set {
				dataSource = value;

				if (dataSource != null && OrgAddStrategy != null)
					foreach (DataRow Row in dataSource.Rows)
						AddAPanel(OrgAddStrategy(new OrgPanel(), Row));
			}
		}

		public event EventHandler OrgSelected;

		public void AddAPanel(OrgPanel org) {
			
			org.OrgClick += OrgClicked;

			org.BackColor = SystemColors.ActiveCaptionText;
			org.Width = Width - 40;

			if (org.LName.Text.Length > 40)
				org.Height = Styles.PanelListElementHeight + 40;
			
			else
				org.Height = Styles.PanelListElementHeight;

			Controls.Add(org);

		}

													// Выбрана организация 
		private void OrgClicked(Object s, EventArgs e) {
			if (OrgSelected != null)
				OrgSelected(s, e);
		}
	}
}
