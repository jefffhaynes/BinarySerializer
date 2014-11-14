using System;

namespace Iso9660
{
    [Flags]
    public enum RecordType : byte
    {
        File = 0,
        Directory = 1,
        AssociatedFile = 2,
        RecordFormat = 4,
        Permissions = 8,
    }
}
