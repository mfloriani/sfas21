using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierCautionState : SoldierState
{
    private Vector3 _destination;

    public SoldierCautionState(GameObject owner) : base(owner)
    {
    }

    public override void Enter()
    {
        _destination = _aiScript.GetDestination();
        _aiScript.MoveTo(_destination);
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        if (_aiScript.HasLOS("Player"))
        {
            _destination = GameObject.FindGameObjectWithTag("Player").transform.position;
            if (!_aiScript.TowerGuard)            
                _aiScript.MoveTo(_destination);

            if (_aiScript.HasClearShot())
                _aiScript.SetState(SoldierStates.Alert);
        }
        else
        {
            if (!_aiScript.TowerGuard)
            {
                _aiScript.MoveTo(_destination);
                if (_aiScript.HasArrived())
                {
                    _aiScript.SetState(SoldierStates.Idle);
                    _aiScript.Stop();
                }
            }
            else
            {
                _aiScript.SetState(SoldierStates.Idle);
            }
        }
    }
}
