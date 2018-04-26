using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMap
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    int line;
    int column;
    
    public RaycastHit[] zZeroLine;
    public RaycastHit[] xZeroLine;
    public RaycastHit[] prevLine;
    public RaycastHit left;
    public float LimetedReliefAngle;
    public Point StartPoint;

   // public Texture2D t;

    public MapBuildState State { get; set; }

    public float Progress { get; set; }

    public  CellType[,] MapBarriers { get; private set; }

    public RawMap(int width,int height, float limetedReliefAngle)
    {
        Width = width;
        Height = height;
        zZeroLine = new RaycastHit[width];
        xZeroLine = new RaycastHit[height];
        prevLine = new RaycastHit[width];
        //t = new Texture2D(width, height);
        line = 0;
        column = 0;
        LimetedReliefAngle = limetedReliefAngle;
        MapBarriers = new CellType[height, width];
    }

    public void Iteration()
    {
        for(int i = 0; i < 2750; ++i)// Вычислительная сила процесса
            if (State != null)
                State.Iteration();
    }

    public void StartBuild()
    {
        if (State == null)
            State = new XPassState(this);
    }
}

public abstract class MapBuildState
{
    public RawMap Map { get; private set; }

    public MapBuildState(RawMap map)
    {
        Map = map;
    }

    abstract public void Iteration();
}

public class XPassState : MapBuildState
{
    RaycastHit[] xZeroLine;
    int column = 0;
    public XPassState(RawMap map) : base(map)
    {
        Debug.Log("toXPassState");
        this.xZeroLine = map.xZeroLine;
    }

    public override void Iteration()
    {
        RaycastHit hit;
        Physics.Raycast(new Vector3(0, 500, column), Vector3.down, out hit, 1000, LayerMask.GetMask("Barrier", "NatureBarrier"));
        xZeroLine[column++] = hit;
        if (column == xZeroLine.Length)
            Map.State = new ZPassState(Map);


        Map.Progress += .2f / xZeroLine.Length;
    }
}


public class ZPassState : MapBuildState
{
    RaycastHit[] zZeroLine;
    int line = 0;
    RaycastHit[] prevLine;

    public ZPassState(RawMap map) : base(map)
    {
        this.zZeroLine = map.zZeroLine;
        Debug.Log("toZPassState");
        this.prevLine = map.prevLine;
    }

    public override void Iteration()
    {
        RaycastHit hit;
        Physics.Raycast(new Vector3(line, 500, 0), Vector3.down, out hit, 1000, LayerMask.GetMask("Barrier", "NatureBarrier"));
        zZeroLine[line] = prevLine[line] = hit;
        ++line;
        if (line == zZeroLine.Length)
            Map.State = new InsideFillingState(Map);

        Map.Progress += .2f / zZeroLine.Length;
    }
}

public class InsideFillingState : MapBuildState
{
    public RaycastHit[] zZeroLine;
    public RaycastHit[] xZeroLine;
    public RaycastHit[] prevLine;
    RaycastHit left;
    int line = 1;
    int column = 1;

    public InsideFillingState(RawMap map) : base(map)
    {
        xZeroLine = map.xZeroLine;
        zZeroLine = map.zZeroLine;
        Debug.Log("toInsideFillingState");
        prevLine = map.prevLine;
        left = xZeroLine[1];
    }


