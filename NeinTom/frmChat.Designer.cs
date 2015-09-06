using System;

namespace NeinTom
{
	public partial class frmChat : System.Windows.Forms.Form
	{
		private void InitializeComponents()
		{
			tabControl = new CustomTabControl ();
			tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			tabControl.ItemSize = new System.Drawing.Size(0, 1);
			tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			tabControl.TabIndexChanged += TabControl_TabIndexChanged;
			this.Text = "Chat";

			this.Controls.Add (tabControl);
		}
			
		private CustomTabControl tabControl;
	}
}

