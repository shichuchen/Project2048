using System;

namespace Project2048
{
    public class NamedDouble : IComparable<double>, IEquatable<double>
    {
        internal double Value { get; }
        protected NamedDouble() { }
        protected NamedDouble(double val) { Value = val; }
        protected NamedDouble(string val) { Value = Convert.ToDouble(val); }
        public static implicit operator double(NamedDouble val) { return val.Value; }
        public static bool operator ==(NamedDouble a, double b) { return a?.Value == b; }
        public static bool operator ==(NamedDouble a, NamedDouble b) { return a?.Value == b?.Value; }
        public static bool operator !=(NamedDouble a, double b) { return !(a == b); }
        public static bool operator !=(NamedDouble a, NamedDouble b) { return !(a == b); }
        public bool Equals(double other)
        {
            return Equals(new NamedDouble(other));
        }
        public override bool Equals(object other)
        {
            if (other.GetType() != GetType() && other.GetType() != typeof(string))
            {
                return false;
            }
            return Equals((NamedDouble)other);
        }
        public bool Equals(NamedDouble other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(Value, other.Value);

        }
        public int CompareTo(double other)
        {
            if (Value < other)
            {
                return -1;
            }
            if (Value > other)
            {
                return 1;
            }
            return 0;
        }
        public int CompareTo(NamedDouble other)
        {
            if (Value < other.Value)
            {
                return -1;
            }
            if (Value > other.Value)
            {
                return 1;
            }
            return 0;
        }
        public override int GetHashCode() { return Value.GetHashCode(); }

        public override string ToString() { return Value.ToString(); }


    }
}
