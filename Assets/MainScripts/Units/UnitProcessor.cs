using UnityEngine;
using System.Collections;

public class UnitProcessor : MonoBehaviour
{
    public Unit Unit { get; set; }
    public Point Finish;
    
    // Use this for initialization
    void Start()
    {
        Finish = new Point(900, 900);
        Unit = new Unit(new Point(Mathf.RoundToInt(transform.position.z), Mathf.RoundToInt(transform.position.x)));
        Unit.HPAstar.Initialize(Finish);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Unit.IsFinished)
        {
            Unit.PathPartMoving(Unit.CurrentPath);
        }
    }
}
