using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAlertState : SoldierState
{
    private float _lastShotFiredTime;
    private Vector3 _lastPlayerPos;

    public SoldierAlertState(GameObject owner) : base(owner)
    {
    }

    public override void Enter()
    {
        _lastShotFiredTime = 100.0f;
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        _lastShotFiredTime += Time.deltaTime;

        if (_aiScript.HasLOS("Player"))
        {
            _lastPlayerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

            _aiScript.Aim(_lastPlayerPos);

            if (_aiScript.HasClearShot())
            {
                if (_lastShotFiredTime > _aiScript.RateOfFire)
                {
                    //Debug.Log("Shoot");
                    _aiScript.Shoot();
                    _lastShotFiredTime = 0;
                }
            }
            else
            {
                if (!_aiScript.TowerGuard)
                    _aiScript.MoveTo(_lastPlayerPos);
            }
        }
        else
        {
            if (!_aiScript.TowerGuard)
            {
                _aiScript.MoveTo(_lastPlayerPos);
                if(_aiScript.HasArrived())
                {
                    _aiScript.SetState(SoldierStates.Caution);
                    _aiScript.Stop();
                }
            }
            else
            {
                _aiScript.SetState(SoldierStates.Caution);
            }
        }
    }
}
