namespace GraphGen
{
    internal interface IAttributeEvaluator<out TValue>
    {
        TValue Value { get; }

        TValue BoundValue { get; }
    }
}
