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
		Dictionary<string,frmChat> chatForms;
		public frmMain ()
		{
			InitializeComponent ();
			chatForms = new Dictionary<string,frmChat> ();
			frmLogin frmLogin = new frmLogin ();
			if (frmLogin.ShowDialog () != System.Windows.Forms.DialogResult.OK) {
				this.Close ();
				Application.Exit ();
				return;
			}
			session = new LivecodingSession (LiveCodingChat.LoginFactory.Instance.CreateInstance(frmLogin.LoginMethod),frmLogin.Username);
			session.PasswordRequested += (object sender, ref string Password) => Password = frmLogin.Password;
			session.SessionAutenticated += Session_SessionAutenticated;;
			session.EnsureAuthenticated ();
		}

		void Session_SessionAutenticated (object sender, EventArgs e)
		{
			session.BeginOpenChat("jvbsl",EndOpenChat,null);
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
				frmChat frm = new frmChat ();
				frm.AddTabPage (frm.CreateTabPage (room.Room));
				frm.Show ();
				chatForms.Add (room.Room.ID, frm);
			}
		}
		void Room_Client_MessageReceived (LiveCodingChat.Xmpp.Room room, LiveCodingChat.Xmpp.MessageReceivedEventArgs e)
		{
			if (chatForms.ContainsKey (room.ID)) {
                RoomMessage(chatForms[room.ID],e);
			}
		}
        private void Room_UserStateChanged(LiveCodingChat.Xmpp.Room room, LiveCodingChat.User user, LiveCodingChat.Xmpp.UserState state)
        {
            if (chatForms.ContainsKey(room.ID))
            {
                RoomStateChanged(chatForms[room.ID], room,user,state);
            }
        }
        private void RoomStateChanged(frmChat frm,LiveCodingChat.Xmpp.Room room, LiveCodingChat.User user, LiveCodingChat.Xmpp.UserState state)
        {
            if (frm.InvokeRequired)
            {
                frm.Invoke(new MethodInvoker(delegate () { RoomStateChanged(frm, room,user,state); }));
                return;
            }
            frm.UserStateChanged(user, state);
        }
        private void RoomMessage(frmChat frm,LiveCodingChat.Xmpp.MessageReceivedEventArgs e)
        {
            if (frm.InvokeRequired)
            {
                frm.Invoke(new MethodInvoker(delegate() { RoomMessage(frm, e); }));
                return;
            }
            frm.Activate();
            frm.AddMessage(e);
        }
			
	}
}

