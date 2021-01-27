using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float MaxHealth = 100.0f;
    [SerializeField] float _currentHealth;

    private GameManager _gameManager;

    void Start()
    {
        _currentHealth = MaxHealth;
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void Update()
    {
        
    }

    internal void ApplyDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth < 0)
        {
            _gameManager.InformDeath(transform.gameObject);
            //Destroy(transform.gameObject);
            //transform.gameObject.SetActive(false);
        }
    }
}
