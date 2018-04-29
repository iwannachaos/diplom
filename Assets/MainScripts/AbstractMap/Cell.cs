using UnityEngine;
using System.Collections;

public class Cell: IMapUnit
{
    public Point LocalClusterPosition { get;  set; }
    public ICluster Parent { get; set; }
    public Barrier Barrier { get; set; }
    public bool Passible { get { return Barrier == null || Barrier.Passiable; } }
    public float PassibleCoef { get { return Passible ? 1 : float.PositiveInfinity; } }

    public IMapUnit LeftNeighbor
    {
        get
        {
            return (GlobalClusterPosition.Column - 1 >= 0) ? (LocalClusterPosition.Column != 0) ?
                Parent.GetChildCluster(LocalClusterPosition.Line, LocalClusterPosition.Column - 1) :
                ((ICluster)Parent.LeftNeighbor).GetChildCluster(LocalClusterPosition.Line, Parent.ChildrenWidth - 1) :
                null;
        }
    }
    public IMapUnit RightNeighbor
    {
        get
        {
            if(GlobalClusterPosition.Column + 1 < GeneralGrid.Instance.Cells.GetLength(1) &&
                (LocalClusterPosition.Column == Parent.ChildrenWidth - 1)  &&
                Parent.RightNeighbor == null)
                Debug.Log(Parent.RightNeighbor);

            return (GlobalClusterPosition.Column + 1 < GeneralGrid.Instance.Cells.GetLength(1)) ?
                (LocalClusterPosition.Column != Parent.ChildrenWidth - 1) ?
                Parent.GetChildCluster(LocalClusterPosition.Line, LocalClusterPosition.Column + 1) :
                ((ICluster)Parent.RightNeighbor).GetChildCluster(LocalClusterPosition.Line, 0) :
                null;
        }
    }
    public IMapUnit TopNeighbor
    {
        get
        {
            return (GlobalClusterPosition.Line + 1 < GeneralGrid.Instance.Cells.GetLength(0)) ?
                 (LocalClusterPosition.Line != Parent.ChildrenHeigth - 1) ?
                Parent.GetChildCluster(LocalClusterPosition.Line + 1, LocalClusterPosition.Column) :
                ((ICluster)Parent.TopNeighbor).GetChildCluster(0, LocalClusterPosition.Column) :
                null;
        }
    }
    public IMapUnit BottomNeighbor
    {
        get
        {
            return (GlobalClusterPosition.Line - 1 >= 0) ? (LocalClusterPosition.Line != 0) ?
                Parent.GetChildCluster(LocalClusterPosition.Line - 1, LocalClusterPosition.Column) :
                ((ICluster)Parent.BottomNeighbor).GetChildCluster(Parent.ChildrenHeigth - 1, LocalClusterPosition.Column) :
                null;
        }
    }





    public Point GlobalClusterPosition
    {
        get
        {
            if (Parent == null)
                Debug.Log(Parent);
            return new Point( Parent.GlobalClusterPosition.Line + LocalClusterPosition.Line,
                             Parent.GlobalClusterPosition.Column + LocalClusterPosition.Column);
        }
    }

    public Cell(Point position)
    {
        LocalClusterPosition = position;
    }

    public Cell(int line, int Col)
    {
        LocalClusterPosition = new Point(line, Col);
    }

    public float ClusterPassibilityFromCoef(Direction dir)
    {
        if (Passible)
        {
            switch (dir)
            {
                case Direction.Top:
                    return ((Cell)TopNeighbor).PassibleCoef;
                case Direction.Left:
                    return ((Cell)LeftNeighbor).PassibleCoef;
                case Direction.Right:
                    return ((Cell)RightNeighbor).PassibleCoef;
                case Direction.Bottom:
                    return ((Cell)BottomNeighbor).PassibleCoef;
            }
        }
        return float.PositiveInfinity;
    }
}
