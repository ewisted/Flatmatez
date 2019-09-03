using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmatez.Backend.Data.Exceptions
{
	public class BillNotFoundException : Exception
	{
		public BillNotFoundException() { }

		public BillNotFoundException(string billId)
			: base($"Bill came back null when searching by the following id: \"{billId}\"")
		{
		}

		public BillNotFoundException(string billId, Exception inner)
			: base($"User came back null when searching by the following id: \"{billId}\"", inner)
		{
		}
	}
}
