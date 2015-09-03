using System;
using System.Drawing;

namespace LiveCodingChat
{
	public class User
	{
		public User (string userID)
		{
			ID = userID;
			Color = System.Drawing.Color.Black;
		}
		public string ID{ get; private set; }
		public string Affiliation{ get;  set; }
		public string Role{ get;  set; }
		public bool Premium{ get;  set; }
		public bool Staff{ get;  set; }
		public Color Color{ get;  set; }

		public override string ToString ()
		{
			return ID + "[" + Role + "]";
			//return string.Format ("[User: ID={0}, Affiliation={1}, Role={2}, Premium={3}, Staff={4}, Color={5}]", ID, Affiliation, Role, Premium, Staff, Color);
		}
	}
}

