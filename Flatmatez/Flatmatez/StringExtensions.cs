using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Flatmatez
{
	public static class StringExtensions
	{
		private static Regex _isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);

		public static bool IsGuid(this string @this)
		{
			return _isGuid.IsMatch(@this);
		}
	}
}
