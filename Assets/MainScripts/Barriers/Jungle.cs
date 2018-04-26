using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jungle : Barrier
{
    public int MaxPassTime { get; private set; }
    public int CurPassTime { get; private set; }
    public int ClosingSpeed { get; private set; }
    public int PassPercentOpenning { get; private set; }


    public Jungle() : base()
    {
        //MaxPassTime = maxTime;
        //ClosingSpeed = closeSpeed;
        PassPercentOpenning = 0;
        CurPassTime = 0;
    }

    public override void Interaction(Unit unit, float workTime)
    {
        if (unit is Jungler && PassPercentOpenning < 100)
        {
            PassPercentOpenning += (int)(unit.Productivity * workTime);
        }
    }
}
