using System.Collections.Generic;
using BinarySerialization;

namespace Iso9660
{
    public class Iso9660
    {
        [FieldOrder(0)]
        public PrimaryVolumeDescriptor PrimaryVolumeDescriptor { get; set; }

        [FieldOrder(1)]
        [FieldOffset("PrimaryVolumeDescriptor.PathTableOffset")]
        [FieldLength("PrimaryVolumeDescriptor.PathTableLength")]
        public List<PathTableRecord> PathTableRecords { get; set; }
    }
}
