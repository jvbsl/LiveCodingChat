using System;

namespace LiveCodingChat
{
	public class HtmlHelper
	{
		public static string getString(string data,int fInd=0,char bracket='\"')
		{
			int strStart = data.IndexOf (bracket, fInd);
			if (strStart == -1) {
				if (bracket == '\'')
					return null;
				return getString (data, fInd, '\'');
			}
			strStart++;
			int eInd = data.IndexOf (bracket, strStart);

			return data.Substring (strStart, eInd - strStart);
		}
		public static string getAttribute(string data,string key)
		{
			int fInd = data.IndexOf (key);
			if (fInd == -1)
				return null;
			fInd += key.Length;
			fInd = data.IndexOf ("=", fInd);
			if (fInd == -1)
				return null;
			fInd++;
			return getString (data, fInd);
		}
		public static string getElement(string data,string element)
		{
			string elType = getElementType (element);
			int startEl, endEl,tempEl,searchStart;
			startEl = data.IndexOf (element);
			if (startEl == -1)
				return null;
			searchStart = startEl+ element.Length;
			int stack = 1;
			do {
				endEl = data.IndexOf ("<" + elType, searchStart);
				tempEl = data.IndexOf ("</" + elType, searchStart);
				int dClose = data.IndexOf("/>",searchStart);
				if (dClose != -1 && dClose < tempEl && data.IndexOf("<",searchStart+1) > dClose)
				{
					if (stack == 1)
						return data.Substring(startEl,dClose+1-startEl);
					continue;
				}
				if (endEl != -1 && endEl < tempEl)
				{
					stack++;
					searchStart = endEl+1;
				}
				else if(endEl>tempEl || endEl ==-1)
				{
					if (stack > 1)
					{
						stack--;
						searchStart = tempEl+1;
						continue;
					}
					if (tempEl == -1)
						return null;
					tempEl = data.IndexOf(">",tempEl);
					return data.Substring(startEl,tempEl-startEl+1);
				}

				if (tempEl == -1)
					return data.Substring (startEl);
			} while(stack > 0);
			return null;

		}
		public static string getElementType(string element)
		{
			int start = 0;
			if (element.StartsWith ("<"))
				start += 1;
			else if(element.StartsWith("</"))
				start += 2;
			int end;
			for (end = start; end < element.Length; end++) {
				if (!Char.IsLetterOrDigit (element [end]))
					break;
			}
			return element.Substring (start, end - start);
		}
		public static string getSingleElement(string data,string element)
		{
			int fInd = data.IndexOf (element);
			if (fInd == -1)
				return null;
			int eInd = data.IndexOf (">", fInd + element.Length);
			if (eInd == -1)
				eInd = data.Length - 1;
			return data.Substring (fInd, eInd - fInd+1);
		}
	}
}

