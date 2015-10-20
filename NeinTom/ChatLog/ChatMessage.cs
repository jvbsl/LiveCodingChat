using System;
using System.Drawing;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using LiveCodingChat;
using System.Windows.Forms;

namespace NeinTom.ChatLog
{
    public class ChatMessage
    {
        private User user;
        private ChatMessagePart root;
        private ChatLogControl parent;
		private StringFormat stf;
        public ChatMessage(ChatLogControl parent, User user, string xml)
        {
            this.parent = parent;
            this.user = user;
			stf = StringFormat.GenericTypographic;
			stf.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
			stf.FormatFlags |= StringFormatFlags.LineLimit;
			stf.LineAlignment = StringAlignment.Center;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<body>" + xml + "</body>");
            XmlElement root = (XmlElement)doc.FirstChild;
			this.root = new RootPart(this,(XmlElement)root, new Font("Arial", 16));
            if (user != null)
                Nick = user.ID;
        }
		public Control getParentControl()
		{
			return parent;
		}
        public string MessageText { get; private set; }
        public string TimeStamp { get; set; }
        public string Nick { get; set; }
        public SizeF Size
        {
            get
            {
                return new SizeF(root.Size.Width, root.Size.Height + 3);
            }
        }
        public PointF Position { get; private set; }
        private float offset = 0.0f;
        public void Parse(Graphics g, SizeF Size)
        {
			SizeF size = g.MeasureString(TimeStamp + Nick + ":", parent.Font,Size,stf);
            offset = size.Width;
            root.Parse(g, new SizeF(Size.Width - offset, Size.Height));

            Position = new PointF(0, Size.Height - this.Size.Height);
        }

        public void Draw(Graphics g, PointF position, SizeF size)
        {
            if (user == null)
				g.DrawString(TimeStamp + Nick + ":", parent.Font, new SolidBrush(Color.Black), new RectangleF(position,new SizeF(offset,Size.Height)),stf);
            else
				g.DrawString(TimeStamp + Nick + ":", parent.Font, new SolidBrush(user.Color), new RectangleF(position,new SizeF(offset,Size.Height)),stf);
            root.Draw(g, new PointF(position.X + offset, position.Y), size);
        }
        public void MouseDown(PointF location, MouseEventArgs e)
        {
            if (location.X > offset)
                root.MouseDown(new PointF(location.X - offset, location.Y), e);
        }
        public void MouseMove(PointF location, MouseEventArgs e)
        {
			if (location.X > offset)
				root.MouseMove (new PointF (location.X - offset, location.Y), e);
			else
				root.MouseLeave (new PointF (location.X - offset, location.Y), e);
        }
        public void MouseEnter(PointF location, MouseEventArgs e)
        {
            if (location.X > offset)
                root.MouseEnter(new PointF(location.X - offset, location.Y), e);
        }
        public void MouseLeave(PointF location, MouseEventArgs e)
        {
            //if (location.X > offset)
                root.MouseLeave(new PointF(location.X - offset, location.Y), e);
        }
    }
}

