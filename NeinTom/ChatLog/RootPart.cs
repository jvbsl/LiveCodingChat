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
    internal class RootPart:ContainerPart
    {
		
		private ChatMessage parentChatMessage;
        public RootPart(ChatMessage parentChatMessage,XmlElement xml,Font font = null)
			: base (xml, null,font)
        {
			this.parentChatMessage = parentChatMessage;
        }
        private ChatMessagePart previousHover;
		public override Control getParentControl()
		{
			return parentChatMessage.getParentControl();
		}
        public override ChatMessagePart MouseEnter(PointF location, MouseEventArgs e)
        {
            return previousHover = null;
        }
        public override ChatMessagePart MouseLeave(PointF location, MouseEventArgs e)
        {
			if (previousHover != null) {
				previousHover.MouseLeave (location, e);
			}
            return previousHover = null;
        }
        public override ChatMessagePart MouseMove(PointF location, MouseEventArgs e)
        {
            if (parts == null)
            {
                MouseMoveInternal(location, e);
                return this;
            }
            else
            {
                return MouseForwarding(location, e, new ChatMessagePart.ForwardingDelegate(
                    delegate (ChatMessagePart part, PointF loc, MouseEventArgs ev) {
                        ChatMessagePart tmp = part.MouseMove(loc, ev);
                        if (previousHover != tmp)
                        {
                            if (previousHover != null)
                            {
                                previousHover.MouseLeave(loc, ev);
                            }
                            if (tmp != null)
                            {
                                tmp.MouseEnter(loc, ev);
                            }

                        }
                        previousHover = tmp;
                    }));
            }
        }
    }
}
