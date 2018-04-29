using UnityEngine;
using UnityEditor;
using System.IO;

public class PathTable 
{
    public Path[,] Table { get; private set; }

    public PathTable(int a, int b)
    {
        Table = new Path[a, b];
    }

    public PathTable()
    {
    }

    public void Write(BinaryWriter bw)
    {
        bw.Write(Table.GetLength(0));
        bw.Write(Table.GetLength(1));

        for (int a = 0; a < Table.GetLength(0); a++)
        {
            for (int b = 0; b < Table.GetLength(1); b++)
            {
                Table[a, b].Write(bw);
            }
        }
    }

    public void Read(BinaryReader br)
    {
        var height = br.ReadInt32();
        var width = br.ReadInt32();
        Table = new Path[height, width];
        for (int a = 0; a < height; a++)
        {
            for (int b = 0; b < width; b++)
            {
                Table[a, b] = new Path();
                Table[a, b].Read(br);
            }
        }

    }


    
}