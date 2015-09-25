using System;
using System.Drawing;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using LiveCodingChat;

namespace NeinTom
{
	public class ChatMessage
	{
		private User user;
		private ChatMessagePart root;
		public ChatMessage (User user,string xml)
		{
			this.user = user;
			XmlDocument doc = new XmlDocument();
			doc.LoadXml (xml);
			XmlElement root = (XmlElement)doc.FirstChild;
			this.root = new ContainerPart ((XmlElement)root.FirstChild);
		}
		public string MessageText{ get; private set;}
		public string TimeStamp{get;private set;}
		public string Nick {get;private set;}
		public SizeF Size{
			get 
			{ return root.Size;
			}
		}
		public PointF Position{get;private set;}

		public void Parse(Graphics g,SizeF Size)
		{
			root.Parse (g,Size);
			Position = new PointF (0, Size.Height - this.Size.Height);
		}

		public void Draw(Graphics g,PointF position,SizeF size)
		{
			root.Draw (g, position, size);
		}
	}
}

