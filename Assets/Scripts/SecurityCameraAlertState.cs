using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraAlertState : SecurityCameraState
{
    private float _totalStateTime;
    private float _lastTimeSawPlayer;

    public SecurityCameraAlertState(GameObject owner) : base(owner)
    {
    }

    public override void Enter()
    {
        _aiScript.SetAlarm(true);
        _lastTimeSawPlayer = 0.0f;
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        _totalStateTime += Time.deltaTime;
        _lastTimeSawPlayer += Time.deltaTime;

        if (_aiScript.HasLOS("Player"))
            _lastTimeSawPlayer = 0.0f;

        if (_lastTimeSawPlayer > _aiScript.ForgetAlarmTime)
            _aiScript.SetState(SecurityCamState.Caution);
    }
}
