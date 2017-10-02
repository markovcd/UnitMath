using System;
using System.Linq;
using System.Collections.Generic;

namespace UnitMath
{
	/// <summary>
	/// Description of UnitParser.
	/// </summary>
	public static class UnitParser
	{
		/// <summary> 
	    /// Parse unit from string (ex. "m^2")
	    /// </summary> 
        public static Unit Parse(string text, IDictionary<string, Unit> dict = null)
		{
			var data = text.Trim().Split('^');
			var symbol = data[0] == "1" ? "" : data[0];
			var power = decimal.Parse(data.Length == 2 ? data[1] : "1");
			
			if (dict != null && dict.ContainsKey(symbol)) return dict[symbol].ChangePower(power);
			return new Unit(symbol, power);
		}

	    /// <summary> 
	    /// Parse multiple units from string (ex. "m^2*s*^3/kg")
	    /// </summary> 
        public static IEnumerable<Unit> ParseUnits(string text, IDictionary<string, Unit> dict = null)
		{       	
        	text = new string(text.Where(c => c != ' ' && c != '\t' && c != '(' && c != ')').ToArray());
			
        	return text.Split('/')
        			   .Select(s => s.Split('*').Select(s2 => Parse(s2, dict)))
        			   .Select((e, i) => i == 1 ? Unit.Invert(e) : e)
        			   .SelectMany(e => e)
                       .Where(u => u.Symbol != "");
		}
        
        /// <summary> 
	    /// Parse derived unit (ex. "N=kg*m/s^2")
	    /// </summary> 
        public static Unit ParseLine(string line, IDictionary<string, Unit> dict = null)
		{

            var data = line.Split('=').Select(s => s.Trim()).ToArray();

            if (data.Length > 2) throw new InvalidOperationException("Can't have multiple equal signs in statement.");
		    if (data.Length == 2) return new Unit(data[0], 1, ParseUnits(data[1], dict));
		    if (data[0].Contains('*') || data[0].Contains('/')) return new Unit(data[0], 1, ParseUnits(data[0], dict));
		    return Parse(data[0], dict);
		}
		
	}
}
