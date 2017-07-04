using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic class for define all the states
/// </summary>
public abstract class AIState : MonoBehaviour
{
    protected FSM _machine = null;

    public abstract AIStateType     GetStateType();
    public abstract AIStateType     OnUpdate();
    public abstract void            OnEnterState();
    public abstract void            OnExitState();
    

    /// <summary>
    /// Sets the Finite State Machine
    /// </summary>
    /// <param name="machine"></param>
    public void SetStateMachine(FSM machine)
    {
        _machine = machine;
    }
	
}
