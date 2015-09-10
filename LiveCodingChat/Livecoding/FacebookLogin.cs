using System;
using System.Net;
using System.Collections.Generic;

namespace LiveCodingChat
{
	public class FacebookLogin:ILoginMethod
	{
		public FacebookLogin ()
		{
		}

		#region ILoginMethod implementation

		public event LoginCompleted LoginCompleted;

		public void LoginAsync (string username, string password, ref System.Net.CookieContainer cookies)
		{
			Console.WriteLine ("Login Facebook");
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create ("https://www.livecoding.tv/accounts/facebook/login");
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
				data = HtmlHelper.getElement (data, "<form id=\"login_form\"");
			}


			LoginGithub (username, password,data, ref cookies);
		}
		private void LoginGithub(string username,string password,string data,ref CookieContainer cookies)
		{
			//POST login data
			Dictionary<string,string> postData = new Dictionary<string, string> ();
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create ("https://www.facebook.com"+HtmlHelper.Unescape(HtmlHelper.getAttribute(data,"action")));
			request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
			request.CookieContainer = cookies;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			postData.Add ("utf8", "\u2713");
			postData.Add ("authenticity_token", HtmlHelper.getAttribute(HtmlHelper.getSingleElement(data,"<input name=\"authenticity_token\""),"value"));
			postData.Add ("login", username);
			postData.Add ("password", password);
			postData.Add ("return_to",HtmlHelper.getAttribute(HtmlHelper.getSingleElement(data,"<input id=\"return_to\" name=\"return_to\""),"value"));

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

