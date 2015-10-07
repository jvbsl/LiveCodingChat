using System;

namespace NeinTom.ChatLog
{
	public class MessageInfo
	{
		public MessageInfo (float currentPosition)
		{
			CurrentPosition = currentPosition;
		}
		public float CurrentPosition{get;private set;}
	}
}

