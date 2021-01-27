using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SecurityCameraState : FSMState
{
    protected SecurityCameraAI _aiScript;

    protected SecurityCameraState(GameObject owner) : base(owner)
    {
        _aiScript = _owner.GetComponentInChildren<SecurityCameraAI>();
    }
}
