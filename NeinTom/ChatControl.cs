using System;
using System.Windows.Forms;
using LiveCodingChat.Xmpp;
using System.Collections.Generic;
using LiveCodingChat;
using System.Linq;
using NeinTom.ChatLog;

namespace NeinTom
{
	public partial class ChatControl
	{
		public ChatControl ()
		{
			InitializeComponent ();
            users = new Dictionary<User, UserState>();
        }
		public Room Room{ get; set; }
        private Dictionary<User, UserState> users;
		public void AddMessage(LiveCodingChat.Xmpp.MessageReceivedEventArgs e)
		{
            ChatMessage msg = new ChatMessage(chatLog, e.User, e.Message);
            if (e.User == null)
                msg.Nick = e.Nick;
            msg.TimeStamp = "["+e.TimeStamp.ToString()+"]";
            chatLog.AddMessage(msg);
			//txtChatLog.AppendText ("[" + e.TimeStamp.ToString () + "]" + e.Nick + ": " + e.Message + "\r\n");
			//txtChatLog.ScrollToCaret ();
		}
        public void UserStateChanged(User user, UserState state)
        {
            if (state == UserState.Offline)
            {
                users.Remove(user);
            }
            else if (users.ContainsKey(user))
            {
                users[user] = state;
            }
            else
            {
                users.Add(user, state);
            }
            lstUsers.Items.Clear();
            lstUsers.Items.AddRange(users.Keys.ToArray());
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
            else if(e.Control)
            {
                if (e.KeyCode == Keys.Back)
                {
                    if (txtToSend.SelectionLength > 0)
                        txtToSend.SelectedText = "";
                    int i = -1;
                    for (i = txtToSend.SelectionStart; i > 0; i--)
                    {
                        if (!char.IsLetterOrDigit(txtToSend.Text[i-1]))
                            break;
                    }
                    if (i > -1)
                    {
                        txtToSend.Text = txtToSend.Text.Substring(0, Math.Max(i-1,0)) + txtToSend.Text.Substring(txtToSend.SelectionStart);
                        txtToSend.SelectionStart = i;
                    }
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    if (txtToSend.SelectionLength > 0)
                        txtToSend.SelectedText = "";
                    int i = -1;
                    for (i = txtToSend.SelectionStart-1; i < txtToSend.Text.Length-1; i++)
                    {
                        if (!char.IsLetterOrDigit(txtToSend.Text[i + 1]))
                            break;
                    }
                    if (i<txtToSend.Text.Length)
                    {
                        txtToSend.Text = txtToSend.Text.Substring(0, txtToSend.SelectionStart) + txtToSend.Text.Substring(i, Math.Max(0,txtToSend.Text.Length - 1 - i));
                        txtToSend.SelectionStart = i;
                    }
                    e.SuppressKeyPress = true;
                }
            }
		}
	}
}

