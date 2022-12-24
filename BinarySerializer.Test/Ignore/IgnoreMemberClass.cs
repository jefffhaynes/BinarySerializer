
namespace BinarySerialization.Test.Ignore
{
    [IgnoreMember(nameof(IgnoreMe))]
    [IgnoreMember("invalid")]
    internal class IgnoreMemberClass
    {
        public string IgnoreMe { get; set; }
    }
}
