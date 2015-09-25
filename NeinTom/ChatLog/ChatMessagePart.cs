using System;
using System.Collections.Generic;
using System.Xml;
using System.Drawing;

namespace NeinTom
{
	internal abstract class ChatMessagePart
	{
		protected List<ChatMessagePart> parts;
		protected ChatMessagePart parent;
		public ChatMessagePart(ChatMessagePart parent = null)
		{
			this.parent = parent;
			this.parts = null;
		}
		public ChatMessagePart(XmlElement xml,ChatMessagePart parent = null)
			: this(parent)
		{
			ParseXml (xml);
		}

		private void ParseXml(XmlElement element)
		{
			PreParse (element);
			Text = element.InnerText;
			if (element.HasChildNodes) {//TODO: remove child nodes...everyone gets a part
				parts = new List<ChatMessagePart> ();
				foreach (XmlNode node in element.ChildNodes) {
					if (node.NodeType == XmlNodeType.Text) {
						parts.Add (new TextPart (node.Value,element,this));
					} else {
						//TODO: fallback?
						parts.Add(new ContainerPart((XmlElement)node,this));
					}
				}

			}

		}
		public string XML{ get; protected set; }
		public string Text{get;protected set;}
		public SizeF Size{get;protected set;}

		public void Parse(Graphics g,SizeF Size)
		{
			if (parts == null) {
				ParseInternal (g);
			} else {
				float pX=0, pY=0;
				float sX=0, sY = 0;
				float lineHeight = 0;
				foreach (ChatMessagePart part in parts) {
					part.Parse (g,Size);
					if (pX + part.Size.Width >= Size.Width && lineHeight != 0) {
						pX = 0;
						pY += lineHeight;
						lineHeight = 0;
					}
					lineHeight = Math.Max (lineHeight, part.Size.Height);
					pX += part.Size.Width;
					sX = Math.Max (sX, pX);
					sY = Math.Max (sY, pY + lineHeight);
				}
				this.Size = new SizeF (sX, sY);
			}
		}
		public void Draw(Graphics g,PointF position,SizeF size)
		{
			if (parts == null) {
				DrawInternal (g, position, size);
			} else {
				float pX=0, pY=0;
				float lineHeight = 0;
				foreach (ChatMessagePart part in parts) {
					if (pX + part.Size.Width > Size.Width && lineHeight != 0) {
						pX = 0;
						pY += lineHeight;
						lineHeight = 0;
					}
					lineHeight = Math.Max (lineHeight, part.Size.Height);
					part.Draw (g, new PointF(position.X+pX,lineHeight-part.Size.Height + position.Y+pY), Size);
					pX += part.Size.Width;
				}
			}

		}


		protected abstract void PreParse (XmlElement element);
		protected abstract void ParseInternal(Graphics g);
		protected abstract void DrawInternal (Graphics g, PointF position, SizeF size);
	}
}

