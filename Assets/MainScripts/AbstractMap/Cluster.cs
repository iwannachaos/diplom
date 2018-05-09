using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Cluster : BaseCluster
{
    protected ICluster[,] children;

    override
    public IMapUnit[,] Children
    {
        get { return children; }
    }

    public override int ChildrenWidth
    {
        get
        {
            return children.GetLength(1);
        }
    }
    public override int ChildrenHeigth
    {
        get
        {
            return children.GetLength(0);
        }
    }
    
    public PathTable[,] pathTables;

    public Cluster(int width, int height, Point position, BaseCluster parent) 
        :base(width, height, position, parent)
    {
        
    }


    public override IMapUnit GetChildCluster(Point position)
    {
            if (position.Line >= 0 &&
               position.Line < children.GetLength(0) &&
               position.Column >= 0 &&
               position.Column < children.GetLength(1))
                return children[position.Line, position.Column];
            else return null;
    }

    public override IMapUnit GetChildCluster(int line, int col)
    {
        if (line >= 0 &&
           line < children.GetLength(0) &&
            col >= 0 &&
            col < children.GetLength(1))
            return children[line, col];
        else return null;
    }


    override
    public void Build(int clusterW, int clusterH, int cellW, int cellH, int deepLevel)
    {
        int width = cellW / clusterW;
        int height = cellH / clusterH;
        ClusterDeepLevel = deepLevel;
       // SizeByChildClusters(wSeparate, hSeparate, out width, out  height);
        children = new ICluster[clusterH, clusterW];

        if (clusterH < height
            && clusterW < width)
        {
            for (int i = 0; i < children.GetLength(0); ++i)
            {
                for (int j = 0; j < children.GetLength(1); ++j )
                {
                    var cluster = new Cluster(cellH / clusterH, cellW / clusterW, new Point(i, j), this);
                    children[i, j] = cluster;
                    cluster.Build(clusterW, clusterH, width, height, deepLevel+1);
                }
            }
        }
        else 
        {
            for (int i = 0; i < children.GetLength(0); ++i)
            {
                for (int j = 0; j < children.GetLength(1); ++j)
                {
                    var grid = new Grid(height, width, new Point(i, j), this);
                    children[i, j] = grid;
                    grid.Build(clusterW, clusterH, width, height, deepLevel+1);
                }
            }
        }
    }

    override
    public void LinkClustersByEntries()
    {
     
        for (int i = 0; i < children.GetLength(0); i++)
            for (int j = 0; j < children.GetLength(1); j++)
            {
                ICluster c = children[i, j];
                c.LinkClustersByEntries();
            }

        for (int j = 0; j < children.GetLength(0); j++)
        {          
            ICluster c = children[j, 0];
            if (c == null || c.SelfLeftEntries == null)
                Debug.Log(c);
            if (c.SelfLeftEntries.Count > 0)
                SelfLeftEntries.Add(new MapUnitPair(c,c.LeftNeighbor));
                
        }

        for (int j = 0; j < children.GetLength(0); j++)
        {
            ICluster c = children[j, children.GetLength(0)-1];
            if (c.SelfRightEntries.Count > 0)
                SelfRightEntries.Add(new MapUnitPair(c, c.RightNeighbor));
              
        }

        for (int j = 0; j < children.GetLength(1); j++)
        {
            ICluster c = children[0, j];
            if (c.SelfBottomEntries.Count > 0)
                SelfBottomEntries.Add(new MapUnitPair(c, c.BottomNeighbor));
        }

        for (int j = 0; j < children.GetLength(1); j++)
        {
            ICluster c = children[children.GetLength(1)-1, j];
            if (c.SelfTopEntries.Count > 0)
                SelfTopEntries.Add(new MapUnitPair(c, c.TopNeighbor));
        }

        #region logging
        //FileStream fs = new FileStream("Links.txt", FileMode.Append);
        //StreamWriter sw = new StreamWriter(fs);

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
    public void ComputePathTable()
    {
        AStar ast = new AStar(children);
        pathTables = ast.GetGlobalPathTable(this);
        for (int i = 0; i < ChildrenHeigth; i++)
            for (int j = 0; j < ChildrenWidth; j++)
                children[i, j].ComputePathTable();
    }

    override
    public void Write(BinaryWriter bw)
    {
        int height = ChildrenHeigth;
        int width = ChildrenWidth;
        bw.Write(height);
        bw.Write(width);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++) 
            {
                var cpt = pathTables[i, j];
                cpt.Write(bw);
            }
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                children[i, j].Write(bw);
            }
        }
    }

    override
    public void Read(BinaryReader br)
    {
        int height = br.ReadInt32();
        int width =  br.ReadInt32();
        pathTables = new PathTable[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                pathTables[i, j] = new PathTable();
                pathTables[i, j].Read(br);
            }
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                children[i, j].Read(br);
            }
        }
    }
}
