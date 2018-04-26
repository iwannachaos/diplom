using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Path 
{
    public List<Point> Points { get; private set; }

    public Path(List<Point> points)
    {
        Points = points;
    }

    public Path()
    {
        Points = new List<Point>();
    }

    public void AddPoint(Point point)
    {
        Points.Add(point);
    }

    public void AddPathWithStep(Path path, Point step)
    {
        Points.AddRange(path.Points);
        Points.Add(step);

    }
    public int PathLength()
    {
        return Points.Count;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        if (Points.Count > 0)
        {
            for (int q = 0; q < Points.Count - 1; q++)
            {
                sb.Append(Points[q] + ", ");
            }
            sb.Append(Points[Points.Count - 1]);
        }
        sb.Append("}");

        return sb.ToString();
    }

}
