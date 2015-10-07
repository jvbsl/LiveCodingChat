using System;
using System.Net;
using System.Collections.Generic;
using LiveCodingChat.Xmpp;
using LiveCodingChat.Livecoding;
using LiveCodingChat;
using AdamBot.Commands;

namespace AdamBot
{
    class MainClass
    {
        private static LivecodingSession session;
        private static bool run = false;
        private static System.Media.SoundPlayer player;
        private static int userCount = 0;
        private static List<string> welcomeUser = new List<string>();
        private static System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        private static int timeToSayHelloAgainInMinutes = 30;
        private static bool isStarted = false;
        private static Random rnd = new Random();
        public static void Main(string[] args)
        {
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            try
            {
                player = new System.Media.SoundPlayer(System.IO.Path.Combine(dir, "sound.wav"));
                player.Load();
            }
            catch (Exception ex) { player = null; }
            run = true;
            string username;
            Console.Write("Username:");
            username = Console.ReadLine();



            session = new LiveCodingChat.Livecoding.LivecodingSession(ReadLoginMethod(), username);
            session.PasswordRequested += Session_PasswordRequested;
            session.SessionAutenticated += Session_SessionAutenticated;
            session.EnsureAuthenticated();
            while (run)
            {
                System.Threading.Thread.Sleep(10);
            }
        }
        static ILoginMethod ReadLoginMethod()
        {
            List<Type> types = new List<Type>();
            int i = 0;
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(typeof(LivecodingSession));
            foreach (Type t in asm.GetTypes())
            {
                if (typeof(ILoginMethod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                {
                    types.Add(t);
                    Console.WriteLine("[" + i.ToString() + "] " + t.Name);
                    i++;
                }
            }
            int parsed = 0;
            do
            {
                Console.Write("Use Login Method:");
            } while (!int.TryParse(Console.ReadLine(), out parsed) || parsed >= types.Count || parsed < 0);
            return (ILoginMethod)types[parsed].GetConstructor(new Type[] { }).Invoke(new object[] { });
        }

        static void Session_SessionAutenticated(object sender, EventArgs e)
        {
            Console.WriteLine("Authenticated");
            Console.Write("Room name:");
            string room = Console.ReadLine();
            session.BeginOpenChat(room, new AsyncCallback(EndOpenChat), null);

        }
        static ChatRoom chatRoom=null;
        private static void EndOpenChat(IAsyncResult res)
        {
            try
            {
                chatRoom = session.EndOpenChat(res);
                chatRoom.Client.MessageReceived += Room_Client_MessageReceived;
                chatRoom.Room.UserStateChanged += Room_UserStateChanged;
                Console.WriteLine("Write '" + "start bot" + "' in the Console to start the bot");
                while (true)
                {
                    string ln = Console.ReadLine();
                    if (ln == "exit")
                    {
                        run = false;
                        break;
                    }
                    else if(ln == "start bot")
                    {
                        isStarted = true;
                        stopWatch.Start();
                    }
                    chatRoom.Room.SendMessage(ln);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Room_UserStateChanged(Room room, User user, LiveCodingChat.Xmpp.UserState state)
        {
            if (state == LiveCodingChat.Xmpp.UserState.Available)
            {
                userCount++;
            }
            else
            {
                userCount--;
            }
            if (!isStarted)
                return;
            if (user.ID == chatRoom.Client.Nick)
                return;
            if (state == LiveCodingChat.Xmpp.UserState.Available)
            {
                System.Timers.Timer tmr = new System.Timers.Timer();
                tmr.Interval = 20000;
                tmr.Elapsed +=delegate {
                    if (welcomeUser.Count != 0)
                        for (int i = 0; i < welcomeUser.Count; i++)
                            if (Convert.ToInt32(welcomeUser[i].Remove(0, welcomeUser[i].Length - 3)) <= stopWatch.Elapsed.Minutes - timeToSayHelloAgainInMinutes)
                                welcomeUser.Remove(welcomeUser[i]);
                    
                    if (!welcomeUser.Exists(t => t.Substring(0, t.Length - 3) == user.ID.ToLower()))
                    {
                        room.SendMessage("Willkommen @" + user.ID + ".Ich bin Adam der Bot dieses Streams. Sprich mich an, wenn du Infos zum Stream brauchst");
                        welcomeUser.Add(user.ID.ToLower() + stopWatch.Elapsed.Minutes.ToString("000"));
                    }
                    
                    if (userCount % 10 == 0)
                    {
                        room.SendMessage("@" + user.ID + " ist der " + userCount + " besucher dieses Streams :hi:");
                    }
                    tmr.Stop();
                };
                tmr.Start();
            }

        }

        static void Room_Client_MessageReceived(LiveCodingChat.Xmpp.Room room, LiveCodingChat.Xmpp.MessageReceivedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(e.Nick + ": " + e.Message);
            Console.ForegroundColor = ConsoleColor.White;
            
            if (!isStarted)
                return;
            if (e.User == null)
                return;
            string fnd = e.Message.ToLower();
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("NE[E]*I[I]*N TO[O]*M NE[E]*I[I]*N");
            if (fnd.Contains("@tom") || fnd.Contains("@bobstriker") || r.IsMatch(fnd.ToUpper()))
            {
                if (player == null)
                    System.Media.SystemSounds.Exclamation.Play();
                else
                    player.Play();
            }

            if (e.User.ID == chatRoom.Client.Nick || e.User.ID == "octobot")//TODO email->nick/username
                return;
            if (fnd.Contains(chatRoom.Client.Nick) || fnd.Contains("adam"))
            {
                room.SendMessage("@" + e.Nick + ": Hier wird OctoAwesome entwickelt. Mehr Infos unter http://www.octoawesome.net");
            }
            if (e.Nick == "jvbsl" || e.User.Staff || e.User.Role == "moderator")
            {
                if (fnd.StartsWith("/strawpoll "))
                {
                    string[] args = e.Message.Substring("/strawpoll ".Length).Split(',');
                    Strawpoll poll = new Strawpoll(args);
                    string pollRes = poll.CreatePoll();
                    if (pollRes != null)
                        room.SendMessage("Neuer Poll - " + args[0] + ": " + pollRes);
                }
            }
            if (fnd.Contains("kopfoderzahl?"))
            {
                byte b = Convert.ToByte(rnd.Next(0, 2));
                switch (b)
                {
                    case 0:
                        room.SendMessage("@" + e.Nick + " Kopf");
                        break;
                    case 1:
                        room.SendMessage("@" + e.Nick + " Zahl");
                        break;
                }
            }
        }

        static void Session_PasswordRequested(object sender, ref string Password)
        {
            Console.Write("Password:");
            Password = readPassword();
        }
        private static string readPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace)
                {

                    if (key.Key == ConsoleKey.Enter)
                        Console.WriteLine("");
                    else
                    {
                        pass += key.KeyChar;
                        Console.Write("*");
                    }
                }
                else if (pass.Length > 0)
                {
                    Console.Write("\b");
                    pass = pass.Substring(0, pass.Length - 1);
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            return pass;
        }
    }
}
