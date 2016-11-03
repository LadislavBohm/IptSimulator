namespace IptSimulator.Core
{
    public interface ICompletionResult
    {
        int Priority { get; }
        string Text { get; }
    }
}