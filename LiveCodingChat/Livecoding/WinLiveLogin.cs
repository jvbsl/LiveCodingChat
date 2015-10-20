using System;
using System.Net;
using System.Collections.Generic;

namespace LiveCodingChat
{
    public class WinLiveLogin : ILoginMethod
    {
        public WinLiveLogin()
        {
        }
        #region ILoginMethod implementation

        public event LoginCompleted LoginCompleted;

        public void LoginAsync(string username, string password, ref CookieContainer cookies)
        {
            Console.WriteLine("Login WinLive");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://www.livecoding.tv/accounts/windowslive/login/?process=log");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240";
            request.CookieContainer = cookies;
			request.AllowAutoRedirect = true;
			request.Headers ["Upgrade-Insecure-Requests"] = "1";
			request.Referer = "https://www.livecoding.tv/accounts/login/";
            request.BeginGetResponse(EndGetResponse, new object[] { request, username, password, cookies });

        }
        #endregion

        private void EndGetResponse(IAsyncResult res)
        {
            object[] obj = (object[])res.AsyncState;
            string username = (string)obj[1], password = (string)obj[2];
            CookieContainer cookies = (CookieContainer)obj[3];

            HttpWebRequest request = (HttpWebRequest)obj[0];
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(res);
			if (response.StatusCode == HttpStatusCode.Found) {
				
				request = (HttpWebRequest)HttpWebRequest.Create(response.Headers["Location"]);
				request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240";
				request.CookieContainer = cookies;
				request.AllowAutoRedirect = false;
				request.Referer = response.ResponseUri.ToString();
				request.Headers ["Upgrade-Insecure-Requests"] = "1";
				request.BeginGetResponse(EndGetResponse, new object[] { request, username, password, cookies });
				return;
			}
            string data;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                data = sr.ReadToEnd();
            }


            LoginWinLive(username, password, data, ref cookies);
        }

        private void LoginWinLive(string username, string password,  string data, ref CookieContainer cookies)
        {

            string urlPost, PPSX, Referer;
            int starti, endi;
            starti = data.IndexOf("urlPost:'");
            if (starti < 0) throw new NotImplementedException("urlPost not found");
            starti += "urlPost:'".Length;
            endi = data.IndexOf("'", starti);
            if (endi < 0) throw new NotImplementedException("urlPost not found");
            urlPost = data.Substring(starti, endi - starti);

            starti = data.IndexOf("O:'");
            if (starti < 0) throw new NotImplementedException("PPSX not found");
            starti += "O:'".Length;
            endi = data.IndexOf("'", starti);
            if (endi < 0) throw new NotImplementedException("PPSX not found");
            PPSX = data.Substring(starti, endi - starti);

            starti = data.IndexOf("AV:'");
            if (starti < 0) throw new NotImplementedException("AV:' not found");
            starti += "AV:'".Length;
            endi = data.IndexOf("&mkt", starti);
            if (endi < 0) throw new NotImplementedException("AV:' not found");
            Referer = data.Substring(starti, endi - starti);


            //POST login data

            Dictionary<string, string> postData = new Dictionary<string, string>();
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(urlPost);
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.85 Safari/537.36";
            request.CookieContainer = cookies;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = Referer;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

            postData.Add("loginfmt", username);
            postData.Add("passwd", password);
			postData.Add ("KMSI", "1");
            postData.Add("SI", "Anmelden");
            postData.Add("login", username.ToLower());
            postData.Add("type", "11");
            postData.Add("PPFT",HtmlHelper.getAttribute( HtmlHelper.getSingleElement(data,"<input type=\"hidden\" name=\"PPFT\""),"value"));
            postData.Add("PPSX", PPSX);
            postData.Add("idsbho", "1");
            postData.Add("sso", "0");
            postData.Add("NewUser", "1");
            postData.Add("LoginOptions", "1");
            postData.Add("i1", "0");
            postData.Add("i2", "1");
            postData.Add("i3", "1000"); //LoginTime
            postData.Add("i4", "0");
            postData.Add("i7", "0");
            postData.Add("i12", "1");
            postData.Add("i13", "0");
            postData.Add("i14", "64"); // RenderTime
            postData.Add("i17", "0");
            postData.Add("i18", "__Login_Strings|1,__Login_Core|1,");
            byte[] postBuild = HttpHelper.CreatePostData(postData);
            request.ContentLength = postBuild.Length;
            request.GetRequestStream().Write(postBuild,0, postBuild.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                data = sr.ReadToEnd();
            }

            if (LoginCompleted != null)
				LoginCompleted(this, new LoginEventArgs(data,cookies));
        }
    }
}
