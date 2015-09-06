using System;
using System.Windows.Forms;
using System.Drawing;


namespace NeinTom
{
	public class CustomTabControl:TabControl
	{
		private TabPage dragged;
		public CustomTabControl ()
		{
			MouseDown += CustomTabControl_MouseDown;
			MouseMove += CustomTabControl_MouseMove;
		}

		void CustomTabControl_MouseMove (object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left || dragged == null)
				return;
			TabPage tab = getTabPageFromLocation(e.Location);

			if (tab == null || tab == dragged)
			{
				return;
			}

			Swap(dragged, tab);
			SelectedTab = dragged;

		}

		void CustomTabControl_MouseDown (object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				dragged = getTabPageFromLocation (e.Location);
			}
		}
		private void Swap(TabPage a, TabPage b)
		{
			int i = TabPages.IndexOf(a);
			int j = TabPages.IndexOf(b);
			TabPages[i] = b;
			TabPages[j] = a;
		}
		private TabPage getTabPageFromLocation(Point pt)
		{

			for (int i = 0; i < TabCount; i++)
			{
				if (GetTabRect(i).Contains(pt))
				{
					return TabPages[i];
				}
			}

			return null;
		}
	}
}

