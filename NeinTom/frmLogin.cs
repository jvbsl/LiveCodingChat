using System;
using System.Collections.Generic;

namespace NeinTom
{
	public partial class frmLogin
	{
		public frmLogin ()
		{
			InitializeComponent ();

			foreach (KeyValuePair<string,Type> pair in LiveCodingChat.LoginFactory.Instance.LoginMethods) {
				cmbMethod.Items.Add (pair.Key);
			}
		}
		void TextBoxes_TextChanged (object sender, EventArgs e)
		{
			btnOk.Enabled = txtPassword.TextLength > 0 && txtUsername.TextLength > 0 && cmbMethod.SelectedItem != null;
		}
		public string Username{
			get{ 
				return txtUsername.Text;
			}
		}
		public string Password{
			get{
				return txtPassword.Text;
			}
		}
		public string LoginMethod{
			get{
				return (string)cmbMethod.SelectedItem;
			}
		}
	}
}

