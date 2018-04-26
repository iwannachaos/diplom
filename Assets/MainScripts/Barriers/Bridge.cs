using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bridge : Barrier
{
    public int HumanStrengthMax { get; private set; }
    public int HumanStrengthCur { get; private set; }
    public int BridgeStrengthPercent { get; private set; }


    public Bridge() : base()
    {
        //HumanStrengthMax = maxStrength;
        HumanStrengthCur = 0;
    }


    public override void Interaction(Unit unit, float workTime)
    {
        if (unit is Bridger && BridgeStrengthPercent < 100)
        {
            HumanStrengthCur += (int)(unit.Productivity * workTime);
        }
    }
}
