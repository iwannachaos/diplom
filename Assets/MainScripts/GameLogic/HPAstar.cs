using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HPAstar
{
    public Unit Unit { get; private set; }

    public Point CurrentFinish { get; set; }

    public Path GridPath { get { return PathParts[PathParts.Count - 1]; } }
    public int GridCurrent
    {
        get { return currentPathPoint[currentPathPoint.Count - 1]; }
        set { currentPathPoint[currentPathPoint.Count - 1] = value; }
    }


    public List<Path> PathParts { get; private set; }

    public List<int> currentPathPoint;

    public void Initialize(Point finish)
    {
        PathParts = new List<Path>(GeneralGrid.Instance.DeepLevel);
        currentPathPoint = new List<int>(GeneralGrid.Instance.DeepLevel);
        ICluster curr = RunModel.Instance.CentralCluster;
        PathParts[0] = CalculatePathPart(finish, curr);
        currentPathPoint[0] = 0;
        var parent = curr;
        for (int i = 1; i < PathParts.Count - 1; i++)
        {
            curr = (ICluster)parent.GetChildCluster(PathParts[i - 1].Points[0]);
            var next = (ICluster)parent.GetChildCluster(PathParts[i - 1].Points[1]);
            currentPathPoint[i] = 0;
            var borders = curr.GetSharedBorder(next);
            PathParts[i] = GetTheShortestPath(curr, HelpLib.GetLocalPointPostionInCluster(Unit.Position, curr), borders);
            parent = curr;
        }

        AStar aStar = new AStar(curr.Children);
        Point f = RandomChooseClusterEntry((Grid)curr, (Grid)curr.Parent.GetChildCluster(PathParts[PathParts.Count - 2].Points[1]));
        PathParts[PathParts.Count - 1] = aStar.GetPath(new MapUnitPair(curr.GetChildCluster(HelpLib.GetLocalPointPostionInCluster(Unit.Position, curr)), 
                                         curr.GetChildCluster(f)));
    }

    Point RandomChooseClusterEntry(Grid startGrid, Grid secondGrid)
    {
        List<MapUnitPair> pairs = startGrid.GetSharedBorder(secondGrid);
        return pairs[Random.Range(0, pairs.Count)].FirstCell.LocalClusterPosition;     
    }

    public Path GetTheShortestPath(ICluster cluster, Point start, List<MapUnitPair> pairs)
    {
        int count = RunModel.Instance.CentralCluster.Width;
        Path shortest = null;
        for (int i =0; i< pairs.Count;i++)
        {
            Path path = ((Cluster)cluster).pathTables[start.Line,start.Column].Table[pairs[i].FirstCell.GlobalClusterPosition.Line,
                pairs[i].FirstCell.GlobalClusterPosition.Column];
            if (path.PathLength() < count)
            {
                count = path.PathLength();
                shortest = path;
            }
        }
        return shortest;
    }

    public Path CalculatePathPart(Point finish, ICluster cluster)
    {
        Point startCluster = HelpLib.GetGlobalClusterPostionByPoint(Unit.Position, cluster);
        Point finishCluster = HelpLib.GetGlobalClusterPostionByPoint(finish, cluster);
        PathParts[((Cluster)cluster).ClusterDeepLevel] = ((Cluster)cluster).pathTables[startCluster.Line, startCluster.Column].
            Table[finishCluster.Line, finishCluster.Column];
        var borderPairs = ((ICluster)(cluster.GetChildCluster(startCluster))).
            GetSharedBorder(((ICluster)(cluster.GetChildCluster(PathParts[0].Points[1]))));
        return GetTheShortestPath(cluster, startCluster, borderPairs);
    }
}
