using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraNormalState : SecurityCameraState
{
    private float _yRotation;
    private GameObject _body;

    public SecurityCameraNormalState(GameObject owner) : base(owner)
    {

    }

    public override void Enter()
    {
        _yRotation = _aiScript.CurrentAngle();
        _body = _owner.transform.Find("Body").gameObject;
        _aiScript.SetAlarm(false);
    }

    public override void Exit()
    {
        _aiScript.CurrentAngle(_yRotation);
    }

    public override void Update()
    {
        _yRotation += _aiScript.RotationSpeed * Time.deltaTime;
        _yRotation = Mathf.Clamp(_yRotation, -_aiScript.MaxRotation, _aiScript.MaxRotation);

        if (_yRotation >= _aiScript.MaxRotation || _yRotation <= -_aiScript.MaxRotation)
            _aiScript.RotationSpeed *= -1;

        _body.transform.eulerAngles = (new Vector3(0.0f, _yRotation, 0.0f)) + _aiScript.StartAngle();

        if (_aiScript.HasLOS("Player"))
        {
            _aiScript.SetState(SecurityCamState.Caution);

        }
    }
}
