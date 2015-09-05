using System;

namespace NeinTom
{
	public partial class frmLogin:System.Windows.Forms.Form
	{
		private void InitializeComponents()
		{
			lblUsername = new System.Windows.Forms.Label ();
			lblUsername.Text = "Username:";
			lblUsername.Location = new System.Drawing.Point (12, 12);

			txtUsername = new System.Windows.Forms.TextBox ();
			txtUsername.Location = new System.Drawing.Point(12+12+lblUsername.Width,12);
			txtUsername.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right| System.Windows.Forms.AnchorStyles.Top;
			txtUsername.TextChanged += TextBoxes_TextChanged;


			lblPassword = new System.Windows.Forms.Label ();
			lblPassword.Text = "Password:";
			lblPassword.Location = new System.Drawing.Point (12, 12+lblUsername.Height+12);


			txtPassword = new System.Windows.Forms.TextBox ();
			txtPassword.UseSystemPasswordChar = true;
			txtPassword.Location = new System.Drawing.Point (12 + 12 + lblPassword.Width, lblPassword.Top);
			txtPassword.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right| System.Windows.Forms.AnchorStyles.Top;
			txtPassword.TextChanged += TextBoxes_TextChanged;

			lblMethod = new System.Windows.Forms.Label ();
			lblMethod.Text = "Method";
			lblMethod.Location = new System.Drawing.Point (12, lblPassword.Bottom + 12);

			cmbMethod = new System.Windows.Forms.ComboBox ();
			cmbMethod.Location = new System.Drawing.Point (lblMethod.Right + 12, lblMethod.Top);
			cmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;


			btnOk = new System.Windows.Forms.Button ();
			btnOk.Text = "Login";
			btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			btnOk.Location = new System.Drawing.Point (ClientSize.Width - 12 - btnOk.Width, ClientSize.Height - 12 - btnOk.Height);
			btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			btnOk.Enabled = false;


			btnCancel = new System.Windows.Forms.Button ();
			btnCancel.Text = "Cancel";
			btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancel.Location = new System.Drawing.Point (btnOk.Left - 12 - btnCancel.Width, ClientSize.Height - 12 - btnOk.Height);
			btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;

			this.Text = "Login";
			this.AcceptButton = btnOk;
			this.CancelButton = btnCancel;

			this.Controls.Add (btnOk);
			this.Controls.Add (btnCancel);
			this.Controls.Add(lblUsername);
			this.Controls.Add(txtUsername);
			this.Controls.Add(lblPassword);
			this.Controls.Add(txtPassword);
			this.Controls.Add (lblMethod);
			this.Controls.Add (cmbMethod);
		}
			
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label lblUsername;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Label lblMethod;
		private System.Windows.Forms.ComboBox cmbMethod;
	}
}

