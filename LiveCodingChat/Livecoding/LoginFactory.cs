using System;
using System.Collections.Generic;
using System.Reflection;

namespace LiveCodingChat
{
	public class LoginFactory
	{

		private static LoginFactory instance;
		public static LoginFactory Instance {
			get {
				if (instance == null)
					instance = new LoginFactory ();
				return instance;
			}
		}

		private LoginFactory ()
		{
			LoginMethods = new Dictionary<string,Type> ();
			RegisterLoginMethods (Assembly.GetExecutingAssembly ());
		}
		public Dictionary<string,Type> LoginMethods{ get; private set; }
		public void RegisterLoginMethods(Assembly assembly)
		{
			foreach (Type t in assembly.GetTypes()) {
				if (typeof(ILoginMethod).IsAssignableFrom (t) && !t.IsInterface && !t.IsAbstract) {
					if (LoginMethods.ContainsKey (t.Name))
						continue;
					LoginMethods.Add (t.Name,t);
				}
			}
		}
		public ILoginMethod CreateInstance(string name)
		{
			if (LoginMethods.ContainsKey (name)) {
				return (ILoginMethod)LoginMethods [name].GetConstructor (new Type[]{ }).Invoke (new object[]{ });
			}
			return null;
		}
	}
}

