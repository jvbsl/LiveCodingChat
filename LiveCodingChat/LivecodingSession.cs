using System;
using System.Net;
using System.Collections.Generic;

namespace LiveCodingChat.Livecoding
{
	public delegate void SessionAuthenticatedDelegate(object sender,System.EventArgs e);
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

		public event SessionAuthenticatedDelegate SessionAutenticated;
		public event PasswordRequestDelegate PasswordRequested;
		public LivecodingSession()
		{
		}
		public LivecodingSession (ILoginMethod loginMethod,string username)
		{
			this.Username = username;
			this.loginMethod = loginMethod;
			cookies = new CookieContainer ();
			this.loginMethod.LoginCompleted += LoginMethod_LoginCompleted;
		}

		void LoginMethod_LoginCompleted (object sender, CookieContainer cookies)
		{
			Authenticated = true;
			if (SessionAutenticated != null)
				SessionAutenticated (this, new System.EventArgs ());
		}
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
			return openChatDel.EndInvoke (res);
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

