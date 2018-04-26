using UnityEngine;
using System.Collections;
using System.IO;

public class Grid : BaseCluster
{
    public Grid(Cell[,] Cells, Point pos, BaseCluster parent)
         : base(Cells.GetLength(0), Cells.GetLength(1), pos, parent)
    {
        this.Cells = Cells;
    }

    public Cell[,] Cells { get; private set; }

    public override int ChildrenWidth
    {
        get
        {
            return Cells.GetLength(1);
        }
    }

    public override int ChildrenHeigth
    {
        get
        {
            return Cells.GetLength(0);
        }
    }

    public Grid(int width, int height, Point pos, BaseCluster parent)
    : base(width, height, pos, parent)
    {
        Cells = new Cell[height, width];  
    }


    public Cell GetCellByLocalPosition(Point position)
    {
        return Cells[position.Line, position.Column];
    }

    public Cell GetCellByLocalPosition(int line, int col)
    {
        //Debug.Log(line + ", " + col);
        return Cells[line, col];
    }


    public override void LinkClustersByEntries()
    {
        for (int j = 0; j < Height; j++)
        {
            Cell c = Cells[j, 0];
            if (c.LeftNeighbor!= null && c.Passible && ((Cell)c.LeftNeighbor).Passible)
                SelfLeftEntries.Add(c);
        }

        for (int j = 0; j < Height; j++)
        {
            Cell c = Cells[j, Cells.GetLength(0) - 1];
            if (c.RightNeighbor != null && c.Passible && ((Cell)c.RightNeighbor).Passible)
                SelfRightEntries.Add(c);
        }

        for (int j = 0; j < Width; j++)
        {
            Cell c = Cells[0, j];
            if (c.BottomNeighbor != null && c.Passible &&  ((Cell)c.BottomNeighbor).Passible)
                SelfBottomEntries.Add(c);
        }

        for (int j = 0; j < Width; j++)
        {
            Cell c = Cells[Cells.GetLength(1) - 1, j];
            if (c.TopNeighbor != null && c.Passible && ((Cell)c.TopNeighbor).Passible)
                SelfTopEntries.Add(c);
        }

        FileStream fs = new FileStream("Links.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);

        sw.WriteLine(this.GlobalClusterPosition + " - " + Width + "x" + Height);
        //for (int i = 0; i < SelfBottomEntries.Count; i++)
        //    sw.WriteLine("\t" + SelfBottomEntries[i].GlobalClusterPosition);
        sw.WriteLine("Bottom");
        sw.WriteLine("\t Count" + SelfBottomEntries.Count);
        sw.WriteLine(ClusterPassibilityFromCoef(Direction.Bottom).ToString());
        sw.WriteLine();
        //for (int i = 0; i < SelfLeftEntries.Count; i++)
        //    sw.WriteLine("\t" + SelfLeftEntries[i].GlobalClusterPosition);
        sw.WriteLine("Left");
        sw.WriteLine("\t Count" + SelfLeftEntries.Count);
        sw.WriteLine(ClusterPassibilityFromCoef(Direction.Left).ToString());
        sw.WriteLine();
        //for (int i = 0; i < SelfRightEntries.Count; i++)
        //    sw.WriteLine("\t" + SelfRightEntries[i].GlobalClusterPosition);
        sw.WriteLine("Right");
        sw.WriteLine("\t Count" + SelfRightEntries.Count);
        sw.WriteLine(ClusterPassibilityFromCoef(Direction.Right).ToString());
        sw.WriteLine();
        //for (int i = 0; i < SelfTopEntries.Count; i++)
        //    sw.WriteLine("\t" + SelfTopEntries[i].GlobalClusterPosition);
        sw.WriteLine("Top");
        sw.WriteLine("\t Count" + SelfTopEntries.Count);
        sw.WriteLine(ClusterPassibilityFromCoef(Direction.Top).ToString());
        sw.WriteLine();
        sw.Close();
    }


    override
    public void Build(int clusterW, int clusterH,  int cellW, int cellH) 
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                var gp = GlobalClusterPosition;
                //if (gp.Column * Width + j >= GeneralGrid.Instance.Cells.GetLength(1))
                //    Debug.Log(gp.Column + "," + j);
                Cells[i, j] = GeneralGrid.Instance.Cells[gp.Line + i, gp.Column + j];
                Cells[i, j].LocalClusterPosition = new Point(i, j);
                Cells[i, j].Parent = this;
            }
        }

        Debug.Log(LocalClusterPosition);
    }

    public override IMapUnit GetChildCluster(Point position)
    {
        return Cells[position.Line, position.Column];
    }

    public override IMapUnit GetChildCluster(int line, int col)
    {
        if (line < 0 || line >= Height || col < 0 || col >= Width)
            Debug.Log(line + "," + col);
        return Cells[line, col];
    }
}
