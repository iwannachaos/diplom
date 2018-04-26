using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pit : Barrier
{
    public int MaxPassTime { get; private set; }
    public int CurPassTime { get; private set; }
    public int PassPercentOpenning { get; private set; }

    public Pit() : base()
    {
        //MaxPassTime = maxTime;
        PassPercentOpenning = 0;
        CurPassTime = 0;
    }

    public override void Interaction(Unit unit, float workTime)
    {
        if (unit is Pitter && PassPercentOpenning < 100)
        {
            PassPercentOpenning += (int)(unit.Productivity * workTime);
        }
    }

}
