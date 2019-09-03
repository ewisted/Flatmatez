using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmatez.Backend.Data.Exceptions
{
	public class UserNotFoundException : Exception
	{
		public UserNotFoundException() { }

		public UserNotFoundException(string userId)
			: base($"User came back null when searching by the following id: \"{userId}\"")
		{
		}

		public UserNotFoundException(string userId, Exception inner)
			: base($"User came back null when searching by the following id: \"{userId}\"", inner)
		{
		}
	}
}
