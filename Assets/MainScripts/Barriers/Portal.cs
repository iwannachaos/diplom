using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : Barrier
{
    public int HumanStrengthMax { get; private set; }
    public int HumanStrengthCur { get; private set; }
    public int MaxGateTime { get; private set; }
    public int CurGateTime { get; private set; }
    public int GatePercentOpenning { get; private set; }


    public Portal() : base()
    {
        //HumanStrengthMax = maxStrength;
        //MaxGateTime = maxTime;
        HumanStrengthCur = 0;
        GatePercentOpenning = 0;
        CurGateTime = 0;
    }

    public override void Interaction(Unit unit, float workTime)
    {
        if (unit is Magician && GatePercentOpenning < 100)
        {
            GatePercentOpenning += (int)(unit.Productivity * workTime);
        }
    }
}
