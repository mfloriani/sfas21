using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierPatrolState : SoldierState
{
    private float _totalStatetime;
    private int _currentWaypoint;
    private List<Transform> _waypoints;
    private float _minDistanceNextWaypoint = 0.5f;
    private bool _hasPatrolRoute = false;

    public SoldierPatrolState(GameObject owner) : base(owner)
    {

    }

    public override void Enter()
    {
        //Debug.Log("SoldierPatrolState Enter()");
        _totalStatetime = 0;

        // TODO: should start with the closest waypoint 
        _currentWaypoint = 0;
        _waypoints = new List<Transform>();

        if (_aiScript.PatrolRoute)
        {
            var wpList = _aiScript.PatrolRoute.GetComponentsInChildren<Transform>();
            _waypoints = new List<Transform>(wpList);
            _waypoints.RemoveAt(0); // remove the father transform
        }

        if(_waypoints.Count == 0)
        {
            //Debug.Log("SoldierPatrolState There is no patrol route defined");
            _aiScript.SetState(SoldierStates.Idle);
            return;
        }
        
        _hasPatrolRoute = true;
        _aiScript.MoveTo(_waypoints[_currentWaypoint].position);

        //_aiScript.LostTarget();
    }

    public override void Exit()
    {
        //Debug.Log("SoldierPatrolState Exit()");
    }

    public override void Update()
    {
        _totalStatetime += Time.deltaTime;
        //Debug.Log("SoldierPatrolState Update()");

        if (_aiScript.HasLOS("Player"))
        {
            if (!_aiScript.TowerGuard)
                _aiScript.MoveTo(GameObject.FindGameObjectWithTag("Player").transform.position);

            _aiScript.SetState(SoldierStates.Caution);
            return;
        }

        if (_hasPatrolRoute)
        {
            float distance = (_waypoints[_currentWaypoint].position - _owner.transform.position).sqrMagnitude;
            //Debug.Log("distance=" + distance);
            if (distance < _minDistanceNextWaypoint)
            {
                ++_currentWaypoint;
                if (_currentWaypoint > _waypoints.Count - 1)
                {
                    _currentWaypoint = 0;
                    //Debug.Log("SoldierPatrolState Finished my patrol. Time to rest");
                    _aiScript.SetState(SoldierStates.Idle);
                    return;
                }
                _aiScript.MoveTo(_waypoints[_currentWaypoint].position);
            }
        }
    }
}
