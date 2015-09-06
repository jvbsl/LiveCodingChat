using System;
namespace NeinTom
{
	public partial class ChatControl:System.Windows.Forms.UserControl
	{
		private void InitializeComponents()
		{
			spltUserList = new System.Windows.Forms.SplitContainer();
			spltUserList.Dock = System.Windows.Forms.DockStyle.Fill;

			spltUserList.Orientation = System.Windows.Forms.Orientation.Vertical;

			spltRoom = new System.Windows.Forms.SplitContainer ();
			spltRoom.Dock = System.Windows.Forms.DockStyle.Fill;
			spltRoom.Orientation = System.Windows.Forms.Orientation.Horizontal;


			txtChatLog = new System.Windows.Forms.TextBox ();
			txtChatLog.Multiline = true;
			txtChatLog.Dock = System.Windows.Forms.DockStyle.Fill;


			txtToSend = new System.Windows.Forms.TextBox ();
			txtToSend.Multiline = true;
			txtToSend.Dock = System.Windows.Forms.DockStyle.Fill;
			txtToSend.KeyDown += TxtToSend_KeyDown;


			lstUsers = new System.Windows.Forms.ListBox ();
			lstUsers.Dock = System.Windows.Forms.DockStyle.Fill;

			spltRoom.Panel1.Controls.Add (txtChatLog);

			spltRoom.Panel2.Controls.Add (txtToSend);


			spltUserList.Panel1.Controls.Add (spltRoom);
			spltUserList.Panel2.Controls.Add (lstUsers);

			this.Controls.Add(spltUserList);
		}



		private System.Windows.Forms.SplitContainer spltUserList;
		private System.Windows.Forms.SplitContainer spltRoom;
		private System.Windows.Forms.TextBox txtChatLog;
		private System.Windows.Forms.TextBox txtToSend;
		private System.Windows.Forms.ListBox lstUsers;
	}
}

