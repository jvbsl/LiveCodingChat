using System;
using System.Windows.Forms;
using LiveCodingChat.Xmpp;

namespace NeinTom
{
	public partial class ChatControl
	{
		public ChatControl ()
		{
			InitializeComponent ();
		}
		public Room Room{ get; set; }
		public void AddMessage(LiveCodingChat.Xmpp.MessageReceivedEventArgs e)
		{
			txtChatLog.AppendText ("[" + e.TimeStamp.ToString () + "]" + e.Nick + ": " + e.Message + "\n");
			txtChatLog.ScrollToCaret ();
		}
		void TxtToSend_KeyDown (object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (Room == null)
				return;
			if (e.KeyCode == Keys.Enter && !e.Shift) {
				e.SuppressKeyPress = true;
				Room.SendMessage (txtToSend.Text);
				txtToSend.Text = "";
			}
		}
	}
}

