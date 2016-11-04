namespace IptSimulator.Client.Model.Interfaces
{
    public interface ICompletionResult
    {
        int Priority { get; }
        string Text { get; }
    }
}