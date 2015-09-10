using System;
using System.Net;
using System.Collections.Generic;

namespace LiveCodingChat
{
	public class LinkedInLogin:ILoginMethod
	{
		public LinkedInLogin ()
		{
		}

		#region ILoginMethod implementation

		public event LoginCompleted LoginCompleted;

		public void LoginAsync (string username, string password, ref System.Net.CookieContainer cookies)
		{
			Console.WriteLine ("Login LinkedIn");
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create ("https://www.livecoding.tv/accounts/linkedin/login");
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
				data = HtmlHelper.getElement (data, "<form action=\"/uas/oauth/authorize/submit\"");
			}


			LoginLinkedIn (username, password,data, ref cookies);
		}
		private void LoginLinkedIn(string username,string password,string data,ref CookieContainer cookies)
		{
			//POST login data
			Dictionary<string,string> postData = new Dictionary<string, string> ();
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create ("https://www.linkedin.com/uas/oauth/authorize/submit");
			request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
			request.CookieContainer = cookies;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			postData.Add ("isJsEnabled", "false");
			postData.Add ("session_key", username);
			postData.Add ("session_password", password);
			postData.Add ("agree", "true");
			postData.Add ("oauth_token", HtmlHelper.getAttribute(HtmlHelper.getSingleElement(data,"<input type=\"hidden\" name=\"oauth_token\""),"value"));
			postData.Add ("csrfToken", HtmlHelper.getAttribute(HtmlHelper.getSingleElement(data,"<input type=\"hidden\" name=\"csrfToken\""),"value"));
			postData.Add ("sourceAlias", HtmlHelper.getAttribute(HtmlHelper.getSingleElement(data,"<input type=\"hidden\" name=\"sourceAlias\""),"value"));
			//duration=0&authorize=Zugriff+erlauben&extra=&access=-3&agree=true&oauth_token=77--758ae117-ba0f-472e-b961-c05be8d12cc9&email=&appId=&csrfToken=ajax%3A3324717073940490783&sourceAlias=0_8L1usXMS_e_-SfuxXa1idxJ207ESR8hAXKfus4aDeAk&client_ts=1441384096141&client_r=%3A700951532%3A664704238%3A223863907&client_output=0&client_n=700951532%3A664704238%3A223863907&client_v=1.0.1
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

