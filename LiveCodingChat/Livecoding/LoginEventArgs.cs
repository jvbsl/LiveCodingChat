using System;
using System.Net;

namespace LiveCodingChat
{
	public class LoginEventArgs : EventArgs
	{
		public LoginEventArgs (string data,CookieContainer cookies)
		{
			int ind = data.IndexOf ("woopra.identify(");
			if (ind == -1)
				throw new ArgumentException ("No valid Login");
			int eInd = data.IndexOf (");", ind);
			if (eInd == -1)
				throw new ArgumentException ("No valid Login");

			string json = data.Substring (ind, eInd - ind - 1);
			ind = json.LastIndexOf (':');
			if (ind == -1)
				throw new ArgumentException ("No valid Login");
			ID = json.Substring (ind + 1).Trim ();
			ID = ID.Substring (1, ID.Length - 2);
			this.Cookies = cookies;
		}
		public string ID{get;private set;}
		public CookieContainer Cookies{get;private set;}
	}
}

