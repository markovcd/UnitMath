using System;
using System.Linq;
using System.Collections.Generic;

namespace UnitMath
{
	public class UnitValue : UnitValue<double>
	{
		public UnitValue(double value, Unit unit) : base(value, unit) { }
		
		public static UnitValue<T> Create<T>(T value, Unit unit) where T: struct, IEquatable<T>, IComparable<T>
		{
			return new UnitValue<T>(value, unit);
		}
	}
	
	/// <summary>
	/// Description of UnitValue.
	/// </summary>
	public class UnitValue<T> : IEquatable<UnitValue<T>>, IComparable<UnitValue<T>> where T: struct, IEquatable<T>, IComparable<T>
	{
	    private readonly int _hashCode;

        public Unit Unit { get; private set; }
        public Unit SimplifiedUnit { get; private set; }
		public T Value { get; private set; }
		
		public UnitValue(T value, Unit unit)
		{
			Value = value;
			Unit = unit;
			SimplifiedUnit = Unit.Flatten().Simplify();

		    _hashCode = GetHashCode(this);
		}
		
		public int CompareTo(UnitValue<T> other)
		{
			if (SimplifiedUnit != other.SimplifiedUnit) throw new InvalidOperationException("Can't compare values with different units.");
			return Value.CompareTo(other.Value);
		}

		#region Equals and GetHashCode implementation

		public bool Equals(UnitValue<T> other)
		{
			return GetHashCode() == other.GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as UnitValue<T>);
		} 
		
		public override int GetHashCode()
		{
			return _hashCode;
		}

	    public static int GetHashCode(UnitValue<T> uv)
	    {
	    	return 1000000007 * uv.Value.GetHashCode() + 1000000009 * uv.SimplifiedUnit.GetHashCode();
        }

		#endregion

		#region Operator overloads
		
		public static bool operator ==(UnitValue<T> lhs, UnitValue<T> rhs)
		{
		    if (ReferenceEquals(lhs, rhs)) return true;
		    if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) return false;
		    return lhs.Equals(rhs);
        }
		
		public static bool operator !=(UnitValue<T> lhs, UnitValue<T> rhs)
		{
		    return !(lhs == rhs);
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
			return new UnitValue<T>((dynamic)lhs.Value + (dynamic)rhs.Value, Unit.GetCommon(lhs.Unit, rhs.Unit));
		}
		
		public static UnitValue<T> operator -(UnitValue<T> lhs, UnitValue<T> rhs)
		{
			return new UnitValue<T>((dynamic)lhs.Value - (dynamic)rhs.Value, Unit.GetCommon(lhs.Unit, rhs.Unit));
		}
		
		#endregion
	}
}
