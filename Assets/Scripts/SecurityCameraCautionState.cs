using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraCautionState : SecurityCameraState
{
    private float _totalStateTime;

    public SecurityCameraCautionState(GameObject owner) : base(owner)
    {
    }

    public override void Enter()
    {
        _totalStateTime = 0.0f;
        _aiScript.SetAlarm(false);
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        _totalStateTime += Time.deltaTime;

        if (_aiScript.HasLOS("Player"))
        {
            if (_totalStateTime > _aiScript.TriggerAlarmTime)
                _aiScript.SetState(SecurityCamState.Alert);
        }
        else
        {
            _aiScript.SetState(SecurityCamState.Normal);
        }
    }
}
