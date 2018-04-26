using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Threading;
using System.IO;

public class RunModel : MonoBehaviour
{
    public Cluster CentralCluster { get; private set; }

    void BuildGrid(object cells)
    {
        GeneralGrid.Instance.ToGrid((CellType[,])cells);
    }


    public void Run()
    {
        BinaryReader br = new BinaryReader(new FileStream("map.dat", FileMode.OpenOrCreate));

        var height = br.ReadInt32();
        var width = br.ReadInt32();

        //Texture2D t = new Texture2D(width, height);
        CellType[,] cells = new CellType[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var ct = br.ReadInt32();
                cells[i, j] = (CellType)ct;
                
            }
        }
        br.Close();

        GeneralGrid gg = new GeneralGrid(width, height);

        Thread thr = new Thread(BuildGrid);
        thr.Start(cells);
        thr.Join();

        //GeneralGrid.Instance.ToGrid(cells);
        CentralCluster = new Cluster(width, height, new Point(0,0), null);
        CentralCluster.Build(10, 10, 1000, 1000);
        CentralCluster.LinkClustersByEntries();       
    }

    public void BuildPathTables()
    {
        CentralCluster.ComputePathTable();
        Debug.Log("123123123");
    }
}