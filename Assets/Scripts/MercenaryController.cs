using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenaryController : MonoBehaviour
{
    [SerializeField] float walkingSpeedFactor = 1.5f;
    [SerializeField] float runningSpeedFactor = 2.5f;
    [SerializeField] float crouchedWalkingSpeedFactor = 1.0f;
    [SerializeField] float maxStamina = 100.0f;
    [SerializeField] float recoveringStaminaRate = 25.0f;
    [SerializeField] float runningStaminaCost = 30;
    [SerializeField] float turnSpeed = 2.0f;
    [SerializeField] float BulletTraceDuration = 0.5f;
    [SerializeField] float RateOfFire = 1.0f;
    [SerializeField] Transform _muzzle;
    [SerializeField] float WeaponRange = 10.0f;
    [SerializeField] float BulletDamage = 50.0f;
    [SerializeField] AudioClip ShotSFX;

    private Animator _animator;
    private float _stamina;
    private GameManager _gameManager;
    private LineRenderer _bulletTrace;
    private float _lastShotFiredTime;
    private AudioSource _audioSource;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _stamina = maxStamina;
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _bulletTrace = GetComponent<LineRenderer>();
        _lastShotFiredTime = 100.0f;
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        Move();
    }

    private void Shoot()
    {
        _lastShotFiredTime += Time.deltaTime;

        bool fire = Input.GetAxis("Fire") > 0;
        if (fire && _lastShotFiredTime > RateOfFire)
        {        
            _lastShotFiredTime = 0;

            _audioSource.clip = ShotSFX;
            _audioSource.volume = 0.2f;
            _audioSource.Play();

            _muzzle.GetComponent<ParticleSystem>().Play();
        }
        
        Vector3 rayOrigin = _muzzle.position;
        _bulletTrace.SetPosition(0, rayOrigin);
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, WeaponRange))
        {
            _bulletTrace.SetPosition(1, hit.point);

            if(fire)
            {
                Health health = hit.transform.GetComponent<Health>();
                if (health)
                    health.ApplyDamage(BulletDamage);
            }
        }
        else
        {
            _bulletTrace.SetPosition(1, rayOrigin + (transform.forward * WeaponRange));
        }
    }

    internal void Die()
    {
        _animator.SetTrigger("die");
        gameObject.tag = "Untagged";
    }

    private void Move()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");
        float yAxis = Input.GetAxisRaw("Vertical");
        //Debug.Log(xAxis + "," + yAxis);

        float totalStaminaCost = 0.0f;
        float speedFactor = 1.0f;
        float speed = 0.0f;
        bool walking = false;
        bool running = false;

        if (xAxis < 0 || xAxis > 0)
            speed += Mathf.Abs(xAxis);

        if (yAxis < 0 || yAxis > 0)
            speed += Mathf.Abs(yAxis);

        if (speed > 0)
        {
            walking = true;
            speedFactor = walkingSpeedFactor;
        }

        if (walking && Input.GetButton("Run") && _stamina >= runningStaminaCost)
        {
            running = true;
            walking = false;
            speedFactor = runningSpeedFactor;
            totalStaminaCost += runningStaminaCost;
        }

        _animator.SetBool("walk", walking);
        _animator.SetBool("run", running);

        // handle move direction based on camera orientation
        Vector3 inputDir = new Vector3(xAxis, 0, yAxis);
        Vector3 camDirection = Camera.main.transform.rotation * inputDir;
        Vector3 targetDirection = new Vector3(camDirection.x, 0, camDirection.z);

        if (inputDir != Vector3.zero)
        {
            //turn the character to face the direction of travel when there is input
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * turnSpeed);
        }

        transform.position += targetDirection.normalized * speedFactor * Time.deltaTime; // move player towards input direction

        // stamina handling
        _stamina = _stamina - totalStaminaCost < 0.0f ? 0.0f : _stamina - totalStaminaCost; // consume stamina
        _stamina = _stamina > maxStamina ? maxStamina : _stamina + recoveringStaminaRate * Time.deltaTime; // recover stamina
    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("Collision: " + other.gameObject.tag);
        if (other.gameObject.tag == "Projectile")
        {
            Debug.Log("I was hit");
            // TODO: play took damage animation
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Extraction")
        {
            _gameManager.AskForExtraction();
        }
        else if (other.gameObject.tag == "Door")
        {
            _gameManager.MissionInfo("Press \"A\" or \"Enter\" to open");
        }
        else if (other.gameObject.tag == "Data")
        {
            _gameManager.MissionInfo("Press \"A\" or \"Enter\"  to get the data");
        }
        else if (other.gameObject.tag == "Building")
        {
            Debug.Log("Inside building");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Building")
        {
            Debug.Log("Outside building");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Data")
        {
            if (Input.GetButton("Action"))
                _gameManager.InformDataCollection();
        }
        if (other.gameObject.tag == "Door")
        {
            if (Input.GetButton("Action"))
                other.gameObject.GetComponent<Door>().Open();
        }
    }
}
