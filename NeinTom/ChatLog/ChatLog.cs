using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using LiveCodingChat.Xmpp;

namespace NeinTom
{
	public class ChatLog : UserControl
	{
		CircularBuffer<ChatMessage> messages;
		private bool needsResize;
		Font font;
		Font userNameFont;
		public ChatLog ()
		{
			SetStyle (ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

			messages = new CircularBuffer<ChatMessage> (50);
			font = new Font (new FontFamily("Arial"),12.0f);
			userNameFont = new Font (font, FontStyle.Bold);
			needsResize = true;
			this.Paint += ChatLog_Paint;
		}
		public bool ShowTimeStamp{ get; set; }
		public string TimeStampFormat{ get; set; }
		private void MeasureTexts(Graphics g)
		{
			float currentPosition = ClientSize.Height;
			foreach(ChatMessage msg in messages){
				msg.Parse (g,new SizeF(ClientSize.Width,currentPosition));
				currentPosition -= msg.Size.Height;
			}
		}
		private SizeF MeasureMessage(string message,Graphics g,float leftMargin)
		{
			StringFormat strFormat = StringFormat.GenericTypographic;
			strFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
			strFormat.Trimming = StringTrimming.Word;
			return g.MeasureString (message,font, new SizeF (ClientSize.Width - leftMargin, float.MaxValue), strFormat);
			//g.MeasureString(message,);
		}
		public void AddMessage(ChatMessage message)
		{
			messages.Add (message);
		}
		private void ChatLog_Paint (object sender, PaintEventArgs e)
		{
			if (needsResize)
				MeasureTexts (e.Graphics);
			foreach (ChatMessage m in messages) {
				m.Draw (e.Graphics, m.Position, m.Size);
			}
		}
	}
}

