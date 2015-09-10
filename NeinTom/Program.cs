using System;
using System.Windows.Forms;
namespace NeinTom
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ChatMessage msg = new ChatMessage ("<message>bla bla <b>fett<i>kursiv</i></b> bla bla</message>");
			Application.EnableVisualStyles ();
			Application.Run (new frmMain ());
		}
	}
}

