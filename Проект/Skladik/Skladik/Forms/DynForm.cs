using System.Windows.Forms;

using Skladik.Adapters;

namespace Skladik.Forms
{
	public class DynForm
	{

		protected const TableLayoutPanelCellBorderStyle BorderStyle = TableLayoutPanelCellBorderStyle.Inset;

		protected Form1 programForm;

		protected TableLayoutPanel formContent;

		// Настройка формы
		protected virtual void SetUpMainForm() { }

		// Возврат старой формы
		private void ReturnForm()
		{
			programForm.Controls.Clear();
			programForm.Controls.Add(formContent);
		}

		// Используется для возврвщаения назад по формам
		public void RegenerateOldForm()
		{
			if (programForm != null && formContent != null)
			{
				SetUpMainForm();
				ReturnForm();
			}

		}

		// Настройка формы
		public virtual void Generate(Form1 aForm)
		{
			programForm = aForm;
			SetUpMainForm();
		}

	}
}
