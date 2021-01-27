using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SoldierStates
{
    Idle,
    Patrol,
    Caution,
    Alert,
    Die
}

[RequireComponent(typeof(NavMeshAgent))]
public class SoldierAI : MonoBehaviour
{
    public GameObject PatrolRoute;

    [SerializeField] public float MinDistanceToShoot = 30.0f;
    [SerializeField] public float RateOfFire = 1.0f;
    [SerializeField] public float MaxTimeInIdle = 5.0f;    
    [SerializeField] public bool TowerGuard;
    [SerializeField] float TriggerAlertTime = 5.0f;
    [SerializeField] float TriggerSuspiciousTime = 2.0f;
    [SerializeField] float ForgetPlayerTime = 3.0f;    
    [SerializeField] float AlertSpeed = 3.0f;
    [SerializeField] float CautionSpeed = 2.0f;    
    [SerializeField] float NormalSpeed = 1.0f;    
    [SerializeField] float AimRotationFactor = 1.0f;
    [SerializeField] float ListeningDistance = 30.0f;
    [SerializeField] float WeaponRange = 10.0f;
    [SerializeField] float BulletDamage = 50.0f;
    [SerializeField] float BulletTraceDuration = 0.5f;
    [SerializeField] float MinDistToArrive = 1.0f;
    [SerializeField] AudioClip AlertSFX;
    [SerializeField] AudioClip ShotSFX;
    [SerializeField] Transform _muzzle;

    private AudioSource _audioSource;
    private Animator _anim;
    private FiniteStateMachine _fsm;
    private LineOfSight _lineOfSight;
    private NavMeshAgent _agentNavMesh;
    private SoldierStates _state;
    private Transform _alertSign;
    private LineRenderer _bulletTrace;
    private GameObject _alarmRange;

    void Start()
    {
        _anim = GetComponent<Animator>();
        
        _lineOfSight = transform.Find("LineOfSight").GetComponent<LineOfSight>();

        _agentNavMesh = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();

        _state = SoldierStates.Idle;
        _fsm = new FiniteStateMachine();
        _fsm.Start(new SoldierIdleState(gameObject));

        _alertSign = transform.Find("Alert");
        _alertSign.gameObject.SetActive(false);

        _bulletTrace = GetComponent<LineRenderer>();
        
        _alarmRange = transform.Find("Alarm Range").gameObject;
        _alarmRange.SetActive(false);
    }

    void Update()
    {
        _fsm.Update();

        Debug.DrawRay(_muzzle.position, transform.forward * WeaponRange, Color.red);
    }

    public void SetState(SoldierStates newState)
    {
        if (_state == newState)
            return;

        if(newState == SoldierStates.Idle)
        {
            _fsm.ChangeState(new SoldierIdleState(gameObject));
            _anim.SetBool("walk", false);
            _agentNavMesh.speed = NormalSpeed;
            _alertSign.gameObject.SetActive(false);
            _anim.SetBool("alert", false);
            _alarmRange.SetActive(false);
        }
        else if (newState == SoldierStates.Patrol)
        {
            _lineOfSight.SetStatus(LineOfSight.Status.Idle);
            _fsm.ChangeState(new SoldierPatrolState(gameObject));
            _agentNavMesh.speed = NormalSpeed;
            _alertSign.gameObject.SetActive(false);
            _anim.SetBool("alert", false);
            _alarmRange.SetActive(false);
        }
        else if (newState == SoldierStates.Caution)
        {
            _lineOfSight.SetStatus(LineOfSight.Status.Suspicious);
            _fsm.ChangeState(new SoldierCautionState(gameObject));

            _audioSource.clip = AlertSFX;
            _audioSource.volume = 1.0f;
            _audioSource.Play();

            _agentNavMesh.speed = CautionSpeed;

            _alertSign.gameObject.SetActive(true);
            _anim.SetBool("alert", true);

            if(TowerGuard)
                Stop();
        }
        else if (newState == SoldierStates.Alert)
        {
            _lineOfSight.SetStatus(LineOfSight.Status.Alerted);
            _fsm.ChangeState(new SoldierAlertState(gameObject));
            _agentNavMesh.speed = AlertSpeed;
            _anim.SetBool("alert", true);
            if (TowerGuard)
                Stop();
        }
        else if (newState == SoldierStates.Die)
        {
            Stop();
            _anim.SetTrigger("die");
            _fsm.ChangeState(new SoldierDieState(gameObject));
            _lineOfSight.gameObject.SetActive(false);
            _agentNavMesh.enabled = false;
            _alertSign.gameObject.SetActive(false);
            this.enabled = false;
        }

        _state = newState;
    }

    public void Aim(Vector3 target)
    {
        var targetRotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, AimRotationFactor * Time.deltaTime);
    }

    public void Shoot()
    {
        _anim.SetBool("run", false);
        _agentNavMesh.isStopped = true;

        _muzzle.GetComponent<ParticleSystem>().Play();

        _audioSource.clip = ShotSFX;
        _audioSource.volume = 0.2f;
        _audioSource.Play();

        _anim.SetTrigger("shoot");

        _alarmRange.SetActive(true);

        StartCoroutine(ShotTraceEffect());

        Vector3 rayOrigin = _muzzle.position;

        _bulletTrace.SetPosition(0, rayOrigin);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, WeaponRange))
        {
            _bulletTrace.SetPosition(1, hit.point);
            Health health = hit.transform.GetComponent<Health>();
            if (health)
                health.ApplyDamage(BulletDamage);
        }
        else
        {
            _bulletTrace.SetPosition(1, rayOrigin + (transform.forward * WeaponRange));
        }
    }

    public void Die()
    {
        SetState(SoldierStates.Die);
    }

    private IEnumerator ShotTraceEffect()
    {
        _bulletTrace.enabled = true;
        yield return new WaitForSeconds(BulletTraceDuration);
        _bulletTrace.enabled = false;
    }


    public SoldierStates GetState()
    {
        return _state;
    }

    public bool HasArrived()
    {
        float dist = Mathf.Abs((_agentNavMesh.destination - transform.position).sqrMagnitude);        
        if (dist < MinDistToArrive)
            return true;

        return false;
    }

    public bool HasClearShot()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        float distance = (player.transform.position - transform.position).sqrMagnitude;

        if (distance <= MinDistanceToShoot)
            return true;

        return false;
    }

    public bool HasLOS(string tag)
    {
        return _lineOfSight.SeeByTag(tag);
    }

    public void MoveTo(Vector3 newDestination)
    {
        _agentNavMesh.destination = newDestination;
        _agentNavMesh.isStopped = false;
        if(_state == SoldierStates.Alert || _state == SoldierStates.Caution)
        {
            _anim.SetBool("run", true);
            _anim.SetBool("walk", false);
        }
        else
        {
            _anim.SetBool("run", false);
            _anim.SetBool("walk", true);
        }
    }

    public Vector3 GetDestination()
    {
        return _agentNavMesh.destination;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_state == SoldierStates.Die)
            return;

        if (other.tag == "Alarm")
        {
            if (_state == SoldierStates.Idle || _state == SoldierStates.Patrol)
            {
                MoveTo( other.transform.Find("Rally Point").position );
                SetState(SoldierStates.Caution);
            }
        }
        else if (other.tag == "ShotFired")
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if(player)
                MoveTo(player.transform.position);
            SetState(SoldierStates.Caution);
        }
    }

    public void Stop()
    {
        _agentNavMesh.isStopped = true;
        _anim.SetBool("run", false);
        _anim.SetBool("walk", false);
    }
}
