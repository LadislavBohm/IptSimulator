using GraphX.PCL.Logic.Models;
using QuickGraph;

namespace IptSimulator.Client.Model.FsmGraph
{
    internal class FsmGraphLogic : GXLogicCore<FsmState, FsmTransition, BidirectionalGraph<FsmState, FsmTransition>> { }
}
