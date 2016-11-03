using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.Core
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
