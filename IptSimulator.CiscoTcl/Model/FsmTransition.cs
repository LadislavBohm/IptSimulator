using System;
using System.Linq;
using IptSimulator.CiscoTcl.Utils;
using NLog;

namespace IptSimulator.CiscoTcl.Model
{
    /// <summary>
    /// Defines a FSM state transition.
    /// When <see cref="Event"/> occurs and FSM is in <see cref="SourceState"/>
    /// FSM state will change to <see cref="TargetState"/>.
    /// </summary>
    public class FsmTransition
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        public FsmTransition(string sourceState, string @event, string targetState, string procedure)
        {
            if (string.IsNullOrWhiteSpace(sourceState))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourceState));
            if (string.IsNullOrWhiteSpace(@event))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(@event));
            if (string.IsNullOrWhiteSpace(targetState))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetState));
            if (string.IsNullOrWhiteSpace(procedure))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(procedure));

            if (sourceState == FsmSpecialStates.SameState)
            {
                throw new ArgumentException($"Source state cannot be equal to {FsmSpecialStates.SameState}", nameof(sourceState));
            }
            if (targetState == FsmSpecialStates.AnyState)
            {
                throw new ArgumentException($"Target state cannot be equal to {FsmSpecialStates.AnyState}", nameof(targetState));
            }

            SourceState = sourceState;
            Event = @event;
            TargetState = targetState;
            Procedure = procedure;
        }

        public string SourceState { get; }
        public string Event { get; }
        public string TargetState { get; }
        public string Procedure { get; }

        public static FsmTransition CreateInitial(string stateName)
        {
            if (string.IsNullOrWhiteSpace(stateName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(stateName));

            return new FsmTransition(string.Empty, string.Empty, stateName,string.Empty);
        }

        /// <summary>
        /// Determines actual target state. If regular state is defined in <see cref="TargetState"/>, it just returns it.
        /// If special state is defined, it is calculated.
        /// </summary>
        /// <returns></returns>
        public string DetermineActualTargetState()
        {
            var specialState = FsmSpecialStates.All.FirstOrDefault(ss => ss == TargetState);

            if (string.IsNullOrWhiteSpace(specialState)) return TargetState;

            if (specialState == FsmSpecialStates.SameState)
            {
                _logger.Debug($"Target state is: {TargetState}, calculated actual target state from special state {specialState} is {SourceState}.");
                return SourceState;
            }

            _logger.Warn($"Unsupported TargetState special state: {specialState}, TargetState: {TargetState}");
            return TargetState;
        }

        #region Overrides

        public override bool Equals(object obj)
        {
            var transition = obj as FsmTransition;
            if (transition == null) return false;

            return transition.Equals(this);
        }

        private bool Equals(FsmTransition other)
        {
            return string.Equals(SourceState, other.SourceState) &&
                string.Equals(Event, other.Event) && 
                string.Equals(TargetState, other.TargetState) &&
                string.Equals(Procedure, other.Procedure);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SourceState?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Event?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (TargetState?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(SourceState)}: {SourceState}, {nameof(Event)}: {Event}, {nameof(TargetState)}: {TargetState}, {nameof(Procedure)}: {Procedure}";
        }

        #endregion


    }
}