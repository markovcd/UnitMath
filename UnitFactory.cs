using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace UnitMath
{
	
    
    /// <summary>
	/// Description of UnitFactory.
	/// </summary>
	public class UnitFactory : IDictionary<string, Unit>
	{
		private readonly IDictionary<string, Unit> _dictionary;

	    public ICollection<string> Keys { get { return _dictionary.Keys; } }
	    public ICollection<Unit> Values { get { return _dictionary.Values; } }
	    public int Count { get { return _dictionary.Count; } }
	    public bool IsReadOnly { get { return _dictionary.IsReadOnly; } }

	    public Unit this[string key]
	    {
	        get { return _dictionary[key]; }
	        set { _dictionary[key] = value; }
	    }

        public UnitFactory(bool addDefaultUnits = true)
	    {
	        _dictionary = new Dictionary<string, Unit>();

	        if (addDefaultUnits) LoadFromLines(DefaultUnits());
	    }

        public void Add(Unit u)
	    {
	        Add(u.Symbol, u);
	    }

	    public void Add(KeyValuePair<string, Unit> item)
	    {
	        _dictionary.Add(item);
	    }

	    public void Clear()
	    {
	        _dictionary.Clear();
	    }

	    public bool Contains(KeyValuePair<string, Unit> item)
	    {
	        return _dictionary.Contains(item);
	    }

	    public void CopyTo(KeyValuePair<string, Unit>[] array, int arrayIndex)
	    {
	        _dictionary.CopyTo(array, arrayIndex);
	    }

	    public bool Remove(KeyValuePair<string, Unit> item)
	    {
	        return _dictionary.Remove(item);
	    }

	    public void LoadFromFile(string path)
	    {
	        LoadFromLines(File.ReadAllLines(path));
	    }

	    public void LoadFromLines(IEnumerable<string> lines)
	    {
	        foreach (var line in lines) Add(ParseLine(line));
	    }

	    public IEnumerable<string> DefaultUnits()
	    {
	        return new[]
	        {
	            "", "kg", "m", "s", "A", "K", "mol",
	            "N = kg*m/s^2", "J = kg*m^2/s^2", "W = kg*m^2/s^3",
	            "V = kg*m^2/(s^3*A^1)", "F = s^4*A^2/(kg*m^2)", "Ω = kg*m^2/(s^3*A^2)",
	            "S = s^3*A^2/(kg*m^2)", "Pa = kg/(m*s^2)", "Pas = kg(m*s)",
	            "C = A*s", "Hz = 1/s"
	        };
	    }

	    /// <summary> 
	    /// Parse derived unit (ex. "N=kg*m/s^2")
	    /// </summary> 
        public Unit ParseLine(string line)
		{
			var data = line.Split('=').Select(s => s.Trim()).ToArray();

            switch (data.Length)
            {
                case 0:
                    return new Unit("");
                case 1:
                    if (data[0].Contains('*') || data[0].Contains('/')) return new Unit("<undefined>", 1, ParseUnits(data[0]));
                    return Parse(data[0]);
            }

		    return new Unit(data[0], 1, ParseUnits(data[1]));
		}

	    /// <summary> 
	    /// Parse unit from string (ex. "m^2")
	    /// </summary> 
        public Unit Parse(string text)
		{
			var data = text.Trim().Split('^');
			var symbol = data[0] == "1" ? "" : data[0];
			var power = decimal.Parse(data.Length == 2 ? data[1] : "1");

			Unit u;
			return TryGetValue(symbol, out u) ? u.ChangePower(power) : new Unit(symbol, power);
		}

	    /// <summary> 
	    /// Parse multiple units from string (ex. "m^2*s*^3/kg")
	    /// </summary> 
        public IEnumerable<Unit> ParseUnits(string text)
		{
			text = new string(text.Where(c => c != ' ' && c != '\t' && c != '(' && c != ')').ToArray());
			
			return text.Split('/')
					   .Select(s => s.Split('*').Select(Parse))
					   .Select((e, i) => i == 1 ? Unit.Invert(e) : e)
					   .SelectMany(e => e);
		}

	    public IEnumerator<KeyValuePair<string, Unit>> GetEnumerator()
	    {
	        return _dictionary.GetEnumerator();
	    }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
	    }

	    public bool ContainsKey(string key)
	    {
	        return _dictionary.ContainsKey(key);
	    }

	    public void Add(string key, Unit value)
	    {
	        _dictionary.Add(key, value);
	    }

	    public bool Remove(string key)
	    {
	        return _dictionary.Remove(key);
	    }

	    public bool TryGetValue(string key, out Unit value)
	    {
	        return _dictionary.TryGetValue(key, out value);
	    }
	}
}
