using System;

namespace NeinTom
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

