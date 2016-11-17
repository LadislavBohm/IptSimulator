using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Common.Models;

namespace IptSimulator.Client.Model.FsmGraph
{
    public class FsmState : VertexBase
    {
        public FsmState(string name, bool isInitial, bool isCurrent)
        {
            Name = name;
            IsInitial = isInitial;
            IsCurrent = isCurrent;
        }

        public string Name { get; set; }
        public bool IsInitial { get; set; }
        public bool IsCurrent { get; set; }

        #region Overrides

        public override bool Equals(object obj)
        {
            var state = obj as FsmState;
            if (state == null) return false;

            return state.Name == Name && state.IsCurrent == IsCurrent && state.IsInitial == IsInitial;
        }

        protected bool Equals(FsmState other)
        {
            return string.Equals(Name, other.Name) && IsInitial == other.IsInitial && IsCurrent == other.IsCurrent;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ IsInitial.GetHashCode();
                hashCode = (hashCode*397) ^ IsCurrent.GetHashCode();
                return hashCode;
            }
        }


        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(IsInitial)}: {IsInitial}, {nameof(IsCurrent)}: {IsCurrent}";
        }

        #endregion
    }
}
