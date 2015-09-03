using System;
using WebSocket4Net;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml;


namespace LiveCodingChat.Xmpp
{
	public delegate void MessageReceivedDelegate(Room room,string nick,string message);
	public delegate void AuthenticatedDelegate(object sender,EventArgs e);
	public delegate void XMPPMessage(XmlElement el);
	public class XmppTest
	{
		WebSocket socket;
		private string uri;
		private delegate void AuthMethod(string jid,string password);
		private Dictionary<string,XMPPMessage> messageTypes;
		//private Dictionary<string,AuthMethod> authMethods;
		private Dictionary<string,XMPPMessage> iqMethods;
		public event MessageReceivedDelegate MessageReceived;
		public event AuthenticatedDelegate XmppAuthenticated;
		enum eState
		{
			None,
			Opening,
			Features,
			Authenticate,
			SendRessource,
			Session,
			Message,
		}
		private eState state;
		private bool keepAlive = true;
		private System.Timers.Timer keepAliveTimer;
		private string ressource;
		public XmppTest (Livecoding.ChatRoom client)
		{
			Rooms = new Dictionary<string, Room> ();
			IDManager = new IDManager ();
			JID = client.JID;
			Password = client.Password;
			ressource = client.Resource;
			Nick = JID.Split ('@') [0];

			keepAliveTimer = new System.Timers.Timer (1000 * 60);
			keepAliveTimer.Elapsed+= KeepAliveTimer_Elapsed;
			messageTypes = new Dictionary<string, XMPPMessage> ();
			iqMethods = new Dictionary<string, XMPPMessage> ();
			messageTypes.Add ("open", XMPP_Open);
			messageTypes.Add ("stream:features", XMPP_Features);
			messageTypes.Add ("success", XMPP_Success);
			messageTypes.Add ("iq", XMPP_IQ);
			messageTypes.Add ("close", XMPP_Close);
			messageTypes.Add ("message", XMPP_Message);
			messageTypes.Add ("presence", XMPP_Presence);


			List<KeyValuePair<string,string>> customHeaders=new List<KeyValuePair<string,string>>();
			customHeaders.Add(new KeyValuePair<string,string>("Sec-WebSocket-Extensions","permessage-deflate"));
			string origin = "https://www.livecoding.tv";//TODO:
			string UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
			uri = "wss://www.livecoding.tv:443" + client.WebsocketURL;
			socket = new WebSocket (uri,"xmpp",HttpHelper.ConvertCookies(client.Session.Cookies),customHeaders,UserAgent,origin);
			socket.MessageReceived += Socket_MessageReceived;
			socket.Open ();
			socket.Opened+= Socket_Opened;
		}
		public Dictionary<string,Room> Rooms{ get; private set; }
		public IDManager IDManager{ get; private set; }
		public string Nick{ get; private set; }
		public string JID{ get; private set; }
		public string Password{ get; private set; }
		public bool Authenticated{ get; private set; }
		private void XMPP_Open(XmlElement node)
		{
			node.Attributes.GetNamedItem ("");
			state = eState.Features;


		}
		private void XMPP_Close(XmlElement node)
		{
			keepAliveTimer.Stop ();
			socket.Close ();
		}

		void KeepAliveTimer_Elapsed (object sender, System.Timers.ElapsedEventArgs e)
		{
			Ping ();
		}
		private void XMPP_Features(XmlElement node)
		{

			foreach (XmlNode n in node.ChildNodes) {
				string v = n.Value;
			}
			node.Attributes.GetNamedItem ("");
			if (Authenticated)
				SendRessource ();
			else
				Login (JID,Password);
			//Login (JID.Split ('@') [0], Password);

		}
		private void XMPP_Success(XmlElement node)
		{
			if (!Authenticated) {
				Authenticated = true;
				state = eState.Session;
				OpenStream ();
			}
		}
		private void XMPP_IQ(XmlElement element)
		{
			string id = element.Attributes.GetNamedItem ("id").Value;
			if (iqMethods.ContainsKey (id)) {
				if (iqMethods[id]!= null)
					iqMethods[id] (element);
				iqMethods.Remove(id);
			}
		}
		private static string getValue(XmlNode val)
		{
			return val == null ? null : val.Value;
		}
		private void XMPP_Presence(XmlElement element)
		{
			if (element.Attributes.GetNamedItem ("to").Value == JID) {
				string[] frm = element.Attributes.GetNamedItem ("from").Value.Split ('/');
				string roomId = frm[0];
				string userId = frm[1];
				if (Rooms.ContainsKey (frm [0])) {
					Room room = Rooms [roomId];
					User user;
					if (room.Users.ContainsKey(userId))
						user = room.Users [userId];
					else
						room.Users.Add (userId, user = new User (userId));

					foreach (XmlElement el in element.ChildNodes) {
						if (el.Name == "x" && el.FirstChild.Name == "item") {
							string affiliation = getValue(el.FirstChild.Attributes.GetNamedItem ("admin"));
							string role = getValue(el.FirstChild.Attributes.GetNamedItem ("role"));
							string premium = getValue(el.FirstChild.Attributes.GetNamedItem ("premium"));
							string staff = getValue(el.FirstChild.Attributes.GetNamedItem ("staff"));
							string color = getValue(el.FirstChild.Attributes.GetNamedItem ("color"));
							user.Affiliation = affiliation ?? user.Affiliation;
							user.Role = role ?? user.Role;
							user.Premium = premium == null ? user.Premium : bool.Parse (premium);
							user.Staff = staff == null ? user.Staff : bool.Parse (staff);
							user.Color = color == null ? user.Color : System.Drawing.Color.FromArgb (Convert.ToInt32(color.Substring (1),16));
						}
					}
				}
			}
		}
		private void XMPP_Message(XmlElement element)
		{
			if (element.Attributes.GetNamedItem ("to").Value == JID) {
				string frm = element.Attributes.GetNamedItem ("from").Value;
				string key = frm.Split ('/') [0];
				if (Rooms.ContainsKey(key)){
					Room r = Rooms [key];
					string user = frm.Substring (key.Length+1), message = "";
					DateTime stamp = DateTime.Now;
					foreach (XmlElement el in element.ChildNodes) {
						if (el.Name == "body") {
							message = el.InnerXml;
						} else if (el.Name == "delay") {
							string stmp = el.Attributes.GetNamedItem ("stamp").Value;
							stamp = DateTime.Parse (stmp);
						}
					}
					if (MessageReceived != null)
						MessageReceived (r, user, "[" + stamp.ToString () + "]" + message);
				}
			}
		}

