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


		public PanelElement() {
			
			LName = new Label();
			//LName.Dock = DockStyle.Fill;
			LName.AutoSize = true;

			LName.Margin = new Padding(5, 3, 3, 3);

			this.Click += UnionClick;
			LName.Click += UnionClick;

			this.BackColor = Color.Aqua;

			this.AutoSize = true;

			this.Controls.Add(LName);
		}
													
		public PanelElement(int id, string name) : this() {

			ElementId = id;
			LName.Text = name;

		}

													// Объединение событий нажатия
		private void UnionClick(Object s, EventArgs e) {
			if (PanelClick != null)
				PanelClick(this, new EventArgs());
		}


	}

	public delegate PanelElement DAddElementStrategy(PanelElement elem, DataRow row);

												// Класс, используемый для вывода справочников
	public class IdBasedPanelList : PanelList {
			
												// Алгоритм, используемый для составления 
												// элементов из источника
		public DAddElementStrategy AddElementStrategy { get; set; }

		private DataTable dataSource;
		public DataTable DataSource { 
			get { return dataSource; } 
			set {
				dataSource = value;

				if (dataSource != null && AddElementStrategy != null) {
					foreach (DataRow Row in dataSource.Rows)
						AddAPanel(AddElementStrategy(new PanelElement(), Row));

					Redraw();
				}
			}
		}

													// Перерисовка цветов
		private void Redraw() {

			if (ChoosedPanel != null)
				foreach (Control elem in this.Controls) {

					PanelElement EPanel = (PanelElement)elem;

					if (EPanel.ElementId == ChoosedPanel.ElementId) {
					
						ChoosedPanel = EPanel;
						ChoosedPanel.BackColor = Color.Yellow;
						return;

					}
				}
		}

													// Выбранная панель
		public PanelElement ChoosedPanel { get; set; }

													// Событие происходящее при выборе новой панели
		public event EventHandler PanelChoosed;

													// Ширина всех панелей при создании
		public int PanelsWidth { get; set; }

													
		public IdBasedPanelList() : base() {
			ChoosedPanel = null;
			PanelsWidth = Width;
		}


		private void PanelSetUp(PanelElement elem) {
			
			Size ElementsSize = new Size(PanelsWidth, 1000);

			elem.MaximumSize = ElementsSize;
			elem.MinimumSize = new Size(PanelsWidth, 30);
			elem.LName.MaximumSize = ElementsSize;

			elem.PanelClick += PanelClick;

		}

													// Добавление панели
		public virtual void AddAPanel(int id, string name) {

			PanelElement Temp = new PanelElement(id, name);

			PanelSetUp(Temp);

			this.Controls.Add(Temp);

		}

		public void AddAPanel(PanelElement elem) {
			
			PanelSetUp(elem);

			this.Controls.Add(elem);

		}

													// Срабатывает при выборе панели с ид
													// отличной от ныне выбранного
		private void PanelClick(Object s, EventArgs e) {
			
			PanelElement NewChoosedPanel = (PanelElement)s;

			if (ChoosedPanel == null || ChoosedPanel.ElementId != NewChoosedPanel.ElementId) {
					
					
				if (ChoosedPanel != null)
					ChoosedPanel.BackColor = Color.Aqua;

				NewChoosedPanel.BackColor = Color.Yellow;

				ChoosedPanel = NewChoosedPanel;
				if (PanelChoosed != null) 
					PanelChoosed(this, new EventArgs());
			}
			
		}

													// Сброс выбранной панели и её цвета
		public void Reset() {

			if (ChoosedPanel != null)
				ChoosedPanel.BackColor = Color.Aqua;

			ChoosedPanel = null;
		}
	
	}

}
