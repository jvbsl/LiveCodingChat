using LiveCodingChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AdamBot.Commands
{
    class Strawpoll
    {
        private bool canCreate=false;
        public Strawpoll(string[] arguments)
        {
            if (arguments.Length <5)
                return;
            try{
                this.Arguments = arguments;
                this.Private = true;
                this.Question = arguments[0];
                this.MultipleChoice = bool.Parse(arguments[1]);
                this.AllowComments = bool.Parse(arguments[2]);
                this.Answers = new string[Arguments.Length-3];
                for (int i = 0; i < Answers.Length; i++)
                    this.Answers[i] = arguments[i + 3];
                canCreate = true;
            }catch{
            }
        }
        
        public string[] Arguments { get; private set; }
        public string Question { get; private set; }
        public bool Private { get; private set; }
        public bool MultipleChoice { get; private set; }
        public bool AllowComments { get; private set; }
        public string[] Answers { get; private set; }
        public string CreatePoll()
        {
            if (!canCreate)
                return null;
            try
            {
                Dictionary<string, string> postData = new Dictionary<string, string>();
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://strawpoll.de/new?lang=de");
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                postData.Add("question", Question);
                for (int i = 0; i < Answers.Length; i++)
                    postData.Add("a" + i.ToString(), Answers[i]);
                postData.Add("ma", MultipleChoice.ToString().ToLower());
                postData.Add("co", AllowComments.ToString().ToLower());
                postData.Add("lang", "de");
                byte[] postBuild = HttpHelper.CreatePostData(postData);
                request.ContentLength = postBuild.Length;
                request.GetRequestStream().Write(postBuild, 0, postBuild.Length);

                string data;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    data = sr.ReadToEnd();
                }
                return HtmlHelper.getAttribute(HtmlHelper.getElement(HtmlHelper.getElement(data, "<div class=\"sharelinkbox\">"), "<a href="), "href");
            }
            catch { }
            return null;
        }
    }
}
