using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;

namespace UnitMath
{
    public enum UnitDisplayFormat { RootTree, FirstChildren, Flattened, Simplified, FlattenedAndSimplified }

    public class Unit : IEnumerable<Unit>, IEquatable<Unit>
	{
		private readonly IEnumerable<Unit> _units;

	    public string Symbol { get; private set; }
	    public decimal Power { get; private set; }

        private readonly int _hashCode;

		public Unit(string symbol, decimal power = 1, IEnumerable<Unit> units = null)
		{
		    Symbol = symbol;
		    Power = power;
            
		    _units = units ?? Enumerable.Empty<Unit>();
		    _units = DefaultOrder( _units);

            _hashCode = GetHashCode(this);
		}

	    public Unit(string symbol, decimal power = 1, params Unit[] units) : this(symbol, power, (IEnumerable<Unit>)units) { }

		public Unit ChangePower(decimal power)
		{
			return new Unit(Symbol, power, ChangePower(this, Power, power));
		}
			
		public static IEnumerable<Unit> ChangePower(IEnumerable<Unit> units, decimal oldPower, decimal newPower)
		{
		    return units.Select(u => new Unit(u.Symbol, u.Power * newPower / oldPower, ChangePower(u, oldPower, newPower)));
		}

	    public Unit ChangeSymbol(string symbol)
	    {
	        return ChangeSymbol(this, symbol);
	    }

	    public static Unit ChangeSymbol(Unit u, string symbol)
	    {
	        return new Unit(symbol, u.Power, u);
	    }
		
		public static string Aggregate(IEnumerable<Unit> units)
		{
			return units.Any() ? units.Select(u => u.ToString()).Aggregate((a, b) => a + "*" + b) : "";
		}
		
		private string ToString2()
		{
			if (!this.Any()) return ToString();
			
			var positive = Aggregate(this.Where(u => u.Power > 0)
			                         	 .OrderByDescending(u => u.Power));
			
			var negative = Aggregate(this.Where(u => u.Power < 0)
							   			 .OrderBy(u => u.Power)
							   			 .Select(u => u.Invert()));
			
			if (this.Count(u => u.Power < 0) > 1) negative = "(" + negative + ")";
			
			if (positive != "" && negative == "") return positive;
			if (positive == "" && negative != "") return "1/" + negative;
			if (positive != "" && negative != "") return positive + "/" + negative;
			
			return "";
		}
		
		public override string ToString()
		{
			if (Power == 0) return "";
			return Symbol + (Power == 1 ? "" : "^" + Power);
		}

	    public string ToString(UnitDisplayFormat format)
	    {
	        switch (format)
	        {
	            case UnitDisplayFormat.RootTree:
	                return ToString();
	            case UnitDisplayFormat.FirstChildren:
	                return ToString2();
                case UnitDisplayFormat.Flattened:
                    return Flatten().ToString2();
                case UnitDisplayFormat.Simplified:
                    return Simplify().ToString2();
                case UnitDisplayFormat.FlattenedAndSimplified:
                    return Flatten().Simplify().ToString2();

                default:
	                throw new ArgumentOutOfRangeException("format", format, null);
	        }
	    }

	    public Unit Simplify()
	    {
	    	return new Unit(Symbol, Power, Multiply(this));
	    }
		
		public static IEnumerable<Unit> Flatten(Unit unit)
		{
		    if (!unit.Any()) 
		    {
		    	yield return unit;
		    	yield break;
		    }
		    
	        foreach (var units in unit)
	        {
	            foreach (var units2 in Flatten(units))
	            {
	                yield return units2;
	            }
	        }
		}

	    public Unit Flatten()
	    {
	        return new Unit(Symbol, Power, Flatten(this));
	    }
	    
	    public static IEnumerable<Unit> Multiply(IEnumerable<Unit> units)
	    {
	    	return DefaultOrder(units.GroupBy(u => u.Symbol)
	    				             .Select(g => new Unit(g.Key, 
                                                           g.Sum(u => u.Power), 
                                                           Multiply(g.SelectMany(u => u)))));
	    }
	    
