using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using LiveCodingChat.Xmpp;

namespace NeinTom
{
	public class ChatLog : UserControl
	{
		CircularBuffer<MessageReceivedEventArgs> messages;
		CircularBuffer<MessageInfo> messageInfos;
		private bool needsResize;
		Font font;
		Font userNameFont;
		public ChatLog ()
		{
			SetStyle (ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

			messages = new CircularBuffer<LiveCodingChat.Xmpp.MessageReceivedEventArgs> (50);
			font = new Font ("Arial",12.0f);
			userNameFont = new Font (font, FontStyle.Bold);
			needsResize = true;
			this.Paint += ChatLog_Paint;
		}
		public bool ShowTimeStamp{ get; set; }
		public string TimeStampFormat{ get; set; }
		private void MeasureTexts(Graphics g)
		{
			messageInfos.Clear ();
			float currentPosition = ClientSize.Height;
			foreach(MessageReceivedEventArgs msg in messages){
				float leftMargin = 0.0f, height = 0.0f;
				if (ShowTimeStamp) {
					string timestamp = "["+msg.TimeStamp.ToString (TimeStampFormat)+"]";
					SizeF s = g.MeasureString (timestamp, userNameFont);
					leftMargin += s.Width;
					height = Math.Max (height, s.Height);
				}
				SizeF userNameSize = g.MeasureString (msg.Nick, userNameFont);
				leftMargin += userNameSize.Width;
				height = Math.Max (height, userNameSize.Height);
				SizeF msgSize = MeasureMessage (msg.Message, g,leftMargin);
				height = Math.Max (height, msgSize.Height);
				currentPosition -= height;
				messageInfos.Add (new MessageInfo (currentPosition));
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
		private void ChatLog_Paint (object sender, PaintEventArgs e)
		{
			if (needsResize)
				MeasureTexts (e.Graphics);
			foreach (MessageReceivedEventArgs m in messages) {
				
			}
		}
	}
}

