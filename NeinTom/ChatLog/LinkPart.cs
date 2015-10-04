using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace NeinTom.ChatLog
{
    internal class LinkPart : ChatMessagePart
    {
        private Font defaultFont;
        private Font clickedFont;
        private TextFormatFlags formatFlags;
        private string link;
        public LinkPart(ChatMessagePart parent=null,Font font = null)
            :base(parent,font)
        {
            if (font == null)
                defaultFont = new Font(parent.Font, parent.Font.Style | FontStyle.Underline);
            else
                defaultFont = new Font(font, font.Style | FontStyle.Underline);

            formatFlags = TextFormatFlags.Default | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;

            clickedFont = defaultFont;
        }
        public LinkPart(string text, ChatMessagePart parent = null, Font font = null)
            :this(parent, font)
        {
            Text = text;
            link = text;
        }
        
        protected override void DrawInternal(Graphics g, PointF position, SizeF size)
        {
            TextRenderer.DrawText(g, Text, defaultFont, new Rectangle((int)position.X, (int)position.Y, (int)size.Width, (int)size.Height), Color.Blue,formatFlags);
        }

        protected override void MouseDownInternal(PointF location, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start(link);
        }

        protected override void ParseInternal(Graphics g)
        {
            Size = TextRenderer.MeasureText(g, Text, Font, new Size(), formatFlags);
        }

        protected override void PreParse(XmlElement element)
        {
            if (element.Name == "a")
            {
                link = element.Attributes["href"].Value;
                Text = element.InnerText;
            }
        }
    }
}
