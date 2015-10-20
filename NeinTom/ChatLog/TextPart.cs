using System;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace NeinTom.ChatLog
{
    internal class TextPart : ChatMessagePart
    {
        private StringFormat stf;
        private TextFormatFlags formatFlags;
        public TextPart(XmlElement element, ChatMessagePart parent = null, Font font = null)
            : base(parent, font)
        {
            stf = StringFormat.GenericTypographic;
            stf.FormatFlags |= StringFormatFlags.NoClip;
			formatFlags = TextFormatFlags.Default | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.NoClipping;
            Text = XML = element.InnerText;
            PreParse(element);
            InitText();

        }
        public TextPart(string text, ChatMessagePart parent = null, Font font = null)
            : base(parent, font)
        {
            Text = XML = text;
            InitText();
        }

        private void InitText()
        {
			string[] splt = Text.Trim().Split(new char[] { ' ', '\n' },true);
            parts = new List<ChatMessagePart>();
            
            bool found = false;
			int currentPosition = 0;
            foreach (string txt in splt)
            {
				if (string.IsNullOrEmpty (txt)) {
					continue;
				}
				currentPosition += txt.Length;
                if(AddPatterns(txt))
                {
                    found = true;
                }
                else
                {
                    if (splt.Length > 1)
                        parts.Add(new TextPart(txt,this,this.Font));
                }

            }
            if (!found && splt.Length <= 1)
                parts = null;
        }
        private void TestPrefix(int index,string text,ref int lastMatched,ref int matched)
        {
            if (lastMatched < index)
            {
                string prefix = text.Substring(lastMatched, index - lastMatched);
                if (!string.IsNullOrEmpty(prefix))
                {
                    parts.Add(new TextPart(prefix, this, this.Font));
                    matched++;
                    lastMatched += prefix.Length;
                }
            }
        }
        private bool AddPatterns(string text)
        {
            string smileyMatch = SmileyPart.Match;
            string linkMatch = @"(?<Link>(((http|ftp|https|rtmp|rtps|apt):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)))";
            string pattern = "(.*?)(" + smileyMatch + "|" + linkMatch + "|(.*?))(.*?)";//
            int lastMatched = 0;
            MatchCollection matches = Regex.Matches(text, pattern);
            int matched = 0;
            foreach (Match m in matches)
            {
                if (string.IsNullOrEmpty(m.Value))
                    continue;
                TestPrefix(m.Index, text, ref lastMatched, ref matched);
                if (m.Groups["Smiley"].Success)
                {
                    TestPrefix(m.Groups["Smiley"].Index, text, ref lastMatched, ref matched);
                    parts.Add(new SmileyPart(m.Groups["Smiley"].Value, this));
                    lastMatched = m.Index + m.Length;
                    matched++;
                }
                if (m.Groups["Link"].Success)
                {
                    TestPrefix(m.Groups["Link"].Index, text, ref lastMatched, ref matched);
                    parts.Add(new LinkPart(m.Groups["Link"].Value, this, this.Font));
                    lastMatched = m.Index + m.Length;
                    matched++;
                }
                

            }
            if (lastMatched < text.Length && matched > 0)
            {
                string postfix = text.Substring(lastMatched);
                if (!string.IsNullOrEmpty(postfix))
                {
                    parts.Add(new TextPart(postfix, this, this.Font));
                    matched++;
                }
            }
            return matched > 0;
            /*foreach (string splt in tst)
            {
                if (string.IsNullOrEmpty(splt))
                    continue;
                if (Regex.IsMatch(splt, smileyMatch))//quick n dirty
                {
                    parts.Add(new SmileyPart(splt, this));
                }
                else if(Regex.IsMatch(splt,linkMatch))
                {
                    parts.Add(new LinkPart(splt, this, this.Font));
                }
                else
                {
                    parts.Add(new TextPart(splt, this,this.Font));
                }
            }*/
        }
		private SizeF MeasureString(Graphics g,string text,Font font)
		{
			StringFormat formatFlags = StringFormat.GenericTypographic;
			formatFlags.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			//formatFlags.GetMeasurableCharacterRangeCount ();
			formatFlags.SetMeasurableCharacterRanges(new CharacterRange[]{new CharacterRange(0,text.Length)});

			Region[] regions = g.MeasureCharacterRanges (text, font, new RectangleF (0,0,float.MaxValue,float.MaxValue), formatFlags);
			float width=0, height=0;
			foreach (Region r in regions) {
				width = Math.Max (r.GetBounds(g).Right, width);
				height = Math.Max (r.GetBounds (g).Bottom, height);
			}
			return new SizeF (width, height);
		}
        protected override void ParseInternal(Graphics g)
        {
            Size = g.MeasureString(Text, Font, SizeF.Empty, stf);
            Size = TextRenderer.MeasureText(g, Text, Font, new Size(), formatFlags);
			Size = MeasureString (g, Text, Font);
        }
        override protected void PreParse(XmlElement element)
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
        override protected void DrawInternal(Graphics g, PointF position, SizeF size)
        {
            TextRenderer.DrawText(g, Text, Font, new Rectangle((int)position.X, (int)position.Y, (int)size.Width+1, (int)size.Height+1), Color.Black, formatFlags);
            //g.DrawString(Text, Font, Brushes.Black, new RectangleF(position, size), stf);
        }

        protected override void MouseDownInternal(PointF location, MouseEventArgs e)
        {

        }

        protected override void MouseMoveInternal(PointF location, MouseEventArgs e)
        {

        }
    }
}

