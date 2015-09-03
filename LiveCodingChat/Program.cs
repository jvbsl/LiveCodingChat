using System;
using System.Net;

namespace LiveCodingChat
{
	class MainClass
	{
		private static Livecoding.LivecodingSession session;
		private static bool run=false;
		private static System.Media.SoundPlayer player;
		public static void Main(string[] args)
		{
			string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly ().Location);
			try{
				player = new System.Media.SoundPlayer (System.IO.Path.Combine (dir, "sound.wav"));
				player.Load();
			}
			catch(Exception ex){player = null;}
			run = true;
			string username;
			Console.Write ("Username:");
			username = Console.ReadLine ();
			session = new LiveCodingChat.Livecoding.LivecodingSession (new TwitchLogin(),username);
			session.PasswordRequested += Session_PasswordRequested;
			session.SessionAutenticated += Session_SessionAutenticated;
			session.EnsureAuthenticated ();
			while (run) {//TODO: wird eine gui anwendung :P
				System.Threading.Thread.Sleep (10);
			}
		}

		static void Session_SessionAutenticated (object sender, EventArgs e)
		{
			Console.WriteLine ("Authenticated");
			session.BeginOpenChat ("bobstriker", new AsyncCallback (EndOpenChat), null);

		}
		private static void EndOpenChat(IAsyncResult res)
		{
			try{
			Livecoding.ChatRoom room = session.EndOpenChat (res);
			room.Client.MessageReceived += Test_MessageReceived;
			while (true) {
				string ln=Console.ReadLine ();
				if (ln == "exit") {
					run=false;
					break;
				}
				room.Room.SendMessage (ln);
			}
			}catch(Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}

		static void Session_PasswordRequested (object sender, ref string Password)
		{
			Console.Write ("Password:");
			Password = readPassword ();
		}
		private static string readPassword()
		{
			string pass="";
			ConsoleKeyInfo key;
			do
			{
				key = Console.ReadKey(true);

				// Backspace Should Not Work
				if (key.Key != ConsoleKey.Backspace)
				{

					if (key.Key == ConsoleKey.Enter)
						Console.WriteLine("");
					else{
						pass += key.KeyChar;
						Console.Write("*");
					}
				}
				else if (pass.Length > 0)
				{
					Console.Write("\b");
					pass = pass.Substring(0,pass.Length-1);
				}
			}
			// Stops Receving Keys Once Enter is Pressed
			while (key.Key != ConsoleKey.Enter);
			return pass;
		}
		static void Test_MessageReceived (LiveCodingChat.Xmpp.Room room, string nick, string message)
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine (nick + ": " + message);
			Console.ForegroundColor = ConsoleColor.White;
			string fnd = message.ToLower ();
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex ("N[E]*I[I]*N TO[O]*M NE[E]*I[I]*N");
			if (fnd.Contains("@tom") || fnd.Contains("@bobstriker")||r.IsMatch(fnd.ToUpper()))
			{

				if (player == null)
					System.Media.SystemSounds.Exclamation.Play ();
				else
					player.Play ();
			}
		}
	}
}
