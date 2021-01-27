using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoldierState : FSMState
{
    protected SoldierAI _aiScript;
    
    protected SoldierState(GameObject owner) : base(owner)
    {
        _aiScript = _owner.GetComponentInChildren<SoldierAI>();
    }
}
