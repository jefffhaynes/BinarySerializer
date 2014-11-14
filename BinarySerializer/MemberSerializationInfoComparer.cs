using System.Collections.Generic;

namespace BinarySerialization
{
    internal class MemberSerializationInfoComparer : Comparer<MemberSerializationInfo>
    {
        public override int Compare(MemberSerializationInfo x, MemberSerializationInfo y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return 1;

            if (y == null)
                return -1;

            /* There is some indication that the MetadataToken 
             * can be used as an indicator of member declaration order */
            //if (x.SerializeAsAttribute == null && y.SerializeAsAttribute == null)
            //    return x.Member.MetadataToken.CompareTo(y.Member.MetadataToken);

            int xOrder = x.SerializeAsAttribute == null ? 0 : x.SerializeAsAttribute.Order;
            int yOrder = y.SerializeAsAttribute == null ? 0 : y.SerializeAsAttribute.Order;

            return xOrder.CompareTo(yOrder);
        }
    }
}
