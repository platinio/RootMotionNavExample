using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStatePatrol : AIState
{

    [SerializeField] [Range(1.0f , 10.0f)] private float _speed             = 3.0f;
    [SerializeField] [Range(0.1f, 10.0f)]  private float _slerpSpeed        = 3.0f;
    [SerializeField] [Range(0.0f, 10.0f)]  private float _stoppingDistance  = 1.0f;


    /// <summary>
    /// Called by FSM to knows the state type
    /// </summary>
    /// <returns>Current state type</returns>
    public override AIStateType GetStateType()
    {
        return AIStateType.Patroling;
    }

    /// <summary>
    /// Called by FSM before enter a state
    /// </summary>
    public override void OnEnterState()
    {
        //reset values
        _machine.turnOnSpot = 0.0f;
        _machine.speed = _speed;

        //configure rootMotion and navAgent
        _machine.SetNavAgentControl(true, false);
        _machine.SetAnimatorRootMotionControl(true, false);
    }

    /// <summary>
    /// Called by FSM before leave a state
    /// </summary>
    public override void OnExitState()
    {

    }

    /// <summary>
    /// Called by FSM machine every update
    /// </summary>
    /// <returns>next state</returns>
    public override AIStateType OnUpdate()
    {
        //set desire navAgent rotation slowly
        Quaternion newRot = Quaternion.LookRotation(_machine.navAgent.desiredVelocity);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * _slerpSpeed);

        //if we reach the destination
        if (Vector3.Distance(_machine.navAgent.destination, transform.position) < _stoppingDistance)
            return AIStateType.Idle;                 
        
        return AIStateType.Patroling;
    }
}
