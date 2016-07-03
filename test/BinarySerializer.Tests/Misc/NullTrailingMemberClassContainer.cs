﻿using BinarySerialization;

namespace BinarySerialization.Test.Misc
{
    public class NullTrailingMemberClassContainer
    {
        public NullTrailingMemberClassContainer()
        {
            Inner = new NullTrailingMemberClass();
        }

        [FieldOrder(0)]
        public int InnerLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("InnerLength")]
        public NullTrailingMemberClass Inner { get; set; }
    }
}
