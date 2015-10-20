using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
namespace LiveCodingChat.Livecoding
{
	public delegate void SessionAuthenticatedDelegate(object sender,System.EventArgs e);
	public delegate void FollowersChangedDelegate(object sender,System.EventArgs e);
	public delegate void FollowedChangedDelegate(object sender,System.EventArgs e);
	public delegate void PasswordRequestDelegate(object sender,ref string Password);
	[Serializable()]
	public class LivecodingSession
	{
		public const string PROTOCOL = "https";
		public const string HOST = "livecoding.tv";
		public const string PAGE = PROTOCOL + "://www." + HOST+"/";
		public const string UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";

		private CookieContainer cookies;
		private ILoginMethod loginMethod;
		private string rssKey;

		public event SessionAuthenticatedDelegate SessionAutenticated;
		public event PasswordRequestDelegate PasswordRequested;
		public event FollowersChangedDelegate FollowersCanged;
		public event FollowedChangedDelegate FollowedChanged;
		public string LiveCodingID{get;private set;}
		public LivecodingSession()
		{
		}
		public LivecodingSession (ILoginMethod loginMethod,string username)
		{
			Followed = new List<string> ();
			Followers = new List<string> ();
			this.Username = username;
			this.loginMethod = loginMethod;
			cookies = new CookieContainer ();
			this.loginMethod.LoginCompleted += LoginMethod_LoginCompleted;
		}
		
		void LoginMethod_LoginCompleted (object sender, LoginEventArgs e)
		{
			Authenticated = true;
			LiveCodingID = e.ID;
			LoadFollowInfos (true);
			LoadFollowInfos (false);
			if (SessionAutenticated != null)
				SessionAutenticated (this, new System.EventArgs ());
		}
		private void LoadFollowInfos(bool following)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (PAGE + "rss/" + this.LiveCodingID.ToLower() + "/" + (following?"followed":"follower") + "/?key="+getRSSKey());
			request.BeginGetResponse (new AsyncCallback (EndLoadFollowInfos), new object[]{ request, following });
		}
		private void EndLoadFollowInfos(IAsyncResult res)
		{
			object[] tmp = (object[])res.AsyncState;
			HttpWebRequest request = (HttpWebRequest)tmp [0];
			bool following = (bool)tmp [1];
			try{
				WebResponse response = request.EndGetResponse (res);
				if (following)
					Followed.Clear();
				else
					Followers.Clear();
				using (StreamReader stream = new StreamReader(response.GetResponseStream ())) {
					XmlDocument doc = new XmlDocument ();
					doc.LoadXml(stream.ReadToEnd());
					LoadFollowingRSS(doc,following);
					//doc.ChildNodes[
				}
			}
			catch(Exception ex)
			{
				
			}
		}
		private IEnumerable<XmlElement> getElements(XmlNodeList list,string name)
		{
			foreach (XmlNode node in list) {
				if (node.NodeType == XmlNodeType.Element && node.Name == name) {
					yield return (XmlElement)node;
				}
			}
			yield break;
		}
		private void LoadFollowingRSS(XmlDocument document,bool following)
		{
			
			XmlElement rss = getElements (document.ChildNodes, "rss").FirstOrDefault ();
			XmlElement channel = getElements (rss.ChildNodes, "channel").FirstOrDefault ();
			foreach (XmlElement item in getElements(channel.ChildNodes,"item")) {
				string name = getElements (item.ChildNodes, "title").FirstOrDefault ().InnerText;
				if (following)
					Followed.Add (name);
				else
					Followers.Add (name);
			}
			if (following && FollowedChanged != null)
				FollowedChanged (this, new System.EventArgs ());
			else if (!following && FollowersCanged != null)
				FollowersCanged (this, new System.EventArgs ());
		}
		public string getRSSKey()
		{
			if (rssKey == null) {
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (PAGE + this.LiveCodingID.ToLower() + "/settings/followers/");
				request.CookieContainer = cookies;
				using (StreamReader r = new StreamReader (request.GetResponse ().GetResponseStream ())) {
					string data = HtmlHelper.getAttribute(HtmlHelper.getElement(r.ReadToEnd (),"<a class=\"feed-link\""),"href");
					rssKey = data.Substring(data.LastIndexOf("?key=")+5);
				}
			}
			return rssKey;
		}
		public List<string> Followers{ get; private set; }

		public List<string> Followed{ get; private set; }
		public string Username { get; private set; }
		public CookieContainer Cookies{ get{return cookies; }}

		public bool Authenticated{ get; private set; }

		public void EnsureAuthenticated()
		{
			if (Authenticated) {
				if (SessionAutenticated != null)
					SessionAutenticated (this, new System.EventArgs ());
			} else {
				if (PasswordRequested != null) {
					string password = "";
					PasswordRequested (this, ref password);
					BeginAuthenticate (password);
				}
			}
		}
		private void BeginAuthenticate(string password)
		{
			CookieContainer cookies = Cookies;
			loginMethod.LoginAsync (Username, password,ref cookies);
		}
		private delegate ChatRoom OpenChatDel (string room);
		private OpenChatDel openChatDel=null;
		public IAsyncResult BeginOpenChat(string room,AsyncCallback callback,object @object)
		{
			if (openChatDel != null)
				throw new InvalidOperationException ("Can't open multiple Chats at a time");
			openChatDel = new OpenChatDel (OpenChat);
			return openChatDel.BeginInvoke (room, callback,@object);
		}
		public ChatRoom EndOpenChat(IAsyncResult res)
		{
			if (openChatDel == null)
				throw new InvalidOperationException ("Something bad happened");
			OpenChatDel tmp = openChatDel;
			openChatDel = null;
			return tmp.EndInvoke (res);
		}
		private ChatRoom OpenChat (string room)
		{
			string url = PAGE+ "chat/" + room + "/";
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create (url);
			req.UserAgent = UserAgent;
			req.CookieContainer = cookies;
			HttpWebResponse response = (HttpWebResponse)req.GetResponse ();
			if (response.StatusCode == HttpStatusCode.OK) {
				ChatRoom chatRoom = new ChatRoom (this,room);
				using (System.IO.StreamReader sr = new System.IO.StreamReader (response.GetResponseStream ())) {
					string data = sr.ReadToEnd ();
					int fInd = data.IndexOf ("window.Chat.init(");
					//Console.WriteLine (data);
					if (fInd == -1)
						return null;
					Console.WriteLine ("Found start");
					fInd += "window.Chat.init(".Length;
					int eInd = data.IndexOf (");", fInd);
					if (eInd == -1)
						return null;
					Console.WriteLine ("Found end");
					string json = data.Substring (fInd, eInd - fInd - 1);
					fInd = data.IndexOf ("window.Chat.connect(", eInd);
					if (fInd == -1)
						return null;
					eInd = data.IndexOf (");", fInd);
					if (eInd == -1)
						return null;//TODO: exception trowing?
					string tmp = data.Substring (fInd, eInd - fInd - 1);
					tmp = tmp.Split (',') [1];
					tmp = tmp.Split (':') [1].Trim ().Replace ("\"", "");
					chatRoom.Load (json, tmp);

				}
				chatRoom.Login ();
				return chatRoom;
			}
			Console.WriteLine ("Not OK");
			return null;
		}
	}
}

