using System.Collections.Generic;

namespace IptSimulator.Client.Model.Interfaces
{
    public interface ICompletionManager
    {
        IEnumerable<ICompletionResult> GetCompletions(string wholeScript);
    }
}