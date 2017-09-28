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
	        foreach (var line in lines) Add(UnitParser.ParseLine(line, this));
	    }

	    public static IEnumerable<string> DefaultUnits()
	    {
	        return new[]
	        {
	            "", "kg", "m", "s", "A", "K", "mol",
	            "N = kg*m/s^2", "J = kg*m^2/s^2", "W = kg*m^2/s^3",
	            "V = kg*m^2/(s^3*A^1)", "F = s^4*A^2/(kg*m^2)", "Ω = kg*m^2/(s^3*A^2)",
	            "S = s^3*A^2/(kg*m^2)", "Pa = N/(m^2)", "Pas = kg(m*s)",
	            "C = A*s", "Hz = 1/s"
	        };
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
