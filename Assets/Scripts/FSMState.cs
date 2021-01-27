using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMState
{
    protected GameObject _owner;
    
    protected FSMState(GameObject owner)
    {
        _owner = owner;
    }

    public abstract void Enter();

    public abstract void Exit();

    public abstract void Update();
}
