using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Common.Interfaces;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
using NLog;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace IptSimulator.Client.Model.FsmGraph
{
    internal class FsmGraphManager : IFsmGraphManager
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly Random _rnd = new Random();

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

        public FsmGraphLogic Generate(ICollection<CiscoTcl.Model.FsmTransition> transitions)
        {
            
            var logic = new FsmGraphLogic()
            {
                Graph = GenerateGraph(transitions),
                DefaultLayoutAlgorithm = Layout,
                DefaultEdgeRoutingAlgorithm = EdgeRouting,
                EdgeCurvingEnabled = CurveEdges,
                DefaultOverlapRemovalAlgorithm = OverlapRemoval,
                DefaultOverlapRemovalAlgorithmParams = OverlapParameters,
                ParallelEdgeDistance = ParallelEdgeDistance,
            };

            return logic;
        }

        private FsmGraph GenerateGraph(ICollection<CiscoTcl.Model.FsmTransition> transitions)
        {
            var graph = new FsmGraph();

            _logger.Info("Generating graph vertices.");

            var vertices = GenerateVertices(transitions);
            
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

        private ICollection<FsmState> GenerateVertices(ICollection<CiscoTcl.Model.FsmTransition> transitions)
        {
            var allUniqueStates = transitions
                .Select(t => t.SourceState)
                .Union(transitions.Select(t => t.DetermineActualTargetState()))
                .Distinct()
                .Select(s => new FsmState(s, false, false))
                .ToList();

            allUniqueStates[0].IsInitial = true;
            allUniqueStates[_rnd.Next(0, allUniqueStates.Count)].IsCurrent = true;
            return allUniqueStates;
        }

        private void RaiseGraphPropertyChanged()
        {
            GraphPropertyChanged?.Invoke(this,EventArgs.Empty);
        }

        public event EventHandler GraphPropertyChanged;
    }
}