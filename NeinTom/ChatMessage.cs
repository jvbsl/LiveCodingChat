using System;
using System.Drawing;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace NeinTom
{
	public class ChatMessage
	{
		public ChatMessage (string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml (xml);
			new ChatMessagePart ((XmlElement)doc.FirstChild);
		}
		public string MessageText{ get; private set;}
		public string TimeStamp{get;private set;}
		public string Nick {get;private set;}
		public SizeF Size{get;private set;}
		public PointF Position{get;private set;}
	}
	internal class ChatMessagePart
	{
		private List<ChatMessagePart> parts;
		public ChatMessagePart(XmlElement xml)
		{
			parts = null;
			ParseXml (xml);
		}
		private void ParseXml(XmlElement element)
		{
            return;
			Text = element.InnerText;
			if (element.HasChildNodes) {//TODO: remove child nodes...everyone gets a part
				parts = new List<ChatMessagePart> ();
				foreach (XmlElement child in element.ChildNodes) {
					parts.Add(new ChatMessagePart(child));
				}
			}

		}
		public string XML{ get; private set; }
		public string Text{get;private set;}
		public SizeF Size{get;private set;}
		public PointF Position{get;private set;}
		public void Parse(Graphics g)
		{
			
		}
		
	}
}

