using UnityEngine;
using System.Collections;
using System.IO;

public class Grid : BaseCluster
{
    public Grid(Cell[,] Cells, Point pos, BaseCluster parent)
         : base(Cells.GetLength(0), Cells.GetLength(1), pos, parent)
    {
        this.cells = Cells;
    }

    private Cell[,] cells;

    public override int ChildrenWidth
    {
        get
        {
            return cells.GetLength(1);
        }
    }

    public override int ChildrenHeigth
    {
        get
        {
            return cells.GetLength(0);
        }
    }

    public Grid(int width, int height, Point pos, BaseCluster parent)
    : base(width, height, pos, parent)
    {
        cells = new Cell[height, width];  
    }

    override
    public IMapUnit[,] Children
    {
        get { return cells; }
    }

    public Cell GetCellByLocalPosition(Point position)
    {
        return cells[position.Line, position.Column];
    }

    public Cell GetCellByLocalPosition(int line, int col)
    {
        //Debug.Log(line + ", " + col);
        return cells[line, col];
    }


    public override void LinkClustersByEntries()
    {
        for (int j = 0; j < Height; j++)
        {
            Cell c = cells[j, 0];
            if (c.LeftNeighbor!= null && c.Passible && ((Cell)c.LeftNeighbor).Passible)
                SelfLeftEntries.Add(new MapUnitPair(c,c.LeftNeighbor));
        }

        for (int j = 0; j < Height; j++)
        {
            Cell c = cells[j, cells.GetLength(0) - 1];
            if (c.RightNeighbor != null && c.Passible && ((Cell)c.RightNeighbor).Passible)
                SelfRightEntries.Add(new MapUnitPair(c, c.RightNeighbor));
        }

        for (int j = 0; j < Width; j++)
        {
            Cell c = cells[0, j];
            if (c.BottomNeighbor != null && c.Passible &&  ((Cell)c.BottomNeighbor).Passible)
                SelfBottomEntries.Add(new MapUnitPair(c, c.BottomNeighbor));
        }

        for (int j = 0; j < Width; j++)
        {
            Cell c = cells[cells.GetLength(1) - 1, j];
            if (c.TopNeighbor != null && c.Passible && ((Cell)c.TopNeighbor).Passible)
                SelfTopEntries.Add(new MapUnitPair(c, c.TopNeighbor));
        }

        //FileStream fs = new FileStream("Links.txt", FileMode.Append);
        //StreamWriter sw = new StreamWriter(fs);

        #region logging

        //sw.WriteLine(this.GlobalClusterPosition + " - " + Width + "x" + Height);
        ////for (int i = 0; i < SelfBottomEntries.Count; i++)
        ////    sw.WriteLine("\t" + SelfBottomEntries[i].GlobalClusterPosition);
        //sw.WriteLine("Bottom");
        //sw.WriteLine("\t Count" + SelfBottomEntries.Count);
        //sw.WriteLine(ClusterPassibilityFromCoef(Direction.Bottom).ToString());
        //sw.WriteLine();
        ////for (int i = 0; i < SelfLeftEntries.Count; i++)
        ////    sw.WriteLine("\t" + SelfLeftEntries[i].GlobalClusterPosition);
        //sw.WriteLine("Left");
        //sw.WriteLine("\t Count" + SelfLeftEntries.Count);
        //sw.WriteLine(ClusterPassibilityFromCoef(Direction.Left).ToString());
        //sw.WriteLine();
        ////for (int i = 0; i < SelfRightEntries.Count; i++)
        ////    sw.WriteLine("\t" + SelfRightEntries[i].GlobalClusterPosition);
        //sw.WriteLine("Right");
        //sw.WriteLine("\t Count" + SelfRightEntries.Count);
        //sw.WriteLine(ClusterPassibilityFromCoef(Direction.Right).ToString());
        //sw.WriteLine();
        ////for (int i = 0; i < SelfTopEntries.Count; i++)
        ////    sw.WriteLine("\t" + SelfTopEntries[i].GlobalClusterPosition);
        //sw.WriteLine("Top");
        //sw.WriteLine("\t Count" + SelfTopEntries.Count);
        //sw.WriteLine(ClusterPassibilityFromCoef(Direction.Top).ToString());
        //sw.WriteLine();
        //sw.Close();
        #endregion
    }


    override
    public void Build(int clusterW, int clusterH,  int cellW, int cellH, int deepLevel) 
    {
        ClusterDeepLevel = deepLevel;
        GeneralGrid.Instance.DeepLevel = deepLevel;
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                var gp = GlobalClusterPosition;
                //if (gp.Column * Width + j >= GeneralGrid.Instance.Cells.GetLength(1))
                //    Debug.Log(gp.Column + "," + j);
                cells[i, j] = GeneralGrid.Instance.Cells[gp.Line + i, gp.Column + j];
                cells[i, j].LocalClusterPosition = new Point(i, j);
                cells[i, j].Parent = this;
            }
        }

        Debug.Log(LocalClusterPosition);
    }


    override
    public  IMapUnit GetChildCluster(Point position)
    {
        return cells[position.Line, position.Column];
    }

    override
    public  IMapUnit GetChildCluster(int line, int col)
    {
        if (line < 0 || line >= Height || col < 0 || col >= Width)
            Debug.Log(line + "," + col);
        return cells[line, col];
    }
}
