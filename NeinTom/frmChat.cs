using System;
using System.Windows.Forms;
using System.Drawing;
using LiveCodingChat.Xmpp;
using LiveCodingChat;


namespace NeinTom
{
	public partial class frmChat
	{
		public frmChat ()
		{
			InitializeComponent ();

			TestControl ();//TODO: remove
		}

		public void TestControl()
		{
			TabPage page = new TabPage ();
			page.Text = "TestPage";
			ChatLog log = new ChatLog ();
			log.Dock = DockStyle.Fill;
			page.Controls.Add (log);
			User user = new User ("testid");
			user.Color = Color.Red;
			log.AddMessage (new ChatMessage (user, "<message from='bobstriker@chat.livecoding.tv/jvbsl' to='jvbsl@livecoding.tv/web-bobstriker-s56WnvDq-popout' type='groupchat' id='87'><body xmlns='jabber:client'>zumindest <b>zeit<i>lich</i></b> gesehen</body><x xmlns='jabber:x:event'><composing/></x></message>"));
			log.AddMessage (new ChatMessage (user, "<message from='bobstriker@chat.livecoding.tv/jvbsl' to='jvbsl@livecoding.tv/web-bobstriker-s56WnvDq-popout' type='groupchat' id='87'><body xmlns='jabber:client'>haha <b>test<i>5873</i></b> bla</body><x xmlns='jabber:x:event'><composing/></x></message>"));
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
		public void AddMessage(LiveCodingChat.Xmpp.MessageReceivedEventArgs e)
		{
			int tab = 0;
			ChatControl cht = (ChatControl)tabControl.TabPages [tab].Controls [0];
			cht.AddMessage (e);
		}
	}
}

