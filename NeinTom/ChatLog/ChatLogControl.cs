using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using LiveCodingChat.Xmpp;

namespace NeinTom
{
    public class ChatLogControl : UserControl
    {
        CircularBuffer<ChatMessage> messages;
        private bool needsResize;
        Font userNameFont;
        private float logHeight, scrollOffset;
        private Timer tmrUpdate;
        private System.ComponentModel.IContainer components;

        public ChatLogControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);

            InitializeComponent();

            messages = new CircularBuffer<ChatMessage>(100);
            messages.ItemThrown += Messages_ItemThrown;
            Font = new Font(new FontFamily("Arial"), 12.0f);
            userNameFont = new Font(Font, FontStyle.Bold);
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
            this.AutoScrollMinSize = new Size(ClientSize.Width, (int)logHeight);
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
            /*int max = Math.Max((int)logHeight - this.ClientSize.Height, 0);
            if (this.VerticalScroll.Value > max)
                this.VerticalScroll.Value = max;
            this.VerticalScroll.Maximum = max;
            this.VerticalScroll.Visible = this.VerticalScroll.Maximum > 0;*/
            this.AutoScrollMinSize = new Size(ClientSize.Width, (int)logHeight);
            needsResize = true;
            this.Invalidate();
        }
        private void ChatLog_Paint(object sender, PaintEventArgs e)
        {
            if (needsResize)
            {
                MeasureTexts(e.Graphics);
                this.AutoScrollMinSize = new Size(ClientSize.Width, (int)logHeight);
            }
            int scrollOffset = this.AutoScrollPosition.Y;//this.VerticalScroll.Value - this.VerticalScroll.Minimum;
            foreach (ChatMessage m in messages)
            {
                m.Draw(e.Graphics, new PointF(m.Position.X, m.Position.Y + ((int)logHeight - this.ClientSize.Height) + scrollOffset), m.Size);
            }
        }
        private void ChatLog_Resize(object sender, EventArgs e)
        {
            MeasureTexts(this.CreateGraphics());//pöse pöse creategraphics :D
            this.Invalidate();
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Enabled = true;
            this.tmrUpdate.Interval = 40;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // ChatLogControl
            // 
            this.AutoScroll = true;
            this.Name = "ChatLogControl";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ChatLog_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChatLogControl_MouseDown);
            this.Resize += new System.EventHandler(this.ChatLog_Resize);
            this.ResumeLayout(false);

        }

        private void ChatLogControl_Scroll(object sender, ScrollEventArgs e)
        {

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                //VerticalScroll.Value = e.NewValue;
                this.Invalidate();
            }

        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
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

