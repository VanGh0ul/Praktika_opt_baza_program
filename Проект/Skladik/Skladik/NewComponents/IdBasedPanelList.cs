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

													// Нажатие на панель или на имя
		public event EventHandler PanelClick;

													// Панель категории
		public PanelElement(int id, string name) {
			LName = new Label();
			LName.Dock = DockStyle.Fill;
			LName.Margin = new Padding(5, 3, 3, 3);

			this.Click += UnionClick;
			LName.Click += UnionClick;

			// this.BackColor = SystemColors.ActiveCaptionText;

			ElementId = id;
			LName.Text = name;

			this.Controls.Add(LName);
		}

													// Объединение событий нажатия
		private void UnionClick(Object s, EventArgs e) {
			if (PanelClick != null)
				PanelClick(this, new EventArgs());
		}


	}

	public class IdBasedPanelList : PanelList {

		public PanelElement ChoosedPanel { get; set; }

		public event EventHandler PanelChoosed;

		public IdBasedPanelList() : base() {
			ChoosedPanel = null;
			Margin = new Padding(0, 0, 15, 0);
		}

													// Добавление панели
		public virtual void AddAPanel(int id, string name) {

			PanelElement Temp = new PanelElement(id, name);

			Temp.Width = Width - 25;
			
			if (name.Length < 40)
				Temp.Height = 30;
			else 
				Temp.Height = 50;

			Temp.PanelClick += PanelClick;

			this.Controls.Add(Temp);

		}

													// Срабатывает при выборе панели с ид
													// отличной от ныне выбранного
		private void PanelClick(Object s, EventArgs e) {
			if (PanelChoosed != null) {

				PanelElement NewChoosedPanel = (PanelElement)s;

				if (ChoosedPanel == null || ChoosedPanel.ElementId != NewChoosedPanel.ElementId) {


					if (ChoosedPanel != null)
						ChoosedPanel.BackColor = SystemColors.Control;// Цвеи для невыбранной категории
						; 

					NewChoosedPanel.BackColor = Color.Yellow;

					ChoosedPanel = NewChoosedPanel;

					PanelChoosed(this, new EventArgs());
				}

			}
		}

													// Сброс выбранной панели и её цвета
		public void Reset() {

			if (ChoosedPanel != null)
				ChoosedPanel.BackColor = Color.Yellow; // Цвет для выбранной категорий

			ChoosedPanel = null;
		}
	
	}

}
