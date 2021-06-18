using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

using Skladik.Utils;

namespace Skladik.NewComponents {

													// Класс для пагинации списков
	public class Paginator : FlowLayoutPanel {

		private List<Label> labels;
		public int ChoosedPage { get; private set; }
		private int labelCount;
		private int pageCount = 1;
		private int labelOffset = 1;


		public event EventHandler PageChanged;

		public Paginator(int pageCount, int labelCount) {
			this.WrapContents = false;
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

			this.labelCount = labelCount;

			ChoosedPage = 1;

			labels = new List<Label>();

			this.pageCount = pageCount;

			for (int i = 0; i < labelCount ; i++) {
				
				Label NewLabel = new Label();

				//NewLabel.Size = new Size(20, 20);
				NewLabel.AutoSize = true;
				NewLabel.Click += PageOnClick;
				//NewLabel.Cursor = Cursors.Hand;

				labels.Add(NewLabel);
			}

			Redraw();

			foreach (Label Elem in labels)
				this.Controls.Add(Elem);

			LightSelected();
		}

													// Перерисовка пагинатора
		public void Redraw() {

			if (labelOffset > 2)
				labels[0].Text = "<";
			else 
				labels[0].Text = "1";

			if (labelOffset + labelCount < pageCount)
				labels[labelCount - 1].Text = ">";
			else
				labels[labelCount - 1].Text = "";

			for (int i = 1; i < labelCount - 1; i++) { 
				string Temp;

				if (labelOffset + i - 1 < pageCount)
					Temp = (labelOffset + i).ToString();
				else
					Temp = "";
				
				labels[i].Text = Temp;

			}

		}
		
													// Подсветка выбранной страницы
		private void LightSelected() {
			foreach (Label Elem in labels) {
				if (Elem.Text == ChoosedPage.ToString())
					Elem.BackColor = SystemColors.ActiveCaptionText;
				else
					Elem.BackColor = SystemColors.Control;
			}
		}

													// Срабатывает при нажатии на любую метку
		private void PageOnClick(object s, EventArgs e) {
			
			int Temp;
			string ChoosedValue = ((Label)s).Text;

			if (int.TryParse(ChoosedValue, out Temp)) {
				if (Temp != ChoosedPage) {
					ChoosedPage = Temp;
					if (PageChanged != null)
						PageChanged(this, new EventArgs());
				}
			} else if (ChoosedValue == "<") {
 				labelOffset -= labelCount - 2;
				Redraw();
			} else if (ChoosedValue == ">") {
				labelOffset += labelCount - 2;
				Redraw();
			}

			LightSelected();
			
		}

	}
}
