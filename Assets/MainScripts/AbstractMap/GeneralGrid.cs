using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneralGrid
{
    public static GeneralGrid Instance { get; private set; }
    public Cell[,] Cells {get ;private set;}
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int DeepLevel { get;  set; }
   

    public GeneralGrid(int width, int height)
    {
        Instance = this;
        Width = width;
        Height = height;
        Cells = new Cell[Height, Width];
        DeepLevel = 0;
    }

    public void ToGrid(CellType[,] cells)
    {
        Queue<Point> queue = new Queue<Point>();
        int[,] indexes = new int[Height,Width];
        List<Barrier> barriers = new List<Barrier>();

        for (int i = 0; i < cells.GetLength(0); i++)
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                CellType c = cells[i, j];                
                if (c == CellType.None || c == CellType.Start)
                {                    
                    indexes[i, j] = -1;
                }
                else if (indexes[i, j] == 0)
                {
                    CellBarrierBuilder(c, barriers);
                    int barrierId = barriers.Count;
                    //if (barrierId == 0)
                    //    Debug.Log("Bad Color");
                    queue.Enqueue(new Point(i, j));
                    indexes[i, j] = -2;

                    int iters = 0;

                    while (queue.Count != 0)
                    {
                        ++iters;
                        
                        var p = queue.Dequeue();
                        indexes[p.Line, p.Column] = barrierId;
                        if (p.Line - 1 >= 0 && 
                            cells[p.Line - 1, p.Column] == c && 
                            indexes[p.Line - 1, p.Column] == 0)
                        {
                            queue.Enqueue(new Point(p.Line - 1, p.Column));
                            indexes[p.Line - 1, p.Column] = -2;
                        }

                        if (p.Line + 1 < Height &&
                           cells[p.Line + 1, p.Column] == c &&
                           indexes[p.Line + 1, p.Column] == 0)
                        {
                            queue.Enqueue(new Point(p.Line + 1, p.Column));
                            indexes[p.Line + 1, p.Column] = -2;
                        }

                        if (p.Column - 1 >= 0 &&
                           cells[p.Line, p.Column - 1] == c &&
                           indexes[p.Line, p.Column - 1] == 0)
                        {
                            queue.Enqueue(new Point(p.Line, p.Column - 1));
                            indexes[p.Line, p.Column - 1] = -2;
                        }

                        if (p.Column + 1 < Width &&
                           cells[p.Line, p.Column + 1] == c &&
                           indexes[p.Line, p.Column + 1] == 0)
                        {
                            queue.Enqueue(new Point(p.Line, p.Column + 1));
                            indexes[p.Line, p.Column + 1] = -2;
                        }
                    }

                }
            }

        int freeP = 0;
        for (int i = 0; i <cells.GetLength(0); i++)
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                Cells[i, j] = new Cell(new Point(i, j));
                if (indexes[i, j] > 0)
                {
                    barriers[indexes[i, j] - 1].Area.Add(Cells[i, j]);
                    Cells[i, j].Barrier = barriers[indexes[i, j] - 1];
                }
                if (Cells[i, j].Passible)
                    freeP++;
            }
        Debug.Log("Free points: " + freeP);
    }

    private void CellBarrierBuilder(CellType c, List<Barrier> barriers)
    {
        if (c == CellType.Unpass || c == CellType.High || c == CellType.Low)
            barriers.Add(new UnpassibleBarrier());
        else if (c == CellType.Jungle)
            barriers.Add(new Jungle());
        else if (c == CellType.Portal)
            barriers.Add(new Portal());
        else if (c == CellType.Bridge)
            barriers.Add(new Bridge());
        else if (c == CellType.Pit)
            barriers.Add(new Pit());

    }



}
