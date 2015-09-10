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

