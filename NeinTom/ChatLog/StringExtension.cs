using System;
using System.Collections.Generic;
namespace NeinTom
{
	public static class StringExtension
	{
		internal static string[] Split(this string s,char[] chars,bool keep)
		{
			if (!keep)
				return s.Split (chars);

			int found = -1;
			int oFound = 0;
			char character;
			List<string> foundStrings = new List<string> ();
			while((found = getNextIndex(s,chars,out character,found+1)) != -1)
			{
				string sub = s.Substring (oFound,found-oFound);
				if (!string.IsNullOrEmpty (sub))
					foundStrings.Add (sub);
				oFound = found;
			}
			if (found == -1 && foundStrings.Count == 0)
				return new string[]{ s };
			string lastBit = s.Substring (oFound);
			if (!string.IsNullOrEmpty(lastBit))
				foundStrings.Add (lastBit);

			return foundStrings.ToArray ();
		}
		internal static int getNextIndex(string text,char[] chars,out char foundCharacter,int startIndex=0)
		{
			for(int i=startIndex;i<text.Length;i++)
			{
				foreach(char c in chars)
				{
					if (text[i] == c)
					{
						foundCharacter = c;
						return i;
					}
				}
			}
			foundCharacter = '\0';
			return -1;
		}
	}
}

