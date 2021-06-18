using System.Drawing;
using System.Windows.Forms;

using Skladik.Utils;

namespace Skladik.NewComponents {
													// Панель организации
	public class OrgPanel : TableLayoutPanel {
		
		public int UserId { get; private set; }
		public Label LName { get; private set; }
		public PictureBox PbImage { get; private set; }

		public OrgPanel(int id, string name, Image img) {
			
			LName = new Label();
			PbImage = new PictureBox();

			UserId = id;
			
													// Название
			LName.Text = name;
			LName.AutoSize = true;
			//LName.Margin = new Padding(5, 3, 3, 3);
			LName.Anchor = AnchorStyles.None;
			
													// Картинка
			PbImage.BackgroundImage = img;
			PbImage.BackgroundImageLayout = ImageLayout.Zoom;
			PbImage.Dock = DockStyle.Fill;
			PbImage.Margin = new Padding(10, 5, 10, 5);

			this.Height = Styles.PanelListElementHeight;

			this.ColumnCount = 2;
			this.RowCount = 1;

			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

			this.RowStyles.Add(new RowStyle(SizeType.Absolute, Styles.PanelListElementHeight));

			
			this.Controls.Add(PbImage, 0, 0);
			this.Controls.Add(LName, 1, 0);
			

		}

	}

													// Список организаций
	public class OrganizationIdBasedPanelList : PanelList {

		public void AddAPanel(int id, string name, Image img) {
			
			OrgPanel NewPanel = new OrgPanel(id, name, img);
			
			NewPanel.BackColor = SystemColors.ActiveCaptionText;
			NewPanel.Width = Width - 40;
			NewPanel.Height = Styles.PanelListElementHeight;

			Controls.Add(NewPanel);

		}

	}
}
