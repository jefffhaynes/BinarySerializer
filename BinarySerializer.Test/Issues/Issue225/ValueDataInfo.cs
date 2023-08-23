using System;

namespace BinarySerialization.Test.Issues.Issue225
{
    public class ValueDataInfo
    {
        [FieldOrder(0)]
        public ValueBlockType BlockType { get; set; } = ValueBlockType.PlainValue;

        [FieldOrder(1)]
        public UInt32 ParameterId { get; set; }

        [FieldOrder(2)]
        [FieldLength(8)]
        public string Name { get; set; }

        [FieldOrder(3)]
        public UInt32 NrValues { get; set; }

        [FieldOrder(4)]
        public ValueDataType DataTypeId { get; set; } = ValueDataType.Datatype_Invalid;

        [FieldOrder(5)]
        [SerializeWhen(nameof(BlockType), ValueBlockType.PlainValue)]
        [Subtype("DataTypeId", ValueDataType.Datatype_Double, typeof(DoublePlainValuesDataBody))]
        [Subtype("DataTypeId", ValueDataType.Datatype_Float, typeof(FloatPlainValuesDataBody))]
        [Subtype("DataTypeId", ValueDataType.Datatype_Int16, typeof(Int16PlainValuesDataBody))]
        [Subtype("DataTypeId", ValueDataType.Datatype_Int32, typeof(Int32PlainValuesDataBody))]
        [SubtypeDefault(typeof(EmptyPlainValueDataBlock))]
        public PlainValueDataBlock Block { get; set; }

        [FieldOrder(6)]
        [SerializeWhen(nameof(BlockType), ValueBlockType.ValueWithDescriptor)]
        [Subtype("DataTypeId", ValueDataType.Datatype_Double, typeof(DoubleValuesWithDescriptorDataBody))]
        [Subtype("DataTypeId", ValueDataType.Datatype_Float, typeof(FloatValuesWithDescriptorDataBody))]
        [Subtype("DataTypeId", ValueDataType.Datatype_Int16, typeof(Int16ValuesWithDescriptorDataBody))]
        [Subtype("DataTypeId", ValueDataType.Datatype_Int32, typeof(Int32ValuesWithDescriptorDataBody))]
        [SubtypeDefault(typeof(EmptyDescriptorDataBlock))]
        public ValueWithDescriptorDataBlock DescriptorBlock { get; set; }
    }
}