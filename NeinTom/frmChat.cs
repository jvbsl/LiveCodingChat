using System;
using System.Windows.Forms;
using System.Drawing;
using LiveCodingChat.Xmpp;
using LiveCodingChat;
using NeinTom.ChatLog;
using System.Collections.Generic;
namespace NeinTom
{
	public partial class frmChat
	{
		public frmChat ()
		{
			InitializeComponent ();

			pages = new Dictionary<string, TabPage> ();
			//TestControl ();//TODO: remove
		}
		private Dictionary<string,TabPage> pages;
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
			if (pages.ContainsKey (room.ID)) {
				return pages [room.ID];
			}
			pages.Add (room.ID, new TabPage ());
			TabPage page = pages[room.ID];
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
			tabControl.SelectedTab = tabPage;
			if (tabControl.TabCount > 1) {

				tabControl.ItemSize = new Size(125, 20);
				tabControl.SizeMode = TabSizeMode.Normal;
				tabControl.Appearance = TabAppearance.Normal;
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
		public void UserStateChanged(string roomID,User user,UserState state)
        {
			TabPage page = pages [roomID];
			ChatControl cht = (ChatControl)page.Controls [0];
            cht.UserStateChanged(user,state);
        }
		public void AddMessage(string roomID,LiveCodingChat.Xmpp.MessageReceivedEventArgs e)
		{
			TabPage page = pages [roomID];
			ChatControl cht = (ChatControl)page.Controls [0];
			cht.AddMessage (e);
           
		}
	}
}

