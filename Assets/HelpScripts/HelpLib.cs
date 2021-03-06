﻿using UnityEngine;
using System.Collections;

public class HelpLib 
{
    public static  Point GetGlobalClusterPostionByPoint(Point point, ICluster clusterLevel)
    {
        int h = clusterLevel.Height / clusterLevel.ChildrenHeigth;
        int w = clusterLevel.Width / clusterLevel.ChildrenWidth;
        int line = point.Line / h;
        int col = point.Column / w;
        return new Point(line, col);
    }

    public static Point GetLocalPointPostionInCluster(Point point, ICluster cluster)
    {
        //int h = clusterLevel.Height / clusterLevel.ChildrenHeigth;
        //int w = clusterLevel.Width / clusterLevel.ChildrenWidth;
        int line = point.Line % clusterLevel.Height;
        int col = point.Column % clusterLevel.Width;
        return new Point(line, col);
    }

}

public class Point
{
    public int Line;
    public int Column;

    public Point(int i, int j)
    {
        Line = i;
        Column = j;
    }

    public override string ToString()
    {
        return Line + "," + Column +";";
    }
}

public enum CellType
{
    None = 255,
    Low = 100,
    High = 200,
    Unpass = 0,
    Bridge = 10,
    Pit = 20,
    Jungle = 30,
    Portal = 40,
    Start = 50

}

public enum Direction
{
    Top,
    Bottom,
    Left,
    Right
}

public class MapUnitPair
{
    public IMapUnit FirstCell { get; private set; }
    public IMapUnit SecondCell { get; private set; }

    public MapUnitPair(IMapUnit first, IMapUnit sec)
    {
        FirstCell = first;
        SecondCell = sec;
    }

    
}