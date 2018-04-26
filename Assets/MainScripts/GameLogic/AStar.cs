using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;

public class AStar
{
    public IMapUnit[,] Cells { get; private set; }
    private Point[,] pathPoints { get; set; }
    private List<IMapUnit> WaitingCells { get; set; }
    private List<IMapUnit> CheckingCells { get; set; }
    private float[,] pathLenthToHereVar { get; set; }
    private List<IMapUnit> path;
    private bool finished;
    private IMapUnit finish;

    // Use this for initialization
    public AStar(IMapUnit[,] cells)
    {
        finished = false;
        Cells = cells;
        WaitingCells = new List<IMapUnit>();
        CheckingCells = new List<IMapUnit>();
        pathLenthToHereVar = new float[Cells.GetLength(0), Cells.GetLength(1)];
        for (int i = 0; i < Cells.GetLength(0); i++)
            for (int j = 0; j < Cells.GetLength(1); j++)
            {
                //Cells[i, j] = new BaseCluster(i, j);
                setPathLengthToHere(Cells[i, j], -1);
            }
        path = new List<IMapUnit>();
        pathPoints = new Point[Cells.GetLength(0), Cells.GetLength(1)];
    }

    public void GetPathMask(object startFinish)
    {
        IMapUnit start = ((CellPair)startFinish).FirstCell;
        finish = ((CellPair)startFinish).SecondCell;
        pathLenthToHereVar[start.LocalClusterPosition.Line, start.LocalClusterPosition.Column] = 0;
        CheckingCells.Add(start);
        while (!finished)
        {
            GetPathIteration();
        }

        IMapUnit current = finish;
        path.Add(current);

        while (current != start)
        {
            var p = pathPoints[current.LocalClusterPosition.Line, current.LocalClusterPosition.Column];
            current = Cells[p.Line, p.Column];
            path.Add(current);
        }

        path.Reverse();
    }

     public List<IMapUnit> GetPath(CellPair startFinish)
    {
        Thread thr = new Thread(GetPathMask);
        thr.Start(startFinish);
        thr.Join();
        return path;
    }


    public PathTable[,] GetGlobalPathTable(ICluster cluster)
    {
        PathTable[,] pt = new PathTable[cluster.ChildrenHeigth, cluster.ChildrenWidth];
        float start = Time.realtimeSinceStartup;
        for (int i = 0; i < pt.GetLength(0); i++)
        {
            for (int j = 0; j < pt.GetLength(1); j++)
            {
                pt[i, j] = GetPathTable(cluster.GetChildCluster(i, j), cluster);

            }
        }
        float end = Time.realtimeSinceStartup;
        Debug.Log(end - start);
        return pt;
    }

    public PathTable GetPathTable(IMapUnit start, ICluster cluster)
    {
        PathTable pt = new PathTable(cluster.ChildrenHeigth, cluster.ChildrenWidth);
        CheckingCells.Add(start);
        pt.Table[start.LocalClusterPosition.Line, start.LocalClusterPosition.Column] = new Path();
        pt.Table[start.LocalClusterPosition.Line, start.LocalClusterPosition.Column].AddPoint(start.LocalClusterPosition);
        while (CheckingCells.Count != 0)
        {
            GetPathTableIteration();
        }
        for (int i = 0; i < Cells.GetLength(0); i++)
        {
            for (int j = 0; j < Cells.GetLength(1); j++)
            {
                if(pt.Table[i,j] == null)
                    PlaceTable(Cells[i,j], pt);
            }
        }

        FileStream fs = new FileStream("PT.txt", FileMode.Truncate);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < pt.Table.GetLength(0); i++)
        {
            for (int j = 0; j < pt.Table.GetLength(1); j++)
            {
                sw.Write(pt.Table[i, j].ToString());
            }
            sw.WriteLine();
        }

