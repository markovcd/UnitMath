using System;
using System.Linq;
using System.Collections.Generic;

namespace UnitMath
{
    public enum UnitDisplayFormat { RootTree, FirstChildren, Flattened, Simplified, FlattenedAndSimplified }

    public class Unit : IEnumerable<Unit>, IEquatable<Unit>
	{
		private readonly IEnumerable<Unit> _units;

	    public static string UndefinedSymbol = "<undefined>";

	    public string Symbol { get; private set; }
	    public decimal Power { get; private set; }

        private readonly int _hashCode;

		public Unit(string symbol, decimal power = 1, IEnumerable<Unit> units = null)
		{
		    Symbol = symbol;
		    Power = power;
            
		    _units = units ?? Enumerable.Empty<Unit>();
			_hashCode = GetHashCode(this);
		}

		public Unit ChangePower(decimal power)
		{
			return new Unit(Symbol, power, ChangePower(this, Power, power));
		}
			
		public static IEnumerable<Unit> ChangePower(IEnumerable<Unit> units, decimal oldPower, decimal newPower)
		{
		    return units.Select(u => new Unit(u.Symbol, u.Power * newPower / oldPower, ChangePower(u, oldPower, newPower)));
		}
		
		private string ToString2()
		{
			if (!this.Any()) return ToString();
			
			var positive = this.Where(u => u.Power > 0)
							   .OrderByDescending(u => u.Power)
							   .Aggregate();
			
			var negative = this.Where(u => u.Power < 0)
							   .OrderBy(u => u.Power)
							   .Select(u => u.Invert())
							   .Aggregate();
			
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
		    if (!unit.Any()) yield return unit;
		    else
		    {
		        foreach (var units in unit)
		        {
		            foreach (var units2 in Flatten(units))
		            {
		                yield return units2;
		            }
		        }
		    }
            
		}

	    public Unit Flatten()
	    {
	        return new Unit(Symbol, Power, Flatten(this));
	    }
	    
	    public static IEnumerable<Unit> Multiply(IEnumerable<Unit> units)
	    {
	    	return units.GroupBy(u => u.Symbol)
	    				.Select(g => new Unit(g.Key, g.Sum(u => u.Power), Multiply(g.SelectMany(u => u))))
	    				.OrderByDescending(u => u.Power);
	    }
	    
	    public static IEnumerable<Unit> Invert(IEnumerable<Unit> units)
	    {
	        return units.Select(u => new Unit(u.Symbol, -u.Power, Invert(u)));
	    }
	    
	    public Unit Invert()
	    {
	    	return new Unit(Symbol, -Power, Invert(this));
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
				new Unit(UndefinedSymbol, 1, units);
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
			var hashCode = 1;
		    unchecked
		    {
		    	if (unit.Any()) hashCode = unit.Aggregate(hashCode, (current, u) => 31 * current + u.GetHashCode());
                
            	hashCode += 1000000007 * unit.Symbol.GetHashCode();
	            hashCode += 1000000009 * unit.Power.GetHashCode();
                
			}
			
    		return hashCode;
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