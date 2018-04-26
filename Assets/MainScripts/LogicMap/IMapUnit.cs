using UnityEngine;
using System.Collections;

public interface IMapUnit 
{
    IMapUnit LeftNeighbor { get; }
    IMapUnit RightNeighbor { get; }
    IMapUnit TopNeighbor { get; }
    IMapUnit BottomNeighbor { get; }

    ICluster Parent { get; }


    Point LocalClusterPosition { get; }
    Point GlobalClusterPosition { get; }

    float ClusterPassibilityFromCoef(Direction dir);
   // float ClusterPassabilityFromCoef(Dire)

}
