using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public abstract class BaseCluster : ICluster
{   
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Point LocalClusterPosition { get; private set; }
    public int ClusterDeepLevel { get; protected set; }

    public List<MapUnitPair> SelfLeftEntries { get; private set; }
    public List<MapUnitPair> SelfRightEntries { get; private set; }
    public List<MapUnitPair> SelfTopEntries { get; private set; }
    public List<MapUnitPair> SelfBottomEntries { get; private set; }
    public ICluster Parent { get; private set; }


    public BaseCluster(int width, int height, Point position, BaseCluster parent)
    {
        Width = width;
        Height = height;
        LocalClusterPosition = position;
        Parent = parent;

        SelfLeftEntries = new List<MapUnitPair>();
        SelfRightEntries = new List<MapUnitPair>();
        SelfTopEntries = new List<MapUnitPair>();
        SelfBottomEntries = new List<MapUnitPair>();
    }


    public IMapUnit LeftNeighbor
    {
        get
        {
            return (GlobalClusterPosition.Column - 1 >= 0) ? (LocalClusterPosition.Column !=0) ?
                Parent.GetChildCluster(LocalClusterPosition.Line, LocalClusterPosition.Column - 1) :
                ((ICluster)Parent.LeftNeighbor).GetChildCluster(LocalClusterPosition.Line, Parent.ChildrenWidth-1):
                null;
        }
    }
    public IMapUnit RightNeighbor
    {
        get
        {
            return (GlobalClusterPosition.Column + 1 + Width < GeneralGrid.Instance.Cells.GetLength(1)) ?
                (LocalClusterPosition.Column != Parent.ChildrenWidth-1) ?
                Parent.GetChildCluster(LocalClusterPosition.Line, LocalClusterPosition.Column + 1) :
                ((ICluster)Parent.RightNeighbor).GetChildCluster(LocalClusterPosition.Line, 0) :
                null;
        }
    }
    public IMapUnit TopNeighbor
    {
        get
        {
            return (GlobalClusterPosition.Line + 1 + Height < GeneralGrid.Instance.Cells.GetLength(0)) ?
                 (LocalClusterPosition.Line != Parent.ChildrenHeigth-1) ?
                Parent.GetChildCluster(LocalClusterPosition.Line+1, LocalClusterPosition.Column) :
                ((ICluster)Parent.TopNeighbor).GetChildCluster(0, LocalClusterPosition.Column) :
                null;
        }
    }
    public IMapUnit BottomNeighbor
    {
        get
        {
            return (GlobalClusterPosition.Line - 1 >= 0) ? (LocalClusterPosition.Line !=0) ?
                Parent.GetChildCluster(LocalClusterPosition.Line - 1, LocalClusterPosition.Column) :
                ((ICluster)Parent.BottomNeighbor).GetChildCluster(Parent.ChildrenHeigth-1, LocalClusterPosition.Column) :
                null;
        }
    }
   
    public Point GlobalClusterPosition
    {
        get
        {
            if (Parent == null)
                return LocalClusterPosition;

            var parGlPos = Parent.LocalClusterPosition;
            return new Point(Parent.Height * parGlPos.Line + LocalClusterPosition.Line * Height,
                    Parent.Width * parGlPos.Column + LocalClusterPosition.Column * Width);
        }
    }

    public abstract int ChildrenWidth { get; }
    
    public abstract int ChildrenHeigth { get; }

    abstract public IMapUnit[,] Children { get; }

    protected void SizeByChildClusters(int widthSeparationPartsCount, int heightSeparationPartsCount, out int width, out int height)
    {
        var ChildClusterWidth = Width / widthSeparationPartsCount;
        var ChildClusterHeight = Height / heightSeparationPartsCount;
        if (Width % ChildClusterWidth == 0)
            width = ChildClusterWidth;
        else
            width = ChildClusterWidth + 1;
        if (Height % ChildClusterHeight == 0)
            height =  ChildClusterHeight;
        else
            height = ChildClusterHeight + 1;
    }

    abstract public IMapUnit GetChildCluster(Point position);

    abstract public IMapUnit GetChildCluster(int line, int col);

    public abstract void Build(int clusterW, int clusterH, int cellW, int cellH, int deepLevel);

    public abstract void LinkClustersByEntries();

    public float ClusterPassibilityFromCoef(Direction dir)
    {
        switch (dir)
        {
            case Direction.Top:
                return (float)ChildrenWidth / SelfTopEntries.Count;
            case Direction.Left:
                return (float)ChildrenHeigth / SelfLeftEntries.Count;
            case Direction.Right:
                return (float)ChildrenHeigth / SelfRightEntries.Count;
            case Direction.Bottom:
                return (float)ChildrenWidth / SelfBottomEntries.Count;
        }
        return float.PositiveInfinity;
    }

    public List<MapUnitPair> GetSharedBorder(ICluster other)
    {
        if (other == LeftNeighbor)
            return SelfLeftEntries;
        else if (other == RightNeighbor)
            return SelfRightEntries;
        else if (other == BottomNeighbor)
            return SelfBottomEntries;
        else if (other == TopNeighbor)
            return SelfTopEntries;
        return new List<MapUnitPair>();
    }

    virtual public void ComputePathTable()
    {
    }

    virtual public void Write(BinaryWriter bw)
    {
    }

    virtual public void Read(BinaryReader br)
    {
    }

 
}
