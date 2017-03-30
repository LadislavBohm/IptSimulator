using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Common.Interfaces;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
using IptSimulator.CiscoTcl.Events;
using IptSimulator.CiscoTcl.Utils;
using IptSimulator.Client.Annotations;
using NLog;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace IptSimulator.Client.Model.FsmGraph
{
    internal class FsmGraphManager : IFsmGraphManager
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private LayoutAlgorithmTypeEnum _layout = LayoutAlgorithmTypeEnum.KK;
        private EdgeRoutingAlgorithmTypeEnum _edgeRouting = EdgeRoutingAlgorithmTypeEnum.SimpleER;
        private OverlapRemovalAlgorithmTypeEnum _overlapRemoval = OverlapRemovalAlgorithmTypeEnum.FSA;
        private OverlapRemovalParameters _overlapParameters = new OverlapRemovalParameters()
        {
            HorizontalGap = 110,
            VerticalGap = 110
        };
        private bool _curveEdges = true;
        private bool _alignEdgeLabels = true;
        private int _parallelEdgeDistance = 20;
        
        #region Properties

        public bool AlignEdgeLabels
        {
            get { return _alignEdgeLabels; }
            set
            {
                _alignEdgeLabels = value; 
                RaiseGraphPropertyChanged();
            }
        }

        public bool CurveEdges
        {
            get { return _curveEdges; }
            set
            {
                _curveEdges = value; 
                RaiseGraphPropertyChanged();
            }
        }

        public LayoutAlgorithmTypeEnum Layout
        {
            get { return _layout; }
            set
            {
                _layout = value; 
                RaiseGraphPropertyChanged();
            }
        }

        public EdgeRoutingAlgorithmTypeEnum EdgeRouting
        {
            get { return _edgeRouting; }
            set
            {
                _edgeRouting = value; 
                RaiseGraphPropertyChanged();
            }
        }

        public OverlapRemovalAlgorithmTypeEnum OverlapRemoval
        {
            get { return _overlapRemoval; }
            set
            {
                _overlapRemoval = value; 
                RaiseGraphPropertyChanged();
            }
        }

        public int ParallelEdgeDistance
        {
            get { return _parallelEdgeDistance; }
            set
            {
                _parallelEdgeDistance = value; 
                RaiseGraphPropertyChanged();
            }
        }

        [ExpandableObject]
        public OverlapRemovalParameters OverlapParameters
        {
            get { return _overlapParameters; }
            set
            {
                _overlapParameters = value; 
                RaiseGraphPropertyChanged();
            }
        }

        #endregion

        public FsmGraphLogic Generate(string initialState, ICollection<CiscoTcl.Model.FsmTransition> transitions)
        {
            
            var logic = new FsmGraphLogic()
            {
                Graph = GenerateGraph(initialState, transitions),
                DefaultLayoutAlgorithm = Layout,
                DefaultEdgeRoutingAlgorithm = EdgeRouting,
                EdgeCurvingEnabled = CurveEdges,
                DefaultOverlapRemovalAlgorithm = OverlapRemoval,
                DefaultOverlapRemovalAlgorithmParams = OverlapParameters,
                ParallelEdgeDistance = ParallelEdgeDistance,
            };

            return logic;
        }

        private FsmGraph GenerateGraph(string initialState, ICollection<CiscoTcl.Model.FsmTransition> transitions)
        {
            var graph = new FsmGraph();

            _logger.Info("Generating graph vertices.");

            var vertices = GenerateVertices(initialState, transitions);
            
            _logger.Info($"Generated a total of {vertices.Count} vertices.");
            _logger.Debug($"Generated vertices are: [{string.Join("|", vertices)}]");

            graph.AddVertexRange(vertices);

            _logger.Info("Generating graph edges.");

            var edges = GenerateEdges(transitions, vertices);

            _logger.Info($"Generated a total of {edges.Count} edges.");
            _logger.Debug($"Generatd edges are: [{string.Join("|", edges)}]");

            graph.AddEdgeRange(edges);

            return graph;
        }

        private ICollection<FsmTransition> GenerateEdges(ICollection<CiscoTcl.Model.FsmTransition> transitions,
            ICollection<FsmState> vertices)
        {
            var result = new List<FsmTransition>();

            foreach (var transition in transitions)
            {
                var source = vertices.FirstOrDefault(s => transition.SourceState == s.Name);
                var target = vertices.FirstOrDefault(s => transition.DetermineActualTargetState() == s.Name);

                if (transition.SourceState == FsmSpecialStates.AnyState)
                {
                    result.AddRange(HandleAnyStateTransition(transition, transitions, vertices));
                    continue;
                }
                if (source == null)
                {
                    _logger.Warn($"Source state is NULL for transition: {transition}. Not adding to graph.");
                    continue;
                }
                if (target == null)
                {
                    _logger.Warn($"Target state is NULL for transition: {transition}. Not adding to graph.");
                    continue;
                }
                result.Add(new FsmTransition(source, target, transition.Event, transition.Procedure));
            }

            return result;
        }

        private ICollection<FsmState> GenerateVertices(string initialState, ICollection<CiscoTcl.Model.FsmTransition> transitions)
        {
            var allUniqueStates = transitions
                .Where(t => t.SourceState != FsmSpecialStates.AnyState)
                .Select(t => t.SourceState)
                .Union(transitions.Where(t => t.SourceState != FsmSpecialStates.AnyState).Select(t => t.DetermineActualTargetState()))
                .Distinct()
                .Select(s => new FsmState(s, s == initialState, s == initialState))
                .ToList();

            return allUniqueStates;
        }

        private IEnumerable<FsmTransition> HandleAnyStateTransition([NotNull] CiscoTcl.Model.FsmTransition anyStateTransition,
            [NotNull] ICollection<CiscoTcl.Model.FsmTransition> transitions, [NotNull] ICollection<FsmState> vertices)
        {
            if (anyStateTransition == null) throw new ArgumentNullException(nameof(anyStateTransition));
            if (transitions == null) throw new ArgumentNullException(nameof(transitions));
            if (vertices == null) throw new ArgumentNullException(nameof(vertices));
            if(anyStateTransition.SourceState != FsmSpecialStates.AnyState) 
                throw new ArgumentException(@"Transition's source state is not of any event type.", nameof(anyStateTransition));

            var result = new List<FsmTransition>();
            _logger.Debug($"Creating edges for following any_state transition: {anyStateTransition}");

            foreach (var transition in transitions)
            {
                if(transition.Equals(anyStateTransition)) continue;

                var source = vertices.FirstOrDefault(s => transition.DetermineActualTargetState() == s.Name);
                if (source == null)
                {
                    _logger.Warn($"Target state is NULL for transition: {transition}. Not adding to graph.");
                    continue;
                }
                var actualTargetState = anyStateTransition.DetermineActualTargetState();
                var anyStateTarget = actualTargetState == FsmSpecialStates.AnyState
                    ? source
                    : vertices.FirstOrDefault(s => actualTargetState == s.Name);
                
                if (anyStateTarget == null)
                {
                    _logger.Warn($"Source state is NULL for transition: {anyStateTransition}. Not adding to graph.");
                    return Enumerable.Empty<FsmTransition>();
                }

                result.Add(new FsmTransition(source, anyStateTarget, anyStateTransition.Event, anyStateTransition.Procedure));
            }

            _logger.Debug($"Created {result.Count} edges based on {CiscoTclEvents.AnyEvent} event transition.");
            return result;
        }

        private void RaiseGraphPropertyChanged()
        {
            GraphPropertyChanged?.Invoke(this,EventArgs.Empty);
        }

        public event EventHandler GraphPropertyChanged;
    }
}