using System;
using System.Collections.Generic;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;

namespace NeinTom
{
    internal abstract class ChatMessagePart
    {
        protected List<ChatMessagePart> parts;
        protected ChatMessagePart parent;
        protected static Dictionary<string, Type> PartTypes;
        static ChatMessagePart()
        {
            PartTypes = new Dictionary<string, Type>();
            PartTypes.Add("b", typeof(TextPart));
            PartTypes.Add("i", typeof(TextPart));
            PartTypes.Add("s", typeof(TextPart));
            PartTypes.Add("u", typeof(TextPart));
            PartTypes.Add("a", typeof(LinkPart));
        }
        private static ChatMessagePart CreateInstance(Type type, XmlElement element, ChatMessagePart parent)
        {
            return (ChatMessagePart)type.GetConstructor(new Type[] { typeof(XmlElement), typeof(ChatMessagePart), typeof(Font) }).Invoke(new object[] { element, parent, null });
        }
        private static ChatMessagePart CreateInstance(Type type, string element, ChatMessagePart parent)
        {
            return (ChatMessagePart)type.GetConstructor(new Type[] { typeof(string), typeof(ChatMessagePart), typeof(Font) }).Invoke(new object[] { element, parent, null });
        }
        public ChatMessagePart(ChatMessagePart parent = null, Font font = null)
        {
            this.parent = parent;
            this.parts = null;
            if (font == null && parent != null)
                this.Font = parent.Font;
            else
                this.Font = font;
        }
        public ChatMessagePart(XmlElement xml, ChatMessagePart parent = null, Font font = null)
            : this(parent, font)
        {
            ParseXml(xml);
        }
        public Font Font { get; protected set; }
        private void ParseXml(XmlElement element)
        {
            PreParse(element);
            Text = element.InnerText;
            Type partType = null;
            if (PartTypes.ContainsKey(element.Name))
                partType = PartTypes[element.Name];
            if (element.HasChildNodes)
            {//TODO: remove child nodes...everyone gets a part
                parts = new List<ChatMessagePart>();
                foreach (XmlNode node in element.ChildNodes)
                {
                    if (partType != null)
                    {
                        if (node.NodeType == XmlNodeType.Text)
                        {
                            parts.Add(CreateInstance(partType, node.Value, this));
                        }
                        else
                        {
                            parts.Add(CreateInstance(partType, (XmlElement)node, this));
                        }
                    }
                    else if (node.NodeType == XmlNodeType.Text)
                    {
                        parts.Add(new TextPart(node.Value, this));
                    }
                    else
                    {
                        //TODO: fallback?
                        parts.Add(new ContainerPart((XmlElement)node, this));
                    }
                }

            }

        }
        public string XML { get; protected set; }
        public string Text { get; protected set; }
        public SizeF Size { get; protected set; }

        public void Parse(Graphics g, SizeF Size)
        {
            if (parts == null)
            {
                ParseInternal(g);
            }
            else
            {
                float pX = 0, pY = 0;
                float sX = 0, sY = 0;
                float lineHeight = 0;
                foreach (ChatMessagePart part in parts)
                {
                    part.Parse(g, Size);
                    if (pX + part.Size.Width >= Size.Width && lineHeight != 0)
                    {
                        pX = 0;
                        pY += lineHeight;
                        lineHeight = 0;
                    }
                    lineHeight = Math.Max(lineHeight, part.Size.Height);
                    pX += part.Size.Width;
                    sX = Math.Max(sX, pX);
                    sY = Math.Max(sY, pY + lineHeight);
                }
                this.Size = new SizeF(sX + 0.00001f, sY);//gap for windows -.-
            }
        }
        public void Draw(Graphics g, PointF position, SizeF size)
        {
            if (parts == null)
            {
                DrawInternal(g, position, size);
            }
            else
            {
                float pX = 0, pY = 0;
                float lineHeight = 0;
                foreach (ChatMessagePart part in parts)
                {
                    if ((pX + part.Size.Width > Size.Width) && lineHeight != 0)
                    {
                        pX = 0;
                        pY += lineHeight;
                        lineHeight = 0;
                    }
                    lineHeight = Math.Max(lineHeight, part.Size.Height);
                    part.Draw(g, new PointF(position.X + pX, lineHeight - part.Size.Height + position.Y + pY), Size);
                    pX += part.Size.Width;
                }
            }

        }
        public void MouseDown(PointF location, MouseEventArgs e)
        {
            if (parts == null)
            {
                MouseDownInternal(location, e);
            }
            else
            {
                float pX = 0, pY = 0;
                float lineHeight = 0;
                foreach (ChatMessagePart part in parts)
                {
                    if ((pX + part.Size.Width > Size.Width) && lineHeight != 0)
                    {
                        pX = 0;
                        pY += lineHeight;
                        lineHeight = 0;
                    }
                    lineHeight = Math.Max(lineHeight, part.Size.Height);
                    if (new RectangleF(new PointF(pX, pY), part.Size).Contains(location))
                    {
                        part.MouseDown(new PointF(location.X - pX, location.Y - pY), e);
                        break;
                    }
                    pX += part.Size.Width;
                }
            }
        }


        protected abstract void PreParse(XmlElement element);
        protected abstract void ParseInternal(Graphics g);
        protected abstract void DrawInternal(Graphics g, PointF position, SizeF size);
        protected abstract void MouseDownInternal(PointF location, MouseEventArgs e);
    }
}

