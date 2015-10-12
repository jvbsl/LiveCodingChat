using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using LiveCodingChat.Xmpp;

namespace NeinTom.ChatLog
{
    public partial class ChatLogControl : UserControl
    {
        CircularBuffer<ChatMessage> messages;
        private bool needsResize;
        Font userNameFont;
        private float logHeight, scrollOffset;
        
        private Size oldClientSize;
		private ChatMessage previousChatMessage;
        public ChatLogControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);

            InitializeComponent();

            messages = new CircularBuffer<ChatMessage>(100);
            messages.ItemThrown += Messages_ItemThrown;
            Font = new Font(new FontFamily("Arial"), 12.0f);
            userNameFont = new Font(Font, FontStyle.Bold);
            oldClientSize = this.ClientSize;
            needsResize = true;
        }

        private void Messages_ItemThrown(object sender, ChatMessage thrown)
        {
            logHeight -= thrown.Size.Height;
        }

        public bool ShowTimeStamp { get; set; }
        public string TimeStampFormat { get; set; }
        private void MeasureTexts(Graphics g)
        {
            float currentPosition = ClientSize.Height;
            logHeight = 0;
            foreach (ChatMessage msg in messages)
            {
                msg.Parse(g, new SizeF(ClientSize.Width, currentPosition));
                logHeight += msg.Size.Height;
                currentPosition -= msg.Size.Height;
            }
            /*int max = Math.Max((int)logHeight - this.ClientSize.Height, 0);
            if (this.VerticalScroll.Value > max)
                this.VerticalScroll.Value = max;
            this.VerticalScroll.Maximum = max;
            this.VerticalScroll.Visible = this.VerticalScroll.Maximum > 0;*/
            this.AutoScrollMinSize = new Size(0, (int)logHeight);

            needsResize = false;
        }
        private SizeF MeasureMessage(string message, Graphics g, float leftMargin)
        {
            StringFormat strFormat = StringFormat.GenericTypographic;
            strFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
            strFormat.Trimming = StringTrimming.Word;
            return g.MeasureString(message, Font, new SizeF(ClientSize.Width - leftMargin, float.MaxValue), strFormat);
            //g.MeasureString(message,);
        }
        public void AddMessage(ChatMessage message)
        {
            messages.Add(message);
            logHeight += message.Size.Height;

            needsResize = true;
            bool scroll = (((this.ClientSize.Height - (int)logHeight + this.Margin.Bottom)- this.AutoScrollPosition.Y) <= this.Margin.Bottom);
			if (scroll)
            {//729,767,3
                MeasureTexts(CreateGraphics());//TODO: böse
                ScrollToBottom();
            }
            else
                this.AutoScrollMinSize = new Size(0, (int)logHeight);
           
            this.Invalidate();
        }
        public void ScrollToBottom()
        {
            Size sz = this.ClientSize - this.AutoScrollMinSize + this.AutoScrollMargin;
            this.AutoScrollPosition = new Point(this.AutoScrollPosition.X, -sz.Height) ;
            //this.AutoScrollPosition = new Point(this.AutoScrollPosition.X, (this.ClientSize.Height - (int)logHeight+this.Margin.Bottom));
            
        }
        private void ChatLog_Paint(object sender, PaintEventArgs e)
        {
            if (needsResize)
            {
                MeasureTexts(e.Graphics);
            }
            int scrollOffset = this.AutoScrollPosition.Y;//this.VerticalScroll.Value - this.VerticalScroll.Minimum;
            foreach (ChatMessage m in messages)
            {
                m.Draw(e.Graphics, new PointF(m.Position.X, m.Position.Y + ((int)logHeight - this.ClientSize.Height) + scrollOffset), m.Size);
            }
        }

        private void ChatLog_Resize(object sender, EventArgs e)
        {
            bool scroll = ((oldClientSize.Height - (int)logHeight) == this.AutoScrollPosition.Y);
            MeasureTexts(this.CreateGraphics());//pöse pöse creategraphics :D
            if (scroll)
                ScrollToBottom();
            oldClientSize = this.ClientSize;
            this.Invalidate();
        }


        private void ChatLogControl_Scroll(object sender, ScrollEventArgs e)
        {

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                this.Invalidate();
            }

        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void ChatLogControl_MouseMove(object sender, MouseEventArgs e)
        {
            float currentPosition = ClientSize.Height + ((int)logHeight - this.ClientSize.Height) + AutoScrollPosition.Y;
            foreach (ChatMessage msg in messages)
            {
                currentPosition -= msg.Size.Height;
				if (e.Y > currentPosition && e.Y < currentPosition + msg.Size.Height)
				{
					if (msg != previousChatMessage) {
						if (previousChatMessage != null)
							previousChatMessage.MouseLeave (new PointF (e.X, e.Y - currentPosition), e);
						msg.MouseEnter (new PointF (e.X, e.Y - currentPosition),e);
						previousChatMessage = msg;
					}
					

                    msg.MouseMove(new PointF(e.X, e.Y - currentPosition), e);
					break;
				}

            }
        }

        private void ChatLogControl_MouseEnter(object sender, EventArgs eb)
        {
            Point location = this.PointToClient(System.Windows.Forms.Cursor.Position);
            MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, location.X, location.Y, 0);
            float currentPosition = ClientSize.Height + ((int)logHeight - this.ClientSize.Height) + AutoScrollPosition.Y;
            foreach (ChatMessage msg in messages)
            {
                currentPosition -= msg.Size.Height;
                if (e.Y > currentPosition && e.Y < currentPosition + msg.Size.Height)
                    msg.MouseEnter(new PointF(e.X, e.Y - currentPosition), e);

            }
        }

        private void ChatLogControl_MouseLeave(object sender, EventArgs eb)
        {
            Point location = this.PointToClient(System.Windows.Forms.Cursor.Position);
            MouseEventArgs e = new MouseEventArgs(MouseButtons.None,0,location.X,location.Y,0);
            float currentPosition = ClientSize.Height + ((int)logHeight - this.ClientSize.Height) + AutoScrollPosition.Y;
            foreach (ChatMessage msg in messages)
            {
                currentPosition -= msg.Size.Height;
                msg.MouseLeave(new PointF(e.X, e.Y - currentPosition), e);

            }
        }

        private void ChatLogControl_MouseDown(object sender, MouseEventArgs e)
        {
            float currentPosition = ClientSize.Height  +((int)logHeight - this.ClientSize.Height) + AutoScrollPosition.Y;
            foreach (ChatMessage msg in messages)
            {
                currentPosition -= msg.Size.Height;
                if (e.Y > currentPosition && e.Y < currentPosition + msg.Size.Height)
                    msg.MouseDown(new PointF(e.X, e.Y - currentPosition), e);

            }
        }
    }
}

