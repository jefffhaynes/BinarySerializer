namespace BinarySerialization.Test.Issues.Issue39;

public class InputsStateFrameData
{
    public InputsStateFrameData(bool[] inputs)
    {
        Inputs = inputs;
    }

    [FieldCount(16)]
    public bool[] Inputs { get; }
}
