using System;

namespace LiveCodingChat
{
	public class GooglePlusLogin:ILoginMethod
	{
		public GooglePlusLogin ()
		{
		}

		#region ILoginMethod implementation

		public event LoginCompleted LoginCompleted;

		public void LoginAsync (string username, string password, ref System.Net.CookieContainer cookies)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

