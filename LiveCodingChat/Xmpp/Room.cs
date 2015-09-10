using System;
using WebSocket4Net;
using System.Text;
using System.Collections.Generic;


namespace LiveCodingChat.Xmpp
{
    public enum UserState
    {
        Available,
        Offline
    }
    public delegate void UserStateChanged(User user,UserState state);
	public class Room
	{
        public event UserStateChanged UserStateChanged;
		private XmppTest xmpp;
		public Room (string id,XmppTest xmpp)
		{
			Users = new Dictionary<string, User> ();
			this.xmpp = xmpp;
			this.ID = id;
		}
		public string ID{ get; private set; }
		public Dictionary<string,User> Users{ get; private set; }
        public void UserJoined(string id,User user)
        {
            Users.Add(id, user);
            if (UserStateChanged != null)
                UserStateChanged(user,UserState.Available);
        }
        public void UserLeft(string id)
        {
            if (Users.ContainsKey(id)){
                if (UserStateChanged != null)
                    UserStateChanged(Users[id],UserState.Offline);
                Users.Remove(id);

            }
        }
		public void JoinRoom()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append ("<presence to='"+ID+"@chat.livecoding.tv/"+xmpp.Nick+"' id='pres:"+xmpp.IDManager.SendPresenceId().ToString()+"' xmlns='jabber:client'>");
			sb.Append (" <x xmlns='http://jabber.org/protocol/muc'/>");
			sb.Append (" <c xmlns='http://jabber.org/protocol/caps' hash='sha-1' node='https://candy-chat.github.io/candy/' ver='kR9jljQwQFoklIvoOmy/GAli0gA='/>");
			sb.Append ("</presence>");

			xmpp.Send (sb.ToString ());
		}
		public void QueryInfo()
		{
			StringBuilder sb = new StringBuilder ();
			string id = xmpp.IDManager.SendIqId().ToString()+":sendIQ";
			sb.Append ("<iq type='get' from='" + xmpp.Nick + "@livecoding.tv' to='" + ID + "@chat.livecoding.tv' xmlns='jabber:client' id='" + id + "'>");
			sb.Append (" <query xmlns='http://jabber.org/protocol/disco#info'/>");
			sb.Append ("</iq>");
			xmpp.Send (sb.ToString (), "iq", id, null);//TODO: response

		}
		public void SendPresence()
		{
			string id="pres:"+xmpp.IDManager.SendPresenceId().ToString();
			StringBuilder sb = new StringBuilder ();
			sb.Append ("<presence to='" + ID + "@chat.livecoding.tv/" + xmpp.Nick + "' id='"+id.ToString()+"' xmlns='jabber:client'>");
			sb.Append (" <x xmlns='http://jabber.org/protocol/muc'/>");
			sb.Append (" <c xmlns='http://jabber.org/protocol/caps' hash='sha-1' node='https://candy-chat.github.io/candy/' ver='kR9jljQwQFoklIvoOmy/GAli0gA='/>");
			sb.Append ("</presence>");

			xmpp.Send (sb.ToString (), "presence", id, null);
		}
		public void QueryList()
		{
			StringBuilder sb = new StringBuilder ();
			string id = xmpp.IDManager.SendIqId().ToString()+":sendIQ";
			sb.Append ("<iq type='get' from='"+xmpp.Nick+"@livecoding.tv' xmlns='jabber:client' id='"+id+"'>");
			sb.Append (" <query xmlns='jabber:iq:privacy'>");
			sb.Append (" <list name='ignore'/>");
			sb.Append (" </query>");
			sb.Append ("</iq>");

			xmpp.Send (sb.ToString (), "iq", id, null);
		}
        public void SendMessage(string message,User user)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<message to='" + ID + "@chat.livecoding.tv/" + user.ID +"' from='" + xmpp.JID + "' type='chat' id='12' xmlns='jabber:client'>");
            sb.Append(" <body xmlns='jabber:client'>" + message + "</body>");
            sb.Append(" <x xmlns='jabber:x:event'><composing/></x>");
            sb.Append("</message>");
            xmpp.Send(sb.ToString());
        }
		public void SendMessage(string message)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append ("<message to='" + ID + "@chat.livecoding.tv' from='" + xmpp.JID + "' type='groupchat' id='12' xmlns='jabber:client'>");
			sb.Append (" <body xmlns='jabber:client'>" + message + "</body>");
			sb.Append(" <x xmlns='jabber:x:event'><composing/></x>");
			sb.Append("</message>");
			xmpp.Send (sb.ToString ());
		}
	}
}

