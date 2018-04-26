using UnityEngine;
using System.Collections;

public class HelpLib 
{


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