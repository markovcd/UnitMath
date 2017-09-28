using System;
using System.Linq;
using System.Collections.Generic;

namespace UnitMath
{
	public class UnitDouble : UnitValue<double>
	{
		public UnitDouble(double value, Unit unit) : base(value, unit) { }
	}
	
	/// <summary>
	/// Description of UnitValue.
	/// </summary>
	public class UnitValue<T> : IEquatable<UnitValue<T>>, IComparable<UnitValue<T>> where T: struct, IEquatable<T>, IComparable<T>
	{
	    private readonly int _hashCode;

        public Unit Unit { get; private set; }
		public T Value { get; private set; }
		
		public UnitValue(T value, Unit unit)
		{
			Value = value;
			Unit = unit;

		    _hashCode = GetHashCode(this);
		}

		#region IEquatable implementation

		public bool Equals(UnitValue<T> other)
		{
			return Value.Equals(other.Value) && Unit.Equals(other.Unit);
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as UnitValue<T>);
		} 
		
		public override int GetHashCode()
		{
			return _hashCode;
		}

	    private static int GetHashCode(UnitValue<T> u)
	    {
	        return 1000000007 * u.Value.GetHashCode() + 1000000009 * u.GetHashCode();
        }

		#endregion

		#region IComparable implementation

		public int CompareTo(UnitValue<T> other)
		{
			// VerifyUnits(); check units are equivalent
			return Value.CompareTo(other.Value);
		}

		#endregion
		
		#region Operator overloads
		
		public static bool operator ==(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			return lhs.CompareTo(rhs) == 0;
		}
		
		public static bool operator !=(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			return lhs.CompareTo(rhs) != 0;
		}
		
		public static bool operator >(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			return lhs.CompareTo(rhs) == 1;
		}
		
		public static bool operator <(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			return lhs.CompareTo(rhs) == -1;
		}
		
		public static bool operator >=(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			return lhs > rhs || lhs == rhs;
		}
		
		public static bool operator <=(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			return lhs < rhs || lhs == rhs;
		}
		
		public static UnitValue<T> operator *(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			return new UnitValue<T>((dynamic)lhs.Value * (dynamic)rhs.Value, lhs.Unit * rhs.Unit);
		}
		
		public static UnitValue<T> operator /(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			return new UnitValue<T>((dynamic)lhs.Value / (dynamic)rhs.Value, lhs.Unit / rhs.Unit);
		}
		
		public static UnitValue<T> operator +(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			// VerifyUnits(); check units are equivalent
			return new UnitValue<T>((dynamic)lhs.Value + (dynamic)rhs.Value, null);
		}
		
		public static UnitValue<T> operator -(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			// VerifyUnits(); check units are equivalent
			return new UnitValue<T>((dynamic)lhs.Value - (dynamic)rhs.Value, null);
		}
		
		#endregion
	}
}
