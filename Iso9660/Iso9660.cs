using System.Collections.Generic;
using BinarySerialization;

namespace Iso9660
{
    public class Iso9660
    {
        public PrimaryVolumeDescriptor PrimaryVolumeDescriptor { get; set; }

        [FieldOffset("PrimaryVolumeDescriptor.PathTableOffset")]
        [FieldLength("PrimaryVolumeDescriptor.PathTableLength")]
        public List<PathTableRecord> PathTableRecords { get; set; }
    }
}
