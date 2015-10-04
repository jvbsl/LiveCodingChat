using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace NeinTom.ChatLog
{
    internal class SmileyPart : ChatMessagePart
    {

        private static string match;
        private static Dictionary<string, string> emoticons;
        private static Dictionary<string, Bitmap> animations;
        static SmileyPart()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.emoticons);
            emoticons = new Dictionary<string, string>();
            animations = new Dictionary<string, Bitmap>();
            XmlNode el = doc.FirstChild;
            while(el.NodeType != XmlNodeType.Element)
            {
                el = el.NextSibling;
            }
            StringBuilder matchBuilder = new StringBuilder();
            foreach (XmlElement emojiElement in el.ChildNodes)
            {
                if (emojiElement.HasAttribute("file"))
                {
                    string emojiFile = emojiElement.Attributes["file"].Value;
                    foreach (XmlElement emojiSymbol in emojiElement.ChildNodes)
                    {
                        if (emoticons.ContainsKey(emojiSymbol.InnerText))
                            continue;
                        if (matchBuilder.Length != 0)
                            matchBuilder.Append('|');
                        matchBuilder.Append(Regex.Escape(emojiSymbol.InnerText));

                        emoticons.Add(emojiSymbol.InnerText, emojiFile);
                    }
                }
            }
            //LoadEmojies();
            match = "(?<Smiley>(" + matchBuilder.ToString() + "))";
        }
        public static string Match
        {
            get
            {
                return match;
            }
        }
        private static void LoadEmojies()
        {
            foreach (KeyValuePair<string,string> e in emoticons)
            {
                string file = e.Key;
                Bitmap emoji;
                if (!animations.TryGetValue(file, out emoji))
                {
                    object res = Properties.Resources.ResourceManager.GetObject(System.IO.Path.GetFileNameWithoutExtension(file));
                    if (res is Bitmap)
                        emoji = (Bitmap)res;
                    if (emoji != null && ImageAnimator.CanAnimate(emoji))
                    {
                        ImageAnimator.Animate(emoji, new EventHandler(OnFrameChanged));
                    }
                }
            }

        }
        public static Bitmap getEmoji(string text)
        {
            string file;
            if (emoticons.TryGetValue(text,out file))
            {
                Bitmap emoji=null;
                if (!animations.TryGetValue(file,out emoji))
                {
                    object res = Properties.Resources.ResourceManager.GetObject(System.IO.Path.GetFileNameWithoutExtension(file));
                    if (res is Bitmap)
                        emoji = (Bitmap)res;
                    if (emoji != null && ImageAnimator.CanAnimate(emoji))
                    {
                        ImageAnimator.Animate(emoji, new EventHandler(OnFrameChanged));
                    }
                    animations.Add(file, emoji);
                }
                return emoji;
            }
            return null;
        }
        private static void Animate()
        {
            ImageAnimator.UpdateFrames();
        }
        private Bitmap smiley;
        public SmileyPart(string smileyType, ChatMessagePart parent = null)
            : base(parent)
        {
            smiley = getEmoji(smileyType);
        }
        private static void OnFrameChanged(object sender, EventArgs e)
        {

        }
        protected override void DrawInternal(Graphics g, PointF position, SizeF size)
        {
            Animate();
            g.DrawImage(smiley, new RectangleF(position, Size));
        }

        protected override void MouseDownInternal(PointF location, MouseEventArgs e)
        {
            MessageBox.Show("Bla");
        }

        protected override void ParseInternal(Graphics g)
        {
            Size = smiley.Size;//new SizeF(24,24);
        }

        protected override void PreParse(XmlElement element)
        {

        }
    }
}
