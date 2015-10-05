using System;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NeinTom.ChatLog
{
	internal class ContainerPart:ChatMessagePart
	{
		public ContainerPart (XmlElement xml, ChatMessagePart parent = null, Font font = null)
			: base (xml, parent,font)
		{
            
		}
		protected override void DrawInternal (System.Drawing.Graphics g, System.Drawing.PointF position, System.Drawing.SizeF size)
		{
			throw new NotImplementedException ();
		}

        protected override void MouseDownInternal(PointF location, MouseEventArgs e)
        {
            
        }

        protected override void MouseMoveInternal(PointF location, MouseEventArgs e)
        {

        }

        protected override void ParseInternal (System.Drawing.Graphics g)
		{
			//TODO: throw new NotImplementedException ();
		}
		protected override void PreParse (XmlElement element)
		{

            if (parent != null)
                this.Font = parent.Font;
            FontStyle style = Font.Style;
            if (element.Name == "b")
                style |= FontStyle.Bold;
            else if (element.Name == "i")
                style |= FontStyle.Italic;
            else if (element.Name == "s")
                style |= FontStyle.Strikeout;
            else if (element.Name == "u")
                style |= FontStyle.Underline;
            else
                return;

            this.Font = new Font(Font, style);
        }
	}
}

