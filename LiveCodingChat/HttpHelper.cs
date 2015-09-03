using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace LiveCodingChat
{
	public class HttpHelper
	{
		public static List<KeyValuePair<string,string>> ConvertCookies(CookieContainer cont)
		{
			List<KeyValuePair<string,string>> cookieList = new List<KeyValuePair<string, string>> ();
			foreach (Cookie c in cont.GetCookies(new System.Uri("https://www.livecoding.tv")))
				cookieList.Add (new KeyValuePair<string,string>(c.Name, c.Value));
			return cookieList;
		}
		public static byte[] CreatePostData (Dictionary<string,string> postData)
		{
			StringBuilder bld = new StringBuilder ();
			foreach (KeyValuePair<string,string> pair in postData) {
				if (bld.Length != 0)
					bld.Append("&");
				if (pair.Value == "")
					bld.Append (HttpUtility.UrlEncode(pair.Key));
				else
					bld.Append (HttpUtility.UrlEncode(pair.Key) + "=" + HttpUtility.UrlEncode(pair.Value));
			}
			return System.Text.Encoding.UTF8.GetBytes (bld.ToString ());

		}
	}
}

