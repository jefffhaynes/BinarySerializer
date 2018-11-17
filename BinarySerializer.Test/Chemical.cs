namespace BinarySerialization.Test
{
    public abstract class Chemical
    {
        protected Chemical(string formula)
        {
            Formula = formula;
        }

        [SerializeAs(SerializedType.NullTerminatedString)]
        public string Formula { get; set; }
    }
}