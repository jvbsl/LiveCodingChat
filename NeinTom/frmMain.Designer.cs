using System;

namespace NeinTom
{
	public partial class frmMain:System.Windows.Forms.Form
	{
		private void InitializeComponent()
		{
			lstUsers = new System.Windows.Forms.ListBox ();
			lstUsers.Dock = System.Windows.Forms.DockStyle.Fill;
			lstUsers.Enabled = false;
			lstUsers.MouseDoubleClick += LstUsers_MouseDoubleClick;

			this.Text = "Main";
			this.Controls.Add (lstUsers);
		}

		private System.Windows.Forms.ListBox lstUsers;
	}
}