        sw.Close();
        return pt;
    }

    private void PlaceTable(IMapUnit current, PathTable pt)
    {
        var p = pathPoints[current.LocalClusterPosition.Line, current.LocalClusterPosition.Column];
        pt.Table[current.LocalClusterPosition.Line, current.LocalClusterPosition.Column] = new Path();

        if (p != null)
        {
            if (pt.Table[p.Line, p.Column] == null)
                PlaceTable(Cells[p.Line, p.Column], pt);

            pt.Table[current.LocalClusterPosition.Line,
                   current.LocalClusterPosition.Column].AddPathWithStep(pt.Table[p.Line, p.Column], current.LocalClusterPosition);
        }

    }

    private void GetPathIteration()
    {
        for (int i = 0; i < CheckingCells.Count; i++)
        {
            IMapUnit ln = CheckingCells[i].LeftNeighbor;
            IMapUnit rn = CheckingCells[i].RightNeighbor;
            IMapUnit bn = CheckingCells[i].BottomNeighbor;
            IMapUnit tn = CheckingCells[i].TopNeighbor;

            if (ln == finish)
            {
                finished = true;
            }

            if (ln!=null 
                && !float.IsInfinity(ln.ClusterPassibilityFromCoef(Direction.Right)) 
                && (getPathLengthToHere(ln) == -1 || 
                    getPathLengthToHere(ln) > getPathLengthToHere(CheckingCells[i]) +
                     CheckingCells[i].ClusterPassibilityFromCoef(Direction.Left)))
            {
               
                if(getPathLengthToHere(ln) == -1)
                    WaitingCells.Add(ln);
                setPathLengthToHere(ln, getPathLengthToHere(CheckingCells[i]) +
                     CheckingCells[i].ClusterPassibilityFromCoef(Direction.Left));
                pathPoints[ln.LocalClusterPosition.Line, ln.LocalClusterPosition.Column] = 
                    CheckingCells[i].LocalClusterPosition;
            }           

            if (tn == finish)
            {
                finished = true;
            }

            if (tn != null
                && !float.IsInfinity(tn.ClusterPassibilityFromCoef(Direction.Bottom))
                && (getPathLengthToHere(tn) == -1
                || getPathLengthToHere(tn) > 
                getPathLengthToHere(CheckingCells[i]) +
                CheckingCells[i].ClusterPassibilityFromCoef(Direction.Top)))
            {
               
                if(getPathLengthToHere(tn) == -1)
                    WaitingCells.Add(tn);
                setPathLengthToHere(tn, getPathLengthToHere(CheckingCells[i]) +
                     CheckingCells[i].ClusterPassibilityFromCoef(Direction.Top));
                pathPoints[tn.LocalClusterPosition.Line, tn.LocalClusterPosition.Column] = 
                    CheckingCells[i].LocalClusterPosition;
            }


            if (bn == finish)
            {
                finished = true;
            }
        
            if (bn != null
                && !float.IsInfinity(bn.ClusterPassibilityFromCoef(Direction.Top))
                && (getPathLengthToHere(bn) == -1 
                || getPathLengthToHere(bn) > getPathLengthToHere(CheckingCells[i]) +
                 CheckingCells[i].ClusterPassibilityFromCoef(Direction.Bottom)))
            {
               
                if(getPathLengthToHere(bn) == -1)
                    WaitingCells.Add(bn);
                setPathLengthToHere(bn, getPathLengthToHere(CheckingCells[i]) +
                     CheckingCells[i].ClusterPassibilityFromCoef(Direction.Bottom));
                pathPoints[bn.LocalClusterPosition.Line, bn.LocalClusterPosition.Column] =
                    CheckingCells[i].LocalClusterPosition;
            }

            if (rn == finish)
            {
                finished = true;
            }

            if (rn != null
                && !float.IsInfinity(rn.ClusterPassibilityFromCoef(Direction.Left))
                && (getPathLengthToHere(rn) == -1
                || getPathLengthToHere(rn) > getPathLengthToHere(CheckingCells[i]) +
                 CheckingCells[i].ClusterPassibilityFromCoef(Direction.Right)))
            {
                
                if(getPathLengthToHere(rn) == -1)
                    WaitingCells.Add(rn);
                setPathLengthToHere(rn, getPathLengthToHere(CheckingCells[i]) +
                     CheckingCells[i].ClusterPassibilityFromCoef(Direction.Right));
                pathPoints[rn.LocalClusterPosition.Line, rn.LocalClusterPosition.Column] =
                    CheckingCells[i].LocalClusterPosition;
            }
        }

        //FileStream fs = new FileStream("WCells.txt", FileMode.Append);
        //StreamWriter sw = new StreamWriter(fs);
        //sw.WriteLine(WaitingCells.Count);        
        //sw.Close();

        CheckingCells.Clear();
        var tmp = CheckingCells;
        CheckingCells = WaitingCells;
        WaitingCells = tmp;


    }


    private void GetPathTableIteration()
    {
        for (int i = 0; i < CheckingCells.Count; i++)
        {
            IMapUnit ln = CheckingCells[i].LeftNeighbor;
            IMapUnit rn = CheckingCells[i].RightNeighbor;
            IMapUnit bn = CheckingCells[i].BottomNeighbor;
            IMapUnit tn = CheckingCells[i].TopNeighbor;

            if (ln != null
                && !float.IsInfinity(ln.ClusterPassibilityFromCoef(Direction.Right))
                && (getPathLengthToHere(ln) == -1 ||
                    getPathLengthToHere(ln) > getPathLengthToHere(CheckingCells[i]) + 
                    CheckingCells[i].ClusterPassibilityFromCoef(Direction.Left)))
            {

                if (getPathLengthToHere(ln) == -1)
                    WaitingCells.Add(ln);
                setPathLengthToHere(ln, getPathLengthToHere(CheckingCells[i]) +
                     CheckingCells[i].ClusterPassibilityFromCoef(Direction.Left));
                pathPoints[ln.LocalClusterPosition.Line, ln.LocalClusterPosition.Column] =
                    CheckingCells[i].LocalClusterPosition;
            }

            if (tn != null
                && !float.IsInfinity(tn.ClusterPassibilityFromCoef(Direction.Bottom))
                && (getPathLengthToHere(tn) == -1
                || getPathLengthToHere(tn) >
                getPathLengthToHere(CheckingCells[i]) + 
                CheckingCells[i].ClusterPassibilityFromCoef(Direction.Top)))
            {

                if (getPathLengthToHere(tn) == -1)
                    WaitingCells.Add(tn);
                setPathLengthToHere(tn, getPathLengthToHere(CheckingCells[i]) +
                    CheckingCells[i].ClusterPassibilityFromCoef(Direction.Top));
                pathPoints[tn.LocalClusterPosition.Line, tn.LocalClusterPosition.Column] =
                    CheckingCells[i].LocalClusterPosition;
            }


            if (bn != null
                && !float.IsInfinity(bn.ClusterPassibilityFromCoef(Direction.Top))
                && (getPathLengthToHere(bn) == -1
                || getPathLengthToHere(bn) > getPathLengthToHere(CheckingCells[i]) +
                 CheckingCells[i].ClusterPassibilityFromCoef(Direction.Bottom)))
            {

                if (getPathLengthToHere(bn) == -1)
                    WaitingCells.Add(bn);
                setPathLengthToHere(bn, getPathLengthToHere(CheckingCells[i]) + 
                    CheckingCells[i].ClusterPassibilityFromCoef(Direction.Top));
                pathPoints[bn.LocalClusterPosition.Line, bn.LocalClusterPosition.Column] = 
                    CheckingCells[i].LocalClusterPosition;
            }


            if (rn != null
                && !float.IsInfinity(rn.ClusterPassibilityFromCoef(Direction.Left))
                && (getPathLengthToHere(rn) == -1
                || getPathLengthToHere(rn) > getPathLengthToHere(CheckingCells[i]) +
                 CheckingCells[i].ClusterPassibilityFromCoef(Direction.Right)))
            {

                if (getPathLengthToHere(rn) == -1)
                    WaitingCells.Add(rn);
                setPathLengthToHere(rn, getPathLengthToHere(CheckingCells[i]) +
                     CheckingCells[i].ClusterPassibilityFromCoef(Direction.Right));
                pathPoints[rn.LocalClusterPosition.Line, rn.LocalClusterPosition.Column] = 
                    CheckingCells[i].LocalClusterPosition;
            }
        }

        CheckingCells.Clear();
        var tmp = CheckingCells;
        CheckingCells = WaitingCells;
        WaitingCells = tmp;

    }

    private float getPathLengthToHere(IMapUnit here)
    {   
        return pathLenthToHereVar[here.LocalClusterPosition.Line, here.LocalClusterPosition.Column];
    }

    private void setPathLengthToHere(IMapUnit here, float path)
    {
        if (here.LocalClusterPosition.Line >= here.Parent.ChildrenHeigth
            || here.LocalClusterPosition.Line < 0
            || here.LocalClusterPosition.Column >= here.Parent.ChildrenWidth
            || here.LocalClusterPosition.Column < 0)
            Debug.Log(here.LocalClusterPosition.ToString());
        pathLenthToHereVar[here.LocalClusterPosition.Line, here.LocalClusterPosition.Column] = path;
    }

}

public class CellPair
{
    public IMapUnit FirstCell { get; private set; }
    public IMapUnit SecondCell { get; private set; }

    public CellPair(IMapUnit first, IMapUnit sec)
    {
        FirstCell = first;
        SecondCell = sec;
    }
}