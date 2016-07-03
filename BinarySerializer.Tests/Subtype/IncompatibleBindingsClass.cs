﻿namespace BinarySerialization.Test.Subtype
{
    public class IncompatibleBindingsClass
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        public byte Indicator2 { get; set; }

        [FieldOrder(2)]
        [Subtype("Indicator", 1, typeof(SubclassA))]
        [Subtype("Indicator2", 2, typeof(SubclassB))]
        public Superclass Superclass { get; set; }
    }
}
