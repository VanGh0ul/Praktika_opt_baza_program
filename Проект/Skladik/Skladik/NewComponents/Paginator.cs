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
		public int LabelCount { get; set; }
		private int pageCount = 0;

		public int PageCount { 
			get { return pageCount; }
			set { 
				pageCount = value;

				ChoosedPage = 1;
				labelOffset = 0;

				Redraw();
				LightSelected();
			} 
		}

		private int labelOffset = 0;


		public event EventHandler PageChanged;

		public Paginator() {
			
			this.WrapContents = false;
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

			ChoosedPage = 1;

			labels = new List<Label>();

		}

		public Paginator(int labelCount) : this() {

			LabelCount = labelCount;

			CreateLabels();

		}

		public Paginator(int pageCount, int labelCount) : this() {

			LabelCount = labelCount;

			this.pageCount = pageCount;

			CreateLabels();

		}

													// Пересоздает все поля
		public void CreateLabels() {

			labels.Clear();
			this.Controls.Clear();

			for (int i = 0; i < LabelCount ; i++) {
				
				Label NewLabel = new Label();

				//NewLabel.Size = new Size(20, 20);
				NewLabel.AutoSize = true;
				NewLabel.Click += PageOnClick;
				//NewLabel.Cursor = Cursors.Hand;

				labels.Add(NewLabel);
				this.Controls.Add(NewLabel);
			}

			Redraw();
			/*
			foreach (Label Elem in labels)
				this.Controls.Add(Elem);
			*/
			LightSelected();
		}

													// Перерисовка пагинатора
		public void Redraw() {

			if (labelOffset > 1)
				labels[0].Text = "<";
			else 
				labels[0].Text = "1";

			if (labelOffset + LabelCount < pageCount)
				labels[LabelCount - 1].Text = ">";

			else if (labelOffset + LabelCount == pageCount)
				labels[LabelCount - 1].Text = pageCount.ToString();

			else
				labels[LabelCount - 1].Text = "";

			for (int i = 1; i < LabelCount - 1; i++) {  
				string Temp;

				int NewNum = labelOffset + i + 1;

				if (NewNum <= PageCount)
					Temp = NewNum.ToString();
				else
					Temp = "";
				
				labels[i].Text = Temp;

			}

		}
		
													// Подсветка выбранной страницы
		private void LightSelected() {
			foreach (Label Elem in labels) {
				if (Elem.Text == ChoosedPage.ToString())
					Elem.BackColor = Color.Aqua;
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
 				labelOffset -= LabelCount - 2;
				Redraw();

			} else if (ChoosedValue == ">") {
				labelOffset += LabelCount - 2;
				Redraw();
			}

			LightSelected();
			
		}

	}
}
