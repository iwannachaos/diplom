using UnityEngine;
using UnityEditor;

public class PathTable 
{
    public Path[,] Table { get; private set; }

    public PathTable(int a, int b)
    {
        Table = new Path[a, b];
    }






    
}