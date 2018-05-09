using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public Point Position { get; private set; }

    public float Productivity { get; private set; }

    public HPAstar HPAstar { get; private set; }

    public bool IsFinished { get { return Position == HPAstar.CurrentFinish; } }

    public Unit(Point pos)
    {
        Position = pos;
        HPAstar = new HPAstar();
    }

    public void MoveTo(Point newPos)
    {
        Position = newPos;
    }

    public void PathPartMoving()
    {
        if (Position == HPAstar.CurrentPath.Points[HPAstar.CurrentPath.Points.Count - 1])
        {
            HPAstar.UpdatePaths();
        }

        MoveTo(HPAstar.GridPath.Points[HPAstar.GridCurrent++]);
    }




}
