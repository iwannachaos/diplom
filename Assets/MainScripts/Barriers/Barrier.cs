using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Barrier
{
    public List<Cell> Area { get; private set; }
    public bool Passiable { get; protected set; }


    abstract public void Interaction(Unit unit, float workTime);

    public Barrier()
    {
        Area = new List<Cell>();
        Passiable = false;
    }
    

}
