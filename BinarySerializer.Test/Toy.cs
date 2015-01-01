using System;
using BinarySerialization;

namespace BinarySerializer.Test
{
    public class Toy : IEquatable<Toy>
    {
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ Last.GetHashCode();
            }
        }

        public Toy()
        {    
        }

        public Toy(string name, bool last = false)
        {
            Name = name;
            Last = last;
        }

        [FieldOrder(0)]
        public string Name { get; set; }

        [FieldOrder(1)]
        public bool Last { get; set; }

        public bool Equals(Toy other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Last.Equals(other.Last);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Toy) obj);
        }
    }
}
