using System.Collections.Generic;

namespace BinarySerialization.Test.Misc
{
    public class NoConstructorList : List<EmptyClass>
    {
        public NoConstructorList(int capacity) : base(capacity)
        {
        }
    }
}
