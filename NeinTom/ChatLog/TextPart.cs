using System;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;

namespace NeinTom
{
	internal class TextPart:ChatMessagePart
	{
		private StringFormat stf;
		public TextPart (string text,XmlElement element, ChatMessagePart parent = null)
			: base (parent)
		{
			stf = StringFormat.GenericTypographic;
			stf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			Text = XML = text;
			string[] splt = text.Trim().Split (new char[]{ ' ', '\n' });
			if (splt.Length > 1) {
				parts = new List<ChatMessagePart> ();
				foreach (string txt in splt)
					parts.Add (new TextPart (txt,element, this));
			}
			if (parent != null && parent is TextPart)
				FontStyle = ((TextPart)parent).FontStyle;
			PreParse (element);
		}
		protected override void ParseInternal (Graphics g)
		{
			Size = g.MeasureString(Text, new Font (new FontFamily ("Arial"),12, FontStyle),SizeF.Empty,stf);
		}
		override protected void PreParse (XmlElement element)
		{
			if (parent is TextPart)
				FontStyle = ((TextPart)parent).FontStyle;
			if (element.Name == "b")
				FontStyle |= FontStyle.Bold;
			else if(element.Name == "i")
				FontStyle |= FontStyle.Italic;
			else if(element.Name == "s")
				FontStyle |= FontStyle.Strikeout;
			else if(element.Name == "u")
				FontStyle |= FontStyle.Underline;
		}
		override protected void DrawInternal (Graphics g, PointF position, SizeF size)
		{
			g.DrawString (Text, new Font (new FontFamily ("Arial"),12, FontStyle), Brushes.Black, new RectangleF (position, size));
		}
		public FontStyle FontStyle{ get; private set; }
	}
}

