using System;
using System.Net;
using System.Collections.Generic;

namespace LiveCodingChat
{
	public class TwitchLogin:ILoginMethod
	{
		public TwitchLogin ()
		{
		}
		#region ILoginMethod implementation

		public event LoginCompleted LoginCompleted;

		public void LoginAsync(string username, string password,ref CookieContainer cookies)
		{
			Console.WriteLine ("Login Twitch");
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create ("https://www.livecoding.tv/accounts/twitch/login");
			request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
			request.CookieContainer = cookies;
			request.BeginGetResponse (EndGetResponse,new object[]{request,username,password,cookies});

		}
		#endregion
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
				data = HtmlHelper.getElement (data, "<form accept-charset=\"UTF-8\" action=\"/kraken/oauth2/login\"");
			}


			LoginTwitch (username, password,data, ref cookies);
		}
		private void LoginTwitch(string username,string password,string data,ref CookieContainer cookies)
		{
			//POST login data
			Dictionary<string,string> postData = new Dictionary<string, string> ();
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create ("https://api.twitch.tv/kraken/oauth2/login");
			request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
			request.CookieContainer = cookies;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			postData.Add ("utf8", "\u2713");
			postData.Add ("authenticity_token", HtmlHelper.getAttribute(HtmlHelper.getSingleElement(data,"<input name=\"authenticity_token\""),"value"));
			postData.Add ("login_type", "login");
			postData.Add ("client_id", HtmlHelper.getAttribute(HtmlHelper.getSingleElement(data,"<input name='client_id'"),"value"));
			postData.Add ("redirect_uri", "https://www.livecoding.tv/accounts/twitch/login/callback/");
			postData.Add ("response_type", "code");
			postData.Add ("scope", "user_read");
			postData.Add ("state", HtmlHelper.getAttribute(HtmlHelper.getSingleElement(data,"<input name='state'"),"value"));
			postData.Add ("login", username);
			postData.Add ("password", password);
			postData.Add ("date[month]", "");
			postData.Add ("date[day]", "");
			postData.Add ("date[year]", "");
			postData.Add ("email", "");
			postData.Add ("g-recaptcha-response", "");
			byte[] postBuild = HttpHelper.CreatePostData (postData);
			request.ContentLength = postBuild.Length;
			request.GetRequestStream ().Write (postBuild, 0, postBuild.Length);

			HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
			using (System.IO.StreamReader sr = new System.IO.StreamReader (response.GetResponseStream ())) {
				data = sr.ReadToEnd();
			}

			if (LoginCompleted != null)
				LoginCompleted (this, new LoginEventArgs(data,cookies));
		}
	}
}