		private void Login(string jid,string password)
		{
			state = eState.Authenticate;
			StringBuilder sb = new StringBuilder ();

			byte[] data = new byte[Nick.Length+jid.Length + password.Length + 2];
			Array.Copy (System.Text.Encoding.UTF8.GetBytes (jid), 0, data, 1, jid.Length);
			data [jid.Length] = 0;
			Array.Copy (System.Text.Encoding.UTF8.GetBytes (Nick), 0, data, 1+jid.Length, Nick.Length);
			data [Nick.Length+jid.Length+1] = 0;
			Array.Copy (System.Text.Encoding.UTF8.GetBytes (password), 0, data, 2+jid.Length+Nick.Length, password.Length);
			sb.Append ("<auth xmlns='urn:ietf:params:xml:ns:xmpp-sasl'");
			sb.Append(" mechanism='PLAIN'>");
			//
			sb.Append(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("\0"+Nick+"\0"+password)));
			sb.Append("</auth>");

			Send (sb.ToString ());

		}
		private void SendRessource()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append ("<iq type='set' id='_bind_auth_2' xmlns='jabber:client'>");
			sb.Append (" <bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'>");
			sb.Append ("  <resource>");
			sb.Append (ressource);
			sb.Append ("</resource>");
			sb.Append (" </bind>");
			sb.Append ("</iq>");

			iqMethods.Add("_bind_auth_2",new XMPPMessage(delegate(XmlElement el) {
				if(el.FirstChild.Name == "bind")
				{
					el = (XmlElement)el.FirstChild;
					if(el.FirstChild.Name == "jid")
					{
						JID = el.FirstChild.InnerText;
						StartSession();
					}
				}
			}));

			Send (sb.ToString ());
		}
		private void StartSession()
		{
			StringBuilder sb = new StringBuilder ();

			sb.Append ("<iq type='set' id='_session_auth_2' xmlns='jabber:client'><session xmlns='urn:ietf:params:xml:ns:xmpp-session'/></iq>");

			iqMethods.Add("_session_auth_2",new XMPPMessage(delegate(XmlElement el) {
				if (keepAlive) {
					keepAliveTimer.Start ();
				}
				Join();
				if (XmppAuthenticated != null)
					XmppAuthenticated(this,new System.EventArgs());
			}));

			Send(sb.ToString());
		}
		private void Join()//TODO:naming
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append("<presence id='pres:"+IDManager.SendPresenceId().ToString()+"' xmlns='jabber:client'>");
			sb.Append(" <priority>1</priority>");
			sb.Append (" <c xmlns='http://jabber.org/protocol/caps' hash='sha-1' node='https://candy-chat.github.io/candy/' ver='kR9jljQwQFoklIvoOmy/GAli0gA='/>");
			sb.Append ("</presence>");
			Send (sb.ToString ());
		}
		private void Ping()
		{

			StringBuilder sb = new StringBuilder ();
			sb.Append ("<iq type='get'");
			sb.Append (" to='livecoding.tv'");
			sb.Append (" xmlns='jabber:client'");
			sb.Append (" id='");
			sb.Append (IDManager.SendIqId().ToString());
			sb.Append (":sendIQ'>");
			sb.Append ("<ping xmlns='urn:xmpp:ping'/>");
			sb.Append("</iq>");

			Send (sb.ToString ());
		}

		private void OpenStream()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append ("<open xmlns='urn:ietf:params:xml:ns:xmpp-framing'");
			sb.Append (" to='livecoding.tv'");
			sb.Append (" version='1.0' />");

			Send (sb.ToString());
		}
		private void Socket_MessageReceived (object sender, MessageReceivedEventArgs e)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			//Console.WriteLine (e.Message);//TODO:
			Console.ForegroundColor = ConsoleColor.White;
			XmlDocument doc= new XmlDocument();
			doc.LoadXml (e.Message);
			XmlElement node = (XmlElement)doc.FirstChild;
			if (messageTypes.ContainsKey(node.Name))
			{
				XMPPMessage msg = messageTypes[node.Name];
				if (msg != null)
					msg (node);
			}

		}

		private void Socket_Opened (object sender, EventArgs e)
		{
			state = eState.Opening;
			OpenStream ();
		}


		public void Send(string xml,string type=null,string id=null,XMPPMessage del=null)
		{
			if (type != null && del != null) {
				if (type == "iq") {
					iqMethods.Add (id, del);
				}
			}
			Console.ForegroundColor = ConsoleColor.Red;
			//Console.WriteLine (xml);//TODO:
			Console.ForegroundColor = ConsoleColor.White;
			socket.Send (xml);
		}
	}
}

