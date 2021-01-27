using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] float ImageDuration = 10.0f;

    float elapsedTime;

    void Start()
    {
        StartCoroutine(ShowAndHide());
    }
        
    IEnumerator ShowAndHide()
    {
        gameObject.SetActive(true);

        yield return new WaitForSeconds(ImageDuration);

        gameObject.SetActive(false);
    }
}
