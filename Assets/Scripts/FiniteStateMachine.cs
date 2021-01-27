using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    private FSMState _currentState;

    public FiniteStateMachine()
    {

    }

    public void Start(FSMState initialState)
    {
        ChangeState(initialState);
    }

    public void Update()
    {
        _currentState.Update();
    }

    public void ChangeState(FSMState newState)
    {
        if(_currentState != null)
            _currentState.Exit();

        _currentState = newState;

        _currentState.Enter();
    }

    public System.Type GetCurrentStateType()
    {
        return _currentState.GetType();
    }

}
