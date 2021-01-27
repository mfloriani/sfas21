using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform PlayerTransform;

    [SerializeField] [Range(0.0f, 1.0f)] float SmoothFactor = 0.5f;
    [SerializeField] float RotationYSpeed = 1.5f;
    [SerializeField] float RotationXSpeed = 1.0f;
    [SerializeField] bool LookAtPlayer = true;

    private Vector3 _cameraOffset;

    void Start()
    {
        _cameraOffset = transform.position - PlayerTransform.position;
    }

    void LateUpdate()
    {
        if(PlayerTransform)
        {
            float xAxis = Input.GetAxisRaw("RStickX");
            float yAxis = Input.GetAxisRaw("RStickY");

            Quaternion camTurnYAngle = Quaternion.AngleAxis(xAxis * RotationYSpeed, Vector3.up);        
            Quaternion camTurnXAngle = Quaternion.AngleAxis(yAxis * RotationXSpeed, Vector3.right);
        
            _cameraOffset = camTurnYAngle * camTurnXAngle * _cameraOffset;
        
            // TODO: limit the x angle

            // follow player
            Vector3 newPos = PlayerTransform.position + _cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

            if(LookAtPlayer)
                transform.LookAt(PlayerTransform); // focus on player
        }
    }

}
