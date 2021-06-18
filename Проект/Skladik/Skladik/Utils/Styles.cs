using System;
using System.Drawing;
using System.Windows.Forms;

using Skladik.NewComponents;

namespace Skladik.Utils {
	public static class Styles {

		public readonly static Font TextFont = new Font("Comic Sans MS", 10);
		public const int CategoriesWidth = 240;
		public const int AccountPanelHeight = 90;
		public const int ProductsPanelUpperHeight = 60;
		public const int PaginatorLabelCount = 6;
		//public const int AdminPaginatorLabelCount = 2;
		public const int AdminPanelHeaderHeight = 70;
		public const int PanelListElementHeight = 50;
		public readonly static Size ProductBandElementSize = new Size(200, 250);

		public static void PaginatorStyle(Paginator aPaginator) {
			aPaginator.Margin = new Padding(0);
			aPaginator.Font = TextFont;
			aPaginator.Anchor = AnchorStyles.None;
		}

		public static void TextStyle(Label aLabel) {
			aLabel.AutoSize = true;
			aLabel.Font = TextFont;
		}

		public static void TextBoxStyle(TextBox aTextBox) {
			aTextBox.Dock = DockStyle.Fill;
			aTextBox.Font = TextFont;
		}

		public static void PasswordFieldStyle(PasswordField aPassField) {
			aPassField.Dock = DockStyle.Fill;
			aPassField.TextField.Font = TextFont;
			aPassField.ViewButton.BackgroundImageLayout = ImageLayout.Zoom;
			aPassField.ViewButton.BackgroundImage = Properties.Resources.see_password;
		}

	}
}
