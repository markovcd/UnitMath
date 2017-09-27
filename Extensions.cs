using System;
using System.Linq;
using System.Collections.Generic;

namespace UnitMath
{
	/// <summary>
	/// Description of Extensions.
	/// </summary>
	public static class Extensions
	{
		public static string Aggregate(this IEnumerable<Unit> units)
		{
			return units.Any() ? units.Select(u => u.ToString()).Aggregate((a, b) => a + "*" + b) : "";
		}
	}
}
