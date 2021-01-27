using UnityEngine;
using System.Collections;

public class SoldierIdleState : SoldierState
{
    private float _totalStatetime;
    
    public SoldierIdleState(GameObject owner) : base(owner)
    {

    }

    public override void Enter()
    {
        //Debug.Log("SoldierIdleState Enter()");
        _totalStatetime = 0;
    }

    public override void Exit()
    {
        //Debug.Log("SoldierIdleState Exit()");
    }

    public override void Update()
    {
        _totalStatetime += Time.deltaTime;
        //Debug.Log("SoldierIdleState Update()");

        if (_aiScript.HasLOS("Player"))
        {
            if(!_aiScript.TowerGuard)
                _aiScript.MoveTo( GameObject.FindGameObjectWithTag("Player").transform.position );

            _aiScript.SetState(SoldierStates.Caution);
            return;
        }

        if (_totalStatetime > _aiScript.MaxTimeInIdle)
        {
            //Debug.Log("SoldierIdleState have to patrol");
            _aiScript.SetState(SoldierStates.Patrol);
        }
        
    }
}
