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
		}
	}
}

