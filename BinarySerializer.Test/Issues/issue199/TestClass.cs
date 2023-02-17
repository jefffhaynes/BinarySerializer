using System.Collections.Generic;
using BinarySerialization;

public class Issue199TestsClass
{
    [FieldOrder(0)]
    [SerializeAs(SerializedType.TerminatedString, StringTerminator = (char)0x20)]
    public string DataAmount { get; set; }

    //数据列表
    [FieldOrder(1)]
    [FieldCount(nameof(DataAmount), ConverterType = typeof(HexStringToIntConvert))]
    public List<DistData> DistDatas { get; set; }

    public class DistData
    {
        [SerializeAs(SerializedType.TerminatedString, StringTerminator = (char)0x20)]
        public string Data { get; set; }
    }
}

public class OutputChannelInt
{
    [FieldOrder(0)]
    [SerializeAs(SerializedType.TerminatedString, StringTerminator = (char)0x20, ConverterType = typeof(HexStringToIntConvert))]
    public int DataAmount { get; set; }

    //数据列表
    [FieldOrder(1)]
    [FieldCount(nameof(DataAmount))]
    public List<DistDataInt> DistDatas { get; set; }

    public class DistDataInt
    {
        [SerializeAs(SerializedType.TerminatedString, StringTerminator = (char)0x20, ConverterType = typeof(HexStringToIntConvert))]
        public int Data { get; set; }
    }
}

class HexStringToIntConvert : IValueConverter
{
    // Read
    public object Convert(object value, object parameter, BinarySerializationContext context)
    {
        return (System.Convert.ToInt32((string)value, 16));
    }

    //Write
    public object ConvertBack(object value, object parameter, BinarySerializationContext context)
    {
        return value switch
        {
            int intValue => intValue.ToString("X"),
            long longValue => longValue.ToString("X"),
            _ => (value)
        };
    }
}