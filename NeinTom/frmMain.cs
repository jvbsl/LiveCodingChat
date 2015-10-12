using System;
using System.Windows.Forms;
using LiveCodingChat.Livecoding;
using System.Collections.Generic;


namespace NeinTom
{
	public partial class frmMain
	{
		LivecodingSession session;
		ChatRoom room;
		Dictionary<string,frmChat> channels;
		List<frmChat> chatForms;
		public frmMain ()
		{
			InitializeComponent ();
            string m= ChatLog.SmileyPart.Match;
			chatForms = new List<frmChat> ();
            channels = new Dictionary<string,frmChat> ();
			frmLogin frmLogin = new frmLogin ();
			if (frmLogin.ShowDialog () != System.Windows.Forms.DialogResult.OK) {
				this.Close ();
				Application.Exit ();
				return;
			}
			session = new LivecodingSession (LiveCodingChat.LoginFactory.Instance.CreateInstance(frmLogin.LoginMethod),frmLogin.Username);
			session.PasswordRequested += (object sender, ref string Password) => Password = frmLogin.Password;
			session.SessionAutenticated += Session_SessionAutenticated;
			session.FollowedChanged += Session_FollowedChanged;;
			session.EnsureAuthenticated ();
		}

		void Session_FollowedChanged (object sender, EventArgs e)
		{
			if (this.InvokeRequired) {
				this.Invoke (new MethodInvoker (delegate() {
					Session_FollowedChanged (sender, e);
				}));
				return;
			}
			this.lstUsers.Items.Clear ();
			this.lstUsers.Items.AddRange (session.Followed.ToArray());
		}

		void Session_SessionAutenticated (object sender, EventArgs e)
		{
			if (InvokeRequired) {
				this.Invoke (new MethodInvoker (delegate() {
					Session_SessionAutenticated (sender, e);
				}));
				return;
			}
			this.lstUsers.Enabled = true;
		}
		void LstUsers_MouseDoubleClick (object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (lstUsers.SelectedItem == null)
				return;
			session.BeginOpenChat(lstUsers.SelectedItem.ToString(),new AsyncCallback(EndOpenChat),null);
		}
		private void EndOpenChat(IAsyncResult res)
		{
			room = session.EndOpenChat (res);
			if (room == null) {
				MessageBox.Show ("Error occured while connecting to chat!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit ();
				return;
			}
			room.Client.MessageReceived += Room_Client_MessageReceived;
            room.Room.UserStateChanged += Room_UserStateChanged;
			CreateForm ();
		}

        private void CreateForm()
		{
			if (this.InvokeRequired) {
				this.Invoke (new MethodInvoker(delegate() {
					CreateForm();
				}));
			}else{
				if (chatForms.Count == 0)
					chatForms.Add (new frmChat ());
				frmChat frm = chatForms [0];
				frm.AddTabPage (frm.CreateTabPage (room.Room));
				frm.Show ();
				frm.Activate ();
				channels.Add (room.Room.ID, frm);
			}
		}
		void Room_Client_MessageReceived (LiveCodingChat.Xmpp.Room room, LiveCodingChat.Xmpp.MessageReceivedEventArgs e)
		{
			if (channels.ContainsKey (room.ID)) {
				RoomMessage(channels[room.ID],room.ID,e);
			}
		}
        private void Room_UserStateChanged(LiveCodingChat.Xmpp.Room room, LiveCodingChat.User user, LiveCodingChat.Xmpp.UserState state)
        {
            if (channels.ContainsKey(room.ID))
            {
                RoomStateChanged(channels[room.ID], room,user,state);
            }
        }
        private void RoomStateChanged(frmChat frm,LiveCodingChat.Xmpp.Room room, LiveCodingChat.User user, LiveCodingChat.Xmpp.UserState state)
        {
            if (frm.InvokeRequired)
            {
                frm.Invoke(new MethodInvoker(delegate () { RoomStateChanged(frm, room,user,state); }));
                return;
            }
			frm.UserStateChanged(room.ID,user, state);
        }
        private void RoomMessage(frmChat frm,string roomID,LiveCodingChat.Xmpp.MessageReceivedEventArgs e)
        {
            if (frm.InvokeRequired)
            {
                frm.Invoke(new MethodInvoker(delegate() { RoomMessage(frm,roomID, e); }));
                return;
            }
            frm.Activate();
            frm.AddMessage(roomID,e);
        }
			
	}
}

