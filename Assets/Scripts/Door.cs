using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool _isOpen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        if(!_isOpen)
        {
            _isOpen = true;
            StartCoroutine(OpenAnimation());
        }
    }

    IEnumerator OpenAnimation()
    {
        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(Vector3.up * 90.0f);

        float duration = 1.0f;
        float elapsed = 0.0f;
        while(elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

}
