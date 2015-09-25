using System;
using System.Xml;
using System.Collections.Generic;
namespace NeinTom
{
	internal class ContainerPart:ChatMessagePart
	{
		public ContainerPart (XmlElement xml, ChatMessagePart parent = null)
			: base (xml, parent)
		{
		}
		protected override void DrawInternal (System.Drawing.Graphics g, System.Drawing.PointF position, System.Drawing.SizeF size)
		{
			throw new NotImplementedException ();
		}
		protected override void ParseInternal (System.Drawing.Graphics g)
		{
			//TODO: throw new NotImplementedException ();
		}
		protected override void PreParse (XmlElement element)
		{
			//TODO: throw new NotImplementedException ();
		}
	}
}

