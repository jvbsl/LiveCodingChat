using System;
using System.Windows.Forms;
using System.Drawing;
using LiveCodingChat.Xmpp;
using LiveCodingChat;
using NeinTom.ChatLog;

namespace NeinTom
{
	public partial class frmChat
	{
		public frmChat ()
		{
			InitializeComponent ();

			//TestControl ();//TODO: remove
		}

		public void TestControl()
		{
			TabPage page = new TabPage ();
			page.Text = "TestPage";
			ChatLogControl log = new ChatLogControl ();
			log.Dock = DockStyle.Fill;
			page.Controls.Add (log);
			User user = new User ("testid");
			user.Color = Color.Red;
			log.AddMessage (new ChatMessage (log,user, "zumindest <b>zeit<i>lich</i></b> gesehen"));
			log.AddMessage (new ChatMessage (log,user, "haha <b>test<i>5873</i></b> bla"));
            log.AddMessage(new ChatMessage(log, user, "unformatierter test text bla susch sieht das hier niemals"));
            AddTabPage (page);
		}

		public TabPage CreateTabPage(Room room)
		{
			TabPage page = new TabPage ();
			page.Text = "Group: " + room.ID;
			ChatControl cht = new ChatControl ();
			cht.Dock = DockStyle.Fill;
			cht.Room = room;
			page.Controls.Add (cht);
			return page;
		}
		public TabPage CreateTabPage(User user)
		{
			TabPage page = new TabPage ();
			page.Text = user.ID;
			return page;
		}
		public void AddTabPage(TabPage tabPage)
		{
			tabControl.TabPages.Add (tabPage);
			if (tabControl.TabCount > 1) {
				tabControl.Appearance = TabAppearance.Normal;
				tabControl.ItemSize = new Size(0, 1);
				tabControl.SizeMode = TabSizeMode.Normal;
			}

		}
		public void RemoveTabPage(TabPage tabPage)
		{
			tabControl.Appearance = TabAppearance.FlatButtons;
			tabControl.ItemSize = new Size(0, 1);
			tabControl.SizeMode = TabSizeMode.Fixed;
		}

		void TabControl_TabIndexChanged (object sender, EventArgs e)
		{
			if (tabControl.TabIndex == -1)
				return;
			this.Text = "Chat - " + tabControl.SelectedTab.Text;
		}
        public void UserStateChanged(User user,UserState state)
        {
            int tab = 0;
            ChatControl cht = (ChatControl)tabControl.TabPages[tab].Controls[0];
            cht.UserStateChanged(user,state);
        }
		public void AddMessage(LiveCodingChat.Xmpp.MessageReceivedEventArgs e)
		{
			int tab = 0;
			ChatControl cht = (ChatControl)tabControl.TabPages [tab].Controls [0];
			cht.AddMessage (e);
           
		}
	}
}

