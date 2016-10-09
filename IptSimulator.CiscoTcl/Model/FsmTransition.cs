using System;

namespace IptSimulator.CiscoTcl.Model
{
    /// <summary>
    /// Defines a FSM state transition.
    /// When <see cref="Event"/> occurs and FSM is in <see cref="SourceState"/>
    /// FSM state will change to <see cref="TargetState"/>.
    /// </summary>
    public class FsmTransition
    {
        public FsmTransition(string sourceState, string @event, string targetState)
        {
            SourceState = sourceState;
            Event = @event;
            TargetState = targetState;
        }

        public string SourceState { get; }
        public string Event { get; }
        public string TargetState { get; }

        public static FsmTransition CreateInitial(string stateName)
        {
            if (string.IsNullOrWhiteSpace(stateName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(stateName));

            return new FsmTransition(string.Empty, string.Empty, stateName);
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
            return string.Equals(SourceState, other.SourceState) && string.Equals(Event, other.Event) && string.Equals(TargetState, other.TargetState);
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

        #endregion
    }
}