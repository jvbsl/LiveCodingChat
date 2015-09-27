using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace NeinTom
{
    internal class SmileyPart : ChatMessagePart
    {
        private Bitmap smiley;
        public SmileyPart(string smileyType,ChatMessagePart parent=null)
            :base(parent)
        {
            smiley = Properties.Resources.Smiley;
        }
        protected override void DrawInternal(Graphics g, PointF position, SizeF size)
        {
            g.DrawImage(smiley, new RectangleF(position, Size));
        }

        protected override void MouseDownInternal(PointF location, MouseEventArgs e)
        {
            MessageBox.Show("Bla");
        }

        protected override void ParseInternal(Graphics g)
        {
            Size = new SizeF(24,24);
        }

        protected override void PreParse(XmlElement element)
        {
            
        }
    }
}
