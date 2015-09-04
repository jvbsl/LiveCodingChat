using System;
using System.Net;
using System.Collections.Generic;

namespace LiveCodingChat
{
	public class EmailLogin:ILoginMethod
	{
		public EmailLogin ()
		{
		}

		#region ILoginMethod implementation

		public event LoginCompleted LoginCompleted;

		public void LoginAsync (string username, string password, ref System.Net.CookieContainer cookies)
		{
			Console.WriteLine ("Login Twitch");
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create ("https://www.livecoding.tv/accounts/login");
			request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
			request.CookieContainer = cookies;
			request.BeginGetResponse (EndGetResponse,new object[]{request,username,password,cookies});
		}
		private void EndGetResponse(IAsyncResult res)
		{
			object[] obj = (object[])res.AsyncState;
			string username = (string)obj [1],password= (string)obj [2];
			CookieContainer cookies = (CookieContainer)obj [3];

			HttpWebRequest request = (HttpWebRequest)obj[0];
			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(res);
			string data;//todo: cookie
			using (System.IO.StreamReader sr = new System.IO.StreamReader (response.GetResponseStream ())) {
				data = sr.ReadToEnd ();
				data = HtmlHelper.getElement (data, "<form class=\"form-inline clearfix\"");
			}

			LoginNormal (username, password, data, ref cookies);
		}
		private void LoginNormal(string username,string password,string data,ref CookieContainer cookies)
		{
			//POST login data
			Dictionary<string,string> postData = new Dictionary<string, string> ();
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create ("https://www.livecoding.tv/accounts/login/");
			request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
			request.CookieContainer = cookies;
			request.Method = "POST";
			request.Referer = "https://www.livecoding.tv/accounts/login/";
			request.ContentType = "application/x-www-form-urlencoded";

			postData.Add ("csrfmiddlewaretoken", HtmlHelper.getAttribute(HtmlHelper.getSingleElement(data,"<input type='hidden' name='csrfmiddlewaretoken'"),"value"));
			postData.Add ("login", username);
			postData.Add ("password", password);
			byte[] postBuild = HttpHelper.CreatePostData (postData);
			request.ContentLength = postBuild.Length;
			request.GetRequestStream ().Write (postBuild, 0, postBuild.Length);

			HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
			using (System.IO.StreamReader sr = new System.IO.StreamReader (response.GetResponseStream ())) {
				data = sr.ReadToEnd();
			}

			if (LoginCompleted != null)
				LoginCompleted (this, cookies);
		}
		#endregion
	}
}

