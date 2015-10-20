using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
namespace NeinTom
{
	public class frmTest:Form
	{
		private ChatControl ctl;
		private Dictionary<string,LiveCodingChat.User> users;
		string[] userIDs = new string[]{ "jvbsl", "susch19" };
		private Random random;
		public frmTest ()
		{
			users = new Dictionary<string, LiveCodingChat.User> ();
			random = new Random ();
			ctl = new ChatControl ();
			ctl.Dock = DockStyle.Fill;

			this.Controls.Add (ctl);

			PopulateUsers ();

			AddPseudo ();
		}

		private void PopulateUsers()
		{
			for (int i=0;i<userIDs.Length;i++){
				LiveCodingChat.User user = new LiveCodingChat.User(userIDs[i]);
				user.Color = Color.FromArgb(255,random.Next(0,255),random.Next(0,255),random.Next(0,255));
				user.Affiliation = "none";
				this.users.Add (user.ID, user);
			}
		}
		private LiveCodingChat.User getRandomUser()
		{
			return users [userIDs [random.Next (0, userIDs.Length)]];
		}
		public void AddPseudo()
		{
			ctl.AddMessage(new LiveCodingChat.Xmpp.MessageReceivedEventArgs(getRandomUser(),"random mes<b>sage</b> ;)",new DateTime()));
		}
	}
}

