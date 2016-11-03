using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.Core
{
    public class CompletionResult : ICompletionResult
    {
        public CompletionResult(string text, string description)
        {
            Text = text;
            Description = description;
        }

        public string Text { get; }
        public string Description { get; }
    }
}
