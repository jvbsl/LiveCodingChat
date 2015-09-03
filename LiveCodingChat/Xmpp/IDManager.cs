using System;

namespace LiveCodingChat
{
	public class IDManager
	{
		public IDManager ()
		{
		}
		public int SendPresenceId(){
			return PresenceId++;
		}
		public int SendIqId(){
			return IqId++;
		}
		public int PresenceId{ get; private set; }
		public int IqId{ get; private set; }
	}
}

