using System.Windows.Forms;

using Skladik.Adapters;

namespace Skladik.Forms {
	public class DynForm {

		protected Form1 programForm;

		public virtual void Generate(Form1 aForm) {
			programForm = aForm;
			// aForm.User.History.Push(this);
		}

	}
}
