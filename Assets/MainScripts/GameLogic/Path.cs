using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

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

    public void Write(BinaryWriter bw)
    {
        bw.Write(Points.Count);
        for (int c = 0; c < Points.Count; c++)
        {
            bw.Write(Points[c].Line);
            bw.Write(Points[c].Column);
        }
    }

    public void Read(BinaryReader br)
    {
        int length = br.ReadInt32();
            for (int c = 0; c < length;  c++)
        {
           int line = br.ReadInt32();
           int col = br.ReadInt32();
           Points.Add(new Point(line, col));
        }
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
