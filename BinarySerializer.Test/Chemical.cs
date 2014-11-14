using BinarySerialization;

namespace BinarySerializer.Test
{
    abstract public class Chemical
    {
        protected Chemical(string formula)
        {
            Formula = formula;
        }

        [SerializeAs(SerializedType.NullTerminatedString)]
        public string Formula { get; set; }
    }
}
