using System;
using System.Collections.Generic;
using System.Net;

namespace LiveCodingChat.Livecoding
{
	public class ChatRoom
	{
		public ChatRoom (LivecodingSession session,string id)
		{
			this.Session = session;
			this.ID = id;
		}
		public string ID{get;private set;}
		public string Resource{ get; private set; }
		public string RoomJID{ get; private set;}
		public string WebsocketURL{ get; private set; }
		public string PromiseID{ get; private set; }
		public string JID{ get; private set; }
		public string Password{ get; private set; }
		public Xmpp.XmppTest Client{ get; private set; }
		public Xmpp.Room Room{ get; private set; }
		public LivecodingSession Session{ get; private set; }

		public void Load(string json,string promiseID)
		{
			PromiseID = promiseID;
			json = json.Replace ("}", "").Replace ("{", "");
			string[] pairs = json.Split(',');
			foreach (string pair in pairs) {
				string[] p = System.Text.RegularExpressions.Regex.Unescape(pair.Trim ()).Split (new []{':'}, 2);
				switch (p [0]) {
				case "resource":
					Resource = HtmlHelper.getString(p [1].Trim());
					break;
				case "roomJid":
					RoomJID = HtmlHelper.getString(p [1].Trim());
					break;
				case "chatUrlWs":
					WebsocketURL = HtmlHelper.getString(p [1].Trim());
					break;
				}
			}
		}
		public void Login()
		{
			if (Client == null) {
				if (LoadXMPPData ()) {
					Client = new LiveCodingChat.Xmpp.XmppTest (this);
					Room = new LiveCodingChat.Xmpp.Room (ID, Client);
					Client.XmppAuthenticated += (object sender, EventArgs e) => {
						Client.Rooms.Add(Room.ID+"@chat.livecoding.tv",Room);
						Room.JoinRoom ();
						Room.QueryInfo ();
						Room.SendPresence ();
					};
				} else {
					//Session.Authenticated = false;
				}
			}
		}

		public bool setUserColor(string color)
		{
			Dictionary<string,string> postData = new Dictionary<string, string> ();
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create (LivecodingSession.PAGE+"/chat/set-nick-color.json");
			req.UserAgent = LivecodingSession.UserAgent;
			req.CookieContainer = Session.Cookies;
			req.Method = "POST";
			CookieCollection col = Session.Cookies.GetCookies (new System.Uri (LivecodingSession.PAGE));
			foreach(Cookie c in col)
				if (c.Name == "csrftoken")
					req.Headers["X-CSRFToken"]=c.Value;
			req.Headers ["X-Requested-With"] = "XMLHttpRequest";
			req.Referer = LivecodingSession.PAGE+"/chat/"+ID+"/";
			req.ContentType = "application/x-www-form-urlencoded";

			postData.Add ("color", "%23"+color);
			byte[] postBuild = HttpHelper.CreatePostData (postData);
			req.ContentLength = postBuild.Length;
			req.GetRequestStream ().Write (postBuild, 0, postBuild.Length);
			string data="";
			HttpWebResponse response = (HttpWebResponse)req.GetResponse ();
			if (response.StatusCode == HttpStatusCode.OK) {

				using (System.IO.StreamReader sr = new System.IO.StreamReader (response.GetResponseStream ())) {
					data = sr.ReadToEnd ();
					return true;
				}
			}
			return false;
		}
		private bool LoadXMPPData ()
		{
			Dictionary<string,string> postData = new Dictionary<string, string> ();
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create ("https://www.livecoding.tv/chat/"+ID+"/xmpp-session.json");
			req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
			req.CookieContainer = Session.Cookies;
			req.Method = "POST";
			CookieCollection col = Session.Cookies.GetCookies (new System.Uri ("https://www.livecoding.tv"));
			foreach(Cookie c in col)
				if (c.Name == "csrftoken")
					req.Headers["X-CSRFToken"]=c.Value;
			req.Headers ["X-Requested-With"] = "XMLHttpRequest";
			req.Referer = "https://www.livecoding.tv/chat/"+ID+"/";
			req.ContentType = "application/x-www-form-urlencoded";

			postData.Add ("promise_id", PromiseID);
			postData.Add ("websockets", "true");
			byte[] postBuild = HttpHelper.CreatePostData (postData);
			req.ContentLength = postBuild.Length;
			req.GetRequestStream ().Write (postBuild, 0, postBuild.Length);
			string data="";
			HttpWebResponse response = (HttpWebResponse)req.GetResponse ();
			if (response.StatusCode == HttpStatusCode.OK) {

				using (System.IO.StreamReader sr = new System.IO.StreamReader (response.GetResponseStream ())) {
					data = sr.ReadToEnd ();
				}
			}
			if (data == "null")
				return false;
			data = data.Replace ("{","").Replace ("}", "");
			string[] pairs = data.Split(',');
			Dictionary<string ,string> json = new Dictionary<string, string> ();
			foreach (string p in pairs) {
				string[] sp = p.Split (':');
				json.Add (sp [0].Replace ("\"", "").Trim (), sp [1].Replace ("\"", "").Trim ());
			}
			JID = json["jid"];
			Password = json["password"];

			return true;
		}
	}
}

