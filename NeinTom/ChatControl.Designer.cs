using System;
namespace NeinTom
{
	public partial class ChatControl:System.Windows.Forms.UserControl
	{
		private void InitializeComponent()
		{
            this.spltUserList = new System.Windows.Forms.SplitContainer();
            this.spltRoom = new System.Windows.Forms.SplitContainer();
            this.txtToSend = new System.Windows.Forms.TextBox();
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.chatLog = new ChatLog.ChatLogControl();
            ((System.ComponentModel.ISupportInitialize)(this.spltUserList)).BeginInit();
            this.spltUserList.Panel1.SuspendLayout();
            this.spltUserList.Panel2.SuspendLayout();
            this.spltUserList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltRoom)).BeginInit();
            this.spltRoom.Panel1.SuspendLayout();
            this.spltRoom.Panel2.SuspendLayout();
            this.spltRoom.SuspendLayout();
            this.SuspendLayout();
            // 
            // spltUserList
            // 
            this.spltUserList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltUserList.Location = new System.Drawing.Point(0, 0);
            this.spltUserList.Name = "spltUserList";
            // 
            // spltUserList.Panel1
            // 
            this.spltUserList.Panel1.Controls.Add(this.spltRoom);
            // 
            // spltUserList.Panel2
            // 
            this.spltUserList.Panel2.Controls.Add(this.lstUsers);
            this.spltUserList.Size = new System.Drawing.Size(150, 150);
            this.spltUserList.SplitterDistance = 93;
            this.spltUserList.TabIndex = 0;
            // 
            // spltRoom
            // 
            this.spltRoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltRoom.Location = new System.Drawing.Point(0, 0);
            this.spltRoom.Name = "spltRoom";
            this.spltRoom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spltRoom.Panel1
            // 
            this.spltRoom.Panel1.Controls.Add(this.chatLog);
            // 
            // spltRoom.Panel2
            // 
            this.spltRoom.Panel2.Controls.Add(this.txtToSend);
            this.spltRoom.Size = new System.Drawing.Size(93, 150);
            this.spltRoom.SplitterDistance = 115;
            this.spltRoom.TabIndex = 0;
            // 
            // txtToSend
            // 
            this.txtToSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtToSend.Location = new System.Drawing.Point(0, 0);
            this.txtToSend.Multiline = true;
            this.txtToSend.Name = "txtToSend";
            this.txtToSend.Size = new System.Drawing.Size(93, 31);
            this.txtToSend.TabIndex = 0;
            this.txtToSend.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtToSend_KeyDown);
            // 
            // lstUsers
            // 
            this.lstUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstUsers.Location = new System.Drawing.Point(0, 0);
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.Size = new System.Drawing.Size(53, 150);
            this.lstUsers.TabIndex = 0;
            // 
            // chatLog
            // 
            this.chatLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatLog.Font = new System.Drawing.Font("Arial", 12F);
            this.chatLog.Location = new System.Drawing.Point(0, 0);
            this.chatLog.Name = "chatLog";
            this.chatLog.ShowTimeStamp = false;
            this.chatLog.Size = new System.Drawing.Size(93, 115);
            this.chatLog.TabIndex = 0;
            this.chatLog.TimeStampFormat = null;
            // 
            // ChatControl
            // 
            this.Controls.Add(this.spltUserList);
            this.Name = "ChatControl";
            this.spltUserList.Panel1.ResumeLayout(false);
            this.spltUserList.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltUserList)).EndInit();
            this.spltUserList.ResumeLayout(false);
            this.spltRoom.Panel1.ResumeLayout(false);
            this.spltRoom.Panel2.ResumeLayout(false);
            this.spltRoom.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltRoom)).EndInit();
            this.spltRoom.ResumeLayout(false);
            this.ResumeLayout(false);

		}



		private System.Windows.Forms.SplitContainer spltUserList;
		private System.Windows.Forms.SplitContainer spltRoom;
		private System.Windows.Forms.TextBox txtToSend;
		private System.Windows.Forms.ListBox lstUsers;
        private ChatLog.ChatLogControl chatLog;
    }
}