    public override void Iteration()
    {
        RaycastHit hit;
        
        Physics.Raycast(new Vector3(line, 500, column), Vector3.down, out hit, 1000, LayerMask.GetMask("Barrier", "NatureBarrier",
            "StartPoint", "InteractionBarrier"));
        float absolutePointHeight = hit.point.y;
        var leftHeight = left.distance;
        var pointHeight = hit.distance;
        var downHeight = prevLine[line].distance;

        var toDownHeightRange = Mathf.Abs(pointHeight - downHeight);
        var toLeftHeightRange = Mathf.Abs(pointHeight - leftHeight);

        if ((!angleCheck(toDownHeightRange) &&
            hit.collider.gameObject.layer == LayerMask.NameToLayer("NatureBarrier") &&
            prevLine[line].collider.gameObject.layer == LayerMask.NameToLayer("NatureBarrier")) ||
            (!angleCheck(toLeftHeightRange)
            && (hit.collider.gameObject.layer == LayerMask.NameToLayer("NatureBarrier"))
            && left.collider.gameObject.layer == LayerMask.NameToLayer("NatureBarrier")))
        {
            if (absolutePointHeight >= 100)
            {
               // Map.t.SetPixel(line, column, Color.grey);
                Map.MapBarriers[line, column] = CellType.High;
               // Map.t.SetPixel(line, column - 1, Color.grey);
                Map.MapBarriers[line, column-1] = CellType.High;
               // Map.t.SetPixel(line - 1, column, Color.grey);
                Map.MapBarriers[line-1, column] = CellType.High;
            }
            else
            {
               // Map.t.SetPixel(line, column, Color.blue);
                Map.MapBarriers[line, column] = CellType.Low;
               // Map.t.SetPixel(line, column - 1, Color.blue);
                Map.MapBarriers[line, column-1] = CellType.Low;
               // Map.t.SetPixel(line - 1, column, Color.blue);
                Map.MapBarriers[line-1, column] = CellType.Low;
            }
            
        }
       
        //Для будущего валидатора: Нельзя ставить старт поинт и искуственные препятствия на нижнюю и левую границы (минимум 2 метра от краес карты)
        else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Barrier"))
        {
          //  Map.t.SetPixel(line, column, Color.black);
            Map.MapBarriers[line, column] = CellType.Unpass;
        }
        else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("StartPoint"))
        {
           // Map.t.SetPixel(line, column, Color.red);
            Map.MapBarriers[line, column] = CellType.Start;
            Map.StartPoint = new Point(line, column);

        }

        else if (hit.collider.gameObject.tag == "Bush")
        {
           // Map.t.SetPixel(line, column, Color.magenta);
            Map.MapBarriers[line, column] = CellType.Jungle;
        }

        else if (hit.collider.gameObject.tag == "Portal")
        {
            //Map.t.SetPixel(line, column, Color.cyan);
            Map.MapBarriers[line, column] = CellType.Portal;
        }

        else if (hit.collider.gameObject.tag == "Bridge")
        {
            //Map.t.SetPixel(line, column, Color.white);
            Map.MapBarriers[line, column] = CellType.Bridge;
        }

        else if (hit.collider.gameObject.tag == "Pit")
        {
            //Map.t.SetPixel(line, column, Color.yellow);
            Map.MapBarriers[line, column] = CellType.Pit;
        }


        else
        {
           // Map.t.SetPixel(line, column, Color.green);
            Map.MapBarriers[line, column] = CellType.None;
           // Map.t.SetPixel(line, column - 1, Color.green);
            Map.MapBarriers[line, column-1] = CellType.None;
           // Map.t.SetPixel(line - 1, column, Color.green);
            Map.MapBarriers[line-1, column] = CellType.None;
        }

        left = prevLine[line] = hit;

        column++;
        if (column == Map.Width)
        {
            line++;
            column = 1;
            if (line == Map.Height)
            {
                Filling00();
                Map.State = new QueueFillingState(Map);
                //Map.State = new QueuePaintState(Map,)
            }
        }

        Map.Progress += .2f / ((Map.Width - 1) * (Map.Height - 1));
    }

    private void Filling00()
    {
        if ((!angleCheck(Mathf.Abs(xZeroLine[1].distance - xZeroLine[0].distance)) &&
                           (xZeroLine[1].collider.gameObject.layer == LayerMask.NameToLayer("NatureBarrier") &&
                            xZeroLine[0].collider.gameObject.layer == LayerMask.NameToLayer("NatureBarrier"))) ||
                           (!angleCheck(Mathf.Abs(zZeroLine[1].distance - zZeroLine[0].distance)) &&
                           (zZeroLine[1].collider.gameObject.layer == LayerMask.NameToLayer("NatureBarrier") &&
                            zZeroLine[0].collider.gameObject.layer == LayerMask.NameToLayer("NatureBarrier"))))
        {
           // Map.t.SetPixel(0, 0, Color.grey);
            Map.MapBarriers[0, 0] = CellType.High;
        }
        else if (xZeroLine[0].collider.gameObject.layer == LayerMask.NameToLayer("Barrier") &&
                (zZeroLine[0].collider.gameObject.layer == LayerMask.NameToLayer("Barrier")))
        {
            //Map.t.SetPixel(0, 0, Color.black);
            Map.MapBarriers[0, 0] = CellType.Unpass;
        }
        else
        {
            //Map.t.SetPixel(0, 0, Color.green);
            Map.MapBarriers[0, 0] = CellType.None;
        }
    }

    bool angleCheck(float height)
    {
        return Map.LimetedReliefAngle >= Mathf.Atan(Mathf.Abs(height));
    }
}

