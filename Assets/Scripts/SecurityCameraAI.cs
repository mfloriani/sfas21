using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SecurityCamState
{
    Normal,
    Caution,
    Alert
}

public class SecurityCameraAI : MonoBehaviour
{
    [SerializeField] public float RotationSpeed = 10.0f;
    [SerializeField] public float MaxRotation = 45.0f;
    [SerializeField] public float TriggerAlarmTime = 3.0f;
    [SerializeField] public float ForgetAlarmTime = 10.0f;
    
    private LineOfSight _los;
    private FiniteStateMachine _fsm;
    private AudioSource _alarmSfx;
    private Vector3 _startAngle;
    private float _currentAngle;
    private GameObject _alarmRange;

    void Start()
    {
        _startAngle = transform.eulerAngles;
        _los = transform.Find("Body/LineOfSight").GetComponent<LineOfSight>();
        _alarmRange = transform.Find("Alarm Range").gameObject;
        _alarmSfx = GetComponent<AudioSource>();
        _fsm = new FiniteStateMachine();
        _fsm.Start(new SecurityCameraNormalState(gameObject));
    }

    void Update()
    {
        _fsm.Update();
    }

    public Vector3 StartAngle()
    {
        return _startAngle;
    }

    public float CurrentAngle()
    {
        return _currentAngle;
    }

    public void CurrentAngle(float angle)
    {
        _currentAngle = angle;
    }

    public void SetAlarm(bool onOff)
    {
        if(onOff)
        {
            if (!_alarmSfx.isPlaying)
                _alarmSfx.Play();

            _alarmRange.SetActive(true);
        }
        else
        {
            _alarmSfx.Stop();
            _alarmRange.SetActive(false);
        }
    }

    public bool HasLOS(string tag)
    {
        return _los.SeeByTag(tag);
    }

    public void SetState(SecurityCamState newState)
    {
        switch(newState)
        {
            case SecurityCamState.Normal:

                _los.SetStatus(LineOfSight.Status.Idle);
                _fsm.ChangeState(new SecurityCameraNormalState(gameObject));

                break;

            case SecurityCamState.Caution:

                _los.SetStatus(LineOfSight.Status.Suspicious);
                _fsm.ChangeState(new SecurityCameraCautionState(gameObject));

                break;

            case SecurityCamState.Alert:

                _los.SetStatus(LineOfSight.Status.Alerted);
                _fsm.ChangeState(new SecurityCameraAlertState(gameObject));

                break;
        }
    }
}
