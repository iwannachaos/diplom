using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnpassibleBarrier : Barrier
{
    public UnpassibleBarrier() : base()
    {

    }


    public override void Interaction(Unit unit, float workTime)
    {
        throw new System.NotImplementedException();
    }
}
