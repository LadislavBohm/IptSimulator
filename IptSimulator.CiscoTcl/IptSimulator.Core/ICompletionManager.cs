using System.Collections.Generic;

namespace IptSimulator.Core
{
    public interface ICompletionManager
    {
        IEnumerable<ICompletionResult> GetCompletions(string wholeScript);
    }
}