class QueueFillingState : MapBuildState
{
    Queue<Point> queue = new Queue<Point>();
    bool[,] tArray;

    public QueueFillingState(RawMap map) : base(map)
    {
        tArray = new bool[Map.MapBarriers.GetLength(0), Map.MapBarriers.GetLength(1)];
        queue.Enqueue(Map.StartPoint);
        Debug.Log("toQueueFillingState");
    }

    public override void Iteration()
    {
        Point p1 = queue.Dequeue();
        tArray[p1.Line, p1.Column] = true;
        if (p1.Line + 1 < Map.MapBarriers.GetLength(0) &&
            (/*Map.t.GetPixel(p1.Line + 1, p1.Column) == Color.green) ||
            (Map.t.GetPixel(p1.Line + 1, p1.Column) == Color.red)) */ (Map.MapBarriers[p1.Line+1, p1.Column] == CellType.None ||
           Map.MapBarriers[p1.Line + 1, p1.Column ] == CellType.Start) &&
            !tArray[p1.Line + 1, p1.Column]))
        {
            queue.Enqueue(new Point(p1.Line + 1, p1.Column));
            tArray[p1.Line + 1, p1.Column] = true;
        }

        if ( p1.Line - 1 >= 0 && /*((Map.t.GetPixel(p1.Line - 1, p1.Column) == Color.green) ||
           (Map.t.GetPixel(p1.Line - 1, p1.Column) == Color.red)) */ (Map.MapBarriers[p1.Line - 1, p1.Column] == CellType.None ||
           Map.MapBarriers[p1.Line - 1, p1.Column ] == CellType.Start) &&
           !tArray[p1.Line - 1, p1.Column])
        {
            queue.Enqueue(new Point(p1.Line - 1, p1.Column));
            tArray[p1.Line - 1, p1.Column] = true;
        }

        if (p1.Column + 1 < Map.MapBarriers.GetLength(1) && /*((Map.t.GetPixel(p1.Line, p1.Column + 1) == Color.green) ||
           (Map.t.GetPixel(p1.Line, p1.Column + 1) == Color.red)) */ (Map.MapBarriers[p1.Line, p1.Column + 1] == CellType.None ||
           Map.MapBarriers[p1.Line, p1.Column + 1] == CellType.Start) &&
           !tArray[p1.Line, p1.Column + 1])
        {
            queue.Enqueue(new Point(p1.Line, p1.Column + 1));
            tArray[p1.Line, p1.Column + 1] = true;
        }

        if (p1.Column - 1 >= 0 && /*((Map.t.GetPixel(p1.Line, p1.Column - 1) == Color.green) ||
           (Map.t.GetPixel(p1.Line, p1.Column - 1) == Color.red)) */ (Map.MapBarriers[p1.Line, p1.Column - 1] == CellType.None  || 
           Map.MapBarriers[p1.Line, p1.Column - 1] == CellType.Start) &&
           !tArray[p1.Line, p1.Column - 1])
        {
            queue.Enqueue(new Point(p1.Line, p1.Column - 1));
            tArray[p1.Line, p1.Column - 1] = true;
        }

        if (queue.Count == 0)
        {
            Map.State = new QueuePaintState(Map, tArray);
            Map.Progress = 0.8f;
        }

        Map.Progress += .2f / (Map.Height*Map.Width);
    }
}

class QueuePaintState : MapBuildState
{
    bool[,] Mask;
    int line;
    int column;

    public QueuePaintState(RawMap map,  bool[,] mask) : base(map)
    {
        Mask = mask;
        Debug.Log("toQueuePaintState");
    }

    public override void Iteration()
    {
        if (!Mask[line, column] &&/* Map.t.GetPixel(line, column) == Color.green*/ Map.MapBarriers[line, column] == CellType.None)
        //{
        //    Map.t.SetPixel(line, column, Map.t.GetPixel(line, column-1));
        //}
        //Map.t.SetPixel(line, column, Color.grey);
        Map.MapBarriers[line, column] = CellType.High;
        column++;
        if (column == Map.Width)
        {
            line++;
            column = 1;
            if (line == Map.Height)
            {
                MapBuilder.ToDataFile(Map.MapBarriers);
                Map.Progress = 1;
                Map.State = null;
            }
        }
        Map.Progress += .2f / (Map.Height * Map.Width);
    }
}

