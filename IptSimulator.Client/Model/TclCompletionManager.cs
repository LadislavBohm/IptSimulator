using System;
using System.Collections.Generic;
using System.Linq;
using IptSimulator.CiscoTcl.Events;
using IptSimulator.CiscoTcl.Utils;
using IptSimulator.Client.Model.Interfaces;
using IptSimulator.Core;
using IptSimulator.Core.Tcl;
using NLog;

namespace IptSimulator.Client.Model
{
    public class TclCompletionManager : ICompletionManager
    {
        private static readonly HashSet<ICompletionResult> _tclEvents;
        private static readonly HashSet<ICompletionResult> _tclKeywords;
        private static readonly HashSet<ICompletionResult> _customTclCommands;
        private static readonly HashSet<ICompletionResult> _allCompletions;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        static TclCompletionManager()
        {
            try
            {
                Logger.Info("Initializing completion data...");

                _tclEvents = new HashSet<ICompletionResult>();
                foreach (var tclEvent in CiscoTclEvents.All)
                {
                    _tclEvents.Add(new CompletionResult(tclEvent, 3));
                }

                _tclKeywords = new HashSet<ICompletionResult>();
                foreach (var keyword in TclKeywords.All)
                {
                    _tclKeywords.Add(new CompletionResult(keyword, 1));
                }

                _customTclCommands = new HashSet<ICompletionResult>();
                foreach (var command in TclCommandProvider.GetCustomCommands())
                {
                    _customTclCommands.Add(new CompletionResult(command.Name, 1));
                }

                _allCompletions = new HashSet<ICompletionResult>();
                _allCompletions.UnionWith(_tclKeywords);
                _allCompletions.UnionWith(_tclEvents);
                _allCompletions.UnionWith(_customTclCommands);

                Logger.Info("Completion data were successfully initialized.");
            }
            catch(Exception e)
            {
                Logger.Error(e, "Error while intializing completion data.");
                throw;
            }
        }

        public IEnumerable<ICompletionResult> GetCompletions(string wholeScript)
        {
            try
            {
                var splitted = wholeScript
                    .Split(' ')
                    .Where(part => !part.Contains("#") && 
                                   !part.Contains("{") && 
                                   !part.Contains("}") && 
                                   !part.Contains("-") &&
                                   !part.Contains("(") &&
                                   !part.Contains(")") &&
                                   !part.Contains(",") &&
                                   !part.Contains(";"))
                    .Select(part => new CompletionResult(part, 1));

                var result = new List<ICompletionResult>();

                result.AddRange(_allCompletions);

                result.AddRange(splitted.Where(fromScript => !_allCompletions.Contains(fromScript)));

                return result;
                
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error while creating completion list. While script: {wholeScript}.");
                //never return null from this method
                return Enumerable.Empty<ICompletionResult>();
            }
        }
    }
}
