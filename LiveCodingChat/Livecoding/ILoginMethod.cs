using System;
using System.Net;

namespace LiveCodingChat
{
	public delegate void LoginCompleted(object sender,CookieContainer cookies);
	public interface ILoginMethod
	{
		event LoginCompleted LoginCompleted;
		void LoginAsync (string username, string password,ref CookieContainer cookies);
	}
}

