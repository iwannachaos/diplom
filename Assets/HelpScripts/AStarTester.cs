//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;
//using System;

//public class AStarTester : MonoBehaviour
//{
//    [SerializeField]
//    int Width;

//    [SerializeField]
//    int Height;

//    [SerializeField]
//    int startLine;
//    [SerializeField]
//    int startColumn;

//    [SerializeField]
//    int finishLine;
//    [SerializeField]
//    int finishColumn;

//    IMapUnit start;
//    IMapUnit finish;

//    Grid grid;
//    public Cell[,] Cells { get; private set; }


//    // Use this for initialization
//    void Start()
//    {
        
//        Cells = new Cell[Height, Width];
      
//        grid = new Grid(Cells, new Point(0, 0), null);
//        for (int i = 0; i < Cells.GetLength(0); i++)
//            for (int j = 0; j < Cells.GetLength(1); j++)
//            {
//                Cells[i, j] = new Cell(i, j)
//                {
//                    Parent = grid,
//                    Passible = true
//                };
//            }

//        start = Cells[startLine, startColumn];
//        finish = Cells[finishLine, finishColumn];

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    public void Test()
//    {
//        AStar Apath = new AStar(Cells);

//       // List<Cell> path = Apath.GetPath(new CellPair(start, finish));



//        FileStream fs = new FileStream("path.txt", FileMode.Truncate);
//        StreamWriter sw = new StreamWriter(fs);

//        for (int i = 0; i < Cells.GetLength(0); i++)
//        {
//            for (int j = 0; j < Cells.GetLength(1); j++)
//            {
//                if (!path.Contains(Cells[i, j]))
//                {
//                    Debug.Log("#");
//                    sw.Write("#");
//                }
//                else if (Cells[i, j] == start)
//                {
//                    Debug.Log("S");
//                    sw.Write("S");
//                }

//                else if (Cells[i, j] == finish)
//                {
//                    Debug.Log("F");
//                    sw.Write("F");
//                }
//                else if (path.Contains(Cells[i, j]))
//                {
//                    Debug.Log("*");
//                    sw.Write("*");
//                }
//            }
//                sw.WriteLine();
//        }

//        sw.Close();
//    }
//}
