using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeinTom.ChatLog
{
    public partial class ChatLogControl
    {
        private System.ComponentModel.IContainer components;
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Enabled = true;
            this.tmrUpdate.Interval = 40;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // ChatLogControl
            // 
            this.AutoScroll = true;
            this.Name = "ChatLogControl";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ChatLog_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChatLogControl_MouseDown);
            this.MouseEnter += new System.EventHandler(this.ChatLogControl_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ChatLogControl_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChatLogControl_MouseMove);
            this.Resize += new System.EventHandler(this.ChatLog_Resize);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Timer tmrUpdate;
    }
}