	    public static IEnumerable<Unit> Invert(IEnumerable<Unit> units)
	    {
	        return units.Select(u => new Unit(u.Symbol, -u.Power, Invert(u)));
	    }
	    
	    public Unit Invert()
	    {
	    	return new Unit(Symbol, -Power, Invert(this));
	    }

	    public static Unit GetCommon2(Unit u1, Unit u2)
	    {
	        if (u1 == u2) return u1;
	        if (u1 == null || u2 == null) return null;

            var hash1 = new HashSet<Unit>(u1.Any() ? u1 : new[] { u1 }.AsEnumerable());
	    	var hash2 = new HashSet<Unit>(u2.Any() ? u2 : new[] { u2 }.AsEnumerable());
	    	
	    	var common = hash1.Intersect(hash2);
	    	var notCommon1 = Multiply(hash1.Except(common));
	        var notCommon2 = Multiply(hash2.Except(common));
	    	
	    	if (GetHashCode(notCommon1) != GetHashCode(notCommon2)) return null;
	    	
	    	return new Unit("", 1, notCommon1.Concat(common));
	    }

	    public static bool ShallowEquals(Unit u1, Unit u2)
	    {
	        return u1.Symbol == u2.Symbol && u1.Power == u2.Power;
	    }

	    public static IEnumerable<Unit> DefaultOrder(IEnumerable<Unit> units)
	    {
	        return units.OrderBy(u => u.Symbol).OrderByDescending(u => u.Power);
	    }

	    public static IEnumerable<Unit> GetCommon(Unit u1, Unit u2)
	    {
	        if (ShallowEquals(u1, u2))
	        {
	            yield return u1;
                yield break;
	        }

	        var flat1 = DefaultOrder(Flatten(u1));
	        var flat2 = DefaultOrder(Flatten(u2));

	        if (GetHashCode(flat1) != GetHashCode(flat2))
	        {
	            var multiplied1 = Multiply(flat1);
	            var multiplied2 = Multiply(flat2);

	            if (GetHashCode(multiplied1) != GetHashCode(multiplied2)) yield break;

	            foreach (var u in multiplied1) yield return u;
	            
                yield break;
	        }

	        var stack1 = new Stack<Unit>(u1);
            var stack2 = new Stack<Unit>();

	        var common = new HashSet<Unit>(u1);
	        common.IntersectWith(u2);

	        foreach (var u in common) yield return u;
	        

	        foreach (var u12 in u2)
	        {
                if (ShallowEquals(stack1.Peek(), u12)) stack2.Push(stack1.Pop());
	        }
	    }
	 
	    
	    #region Operator overloads
	    
	    public static bool operator ==(Unit lhs, Unit rhs) 
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Unit lhs, Unit rhs) 
		{
			return !(lhs == rhs);
		}
		
		public static Unit operator *(Unit lhs, Unit rhs)
		{
			var units = Multiply(lhs.Concat(rhs));
			return lhs.Symbol == rhs.Symbol ?
				new Unit(lhs.Symbol, lhs.Power + rhs.Power, units) : 
				new Unit(Aggregate(units), 1, units);
		}
		
		public static Unit operator /(Unit lhs, Unit rhs)
		{
			return lhs * rhs.Invert();
		}
	    
	    #endregion
	    
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return Equals(obj as Unit);
		}

		public bool Equals(Unit other)
		{
			if (other == null) return false;
			return GetHashCode() == other.GetHashCode();
		}

		public override int GetHashCode()
		{
    		return _hashCode;
		}	 

	    public static int GetHashCode(Unit unit)
		{
		    unchecked
		    {
		        var hashCode = GetHashCode((IEnumerable<Unit>)unit);
            	hashCode += 1000000007 * unit.Symbol.GetHashCode();
	            hashCode += 1000000009 * unit.Power.GetHashCode();
		        return hashCode;
            }
		}

	    public static int GetHashCode(IEnumerable<Unit> units)
	    {
	        unchecked
	        {
	            return units.Any() ? units.Aggregate(1, (current, u) => 31 * current + u.GetHashCode()) : 0;
	        }
	    }

		#endregion

		#region IEnumerable implementation
		
		public IEnumerator<Unit> GetEnumerator()
		{
			return _units.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion


	   
	}

}