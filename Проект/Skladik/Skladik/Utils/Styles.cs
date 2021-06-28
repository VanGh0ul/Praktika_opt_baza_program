using System;
using System.Drawing;
using System.Windows.Forms;

using Skladik.NewComponents;

namespace Skladik.Utils
{
	public static class Styles
	{

		public readonly static Font TextFont = new Font("Comic Sans MS", 10);
		public const int CategoriesWidth = 240;
		public const int AccountPanelHeight = 90;
		public const int ProductsPanelUpperHeight = 60;
		public const int PaginatorLabelCount = 6;
		//public const int AdminPaginatorLabelCount = 2;
		public const int AdminPanelHeaderHeight = 70;
		public const int PanelListElementHeight = 50;
		public const int AddressListElemCount = 5;
		public const int UserListElemCount = 5;
		public const int ProductPanelElemCount = 20;
		public const int OrderListElemCount = 20;
		public const int OrderContentListElemCount = 5;
		public const int AdminPanelListsElemCount = 5;
		public readonly static Size ProductBandElementSize = new Size(200, 250);
		public readonly static Size OrderListElementMaximumSize = new Size(725, 180);
		public readonly static Size OrderListElementMinimumSize = new Size(725, 100);
		public readonly static Size OrderContentListElementMaximumSize = new Size(520, 180);
		public readonly static Size OrderContentListElementMinimumSize = new Size(520, 100);
		public const int OrderContentFormButtonsHeight = 45;
		public const int ProductsPerDocument = 20;


		public const int OrganizationFormScrollPanelHeight = 900;

		public static void PaginatorStyle(Paginator aPaginator)
		{
			aPaginator.Margin = new Padding(0);
			aPaginator.Font = TextFont;
			aPaginator.Anchor = AnchorStyles.None;
		}

		public static void TextStyle(Label aLabel, int size = 10)
		{
			aLabel.AutoSize = true;
			aLabel.Font = new Font("Comic Sans MS", size);
		}

		public static void TextBoxStyle(TextBox aTextBox)
		{
			aTextBox.Dock = DockStyle.Fill;
			aTextBox.Font = TextFont;
			aTextBox.Margin = new Padding(0);
		}

		public static void PasswordFieldStyle(PasswordField aPassField)
		{
			aPassField.Dock = DockStyle.Fill;
			aPassField.TextField.Font = TextFont;
			aPassField.ViewButton.BackgroundImageLayout = ImageLayout.Zoom;
			aPassField.ViewButton.BackgroundImage = Properties.Resources.see_password;
		}

		public static void ImgStyle(PictureBox aPBox)
		{
			aPBox.BackgroundImageLayout = ImageLayout.Zoom;
			aPBox.Dock = DockStyle.Fill;
			aPBox.Margin = new Padding(0);
		}

		public static void ComboBoxStyle(ComboBox aCBox)
		{
			aCBox.Font = Styles.TextFont;
			aCBox.Dock = DockStyle.Fill;
			aCBox.Margin = new Padding(0);
		}

		public static void ButtonStyle(Button aButton)
		{
			aButton.Font = Styles.TextFont;
			aButton.Dock = DockStyle.Fill;
			aButton.Margin = new Padding(0);
		}

		public static void RichTextBoxStyle(RichTextBox rtb)
		{
			rtb.Dock = DockStyle.Fill;
			rtb.Font = Styles.TextFont;
			rtb.Margin = new Padding(0);
		}

		public static void ContentFramesStyle(TableLayoutPanel aTable, TableLayoutPanelCellBorderStyle borderStyle)
		{

			aTable.Dock = DockStyle.Fill;
			aTable.CellBorderStyle = borderStyle;
			aTable.Margin = new Padding(0);

		}

		// Централизация окна относительно экрана
		public static Point CentralizeFormByDesktop(Size formSize)
		{

			return new Point((SystemInformation.VirtualScreen.Width - formSize.Width) / 2, (SystemInformation.VirtualScreen.Height - formSize.Height) / 2);

		}

		// Цетрализация окна относительно другого окна
		public static Point CentralizeFormByAnotherOne(Size formToCentralizeSize, Point anotherFormLocation, Size anotherFormSize)
		{

			return new Point(
				anotherFormLocation.X + (anotherFormSize.Width - formToCentralizeSize.Width) / 2,
				anotherFormLocation.Y + (anotherFormSize.Height - formToCentralizeSize.Height) / 2
			);

		}

	}
}
