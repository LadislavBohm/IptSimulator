using IptSimulator.Client.Model.Interfaces;

namespace IptSimulator.Client.Model
{
    public class CompletionResult : ICompletionResult
    {
        public CompletionResult(string text, int priority)
        {
            Text = text;
            Priority = priority;
        }

        public string Text { get; }
        public int Priority { get; }
    }
}
