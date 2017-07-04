using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateIdle : AIState
{

    [SerializeField] [Range(0.1f , 10.0f)] private float    _idleTime = 3.0f;
    [SerializeField] private float                          _maxAngle = 45.0f;
    [SerializeField] private float                          _minAngle = 20.0f;

    private float _timer = 0.0f;
   
    /// <summary>
    /// Called by FSM to knows the state type
    /// </summary>
    /// <returns>Current state type</returns>
    public override AIStateType GetStateType()
    {
        return AIStateType.Idle;
    }

    /// <summary>
    /// Called by FSM before enter a state
    /// </summary>
    public override void OnEnterState()
    {
        //reset values
        _machine.navAgent.path  = new UnityEngine.AI.NavMeshPath(); //clear the path
        _machine.speed          = 0.0f;
        _timer                  = _idleTime;

        //configure rootMotion and navAgent
        _machine.SetNavAgentControl(true, false);
        _machine.SetAnimatorRootMotionControl(true, true);
        
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

        _timer -= Time.deltaTime;

        if (_timer > 0.0f)
            return AIStateType.Idle;

        //if we dont have a path and we are no procesing a path, set a new path
        if (!_machine.navAgent.hasPath && !_machine.navAgent.pathPending)
            _machine.navAgent.SetDestination(_machine.waypointNetwork.GetWayPoint());        
        //keep on the state while procesing the path, can take a fraction of seg
        else if(_machine.navAgent.pathPending)
            return AIStateType.Idle;
        
        //get the signed angle betewn our forwar direction, and the direction what our nav agent wants to face
        float angle = FSM.FindSignedAngle(transform.forward, (_machine.navAgent.steeringTarget - transform.position));
        
        //if we have a path and the angle is to big and we are no performing a turn on spot animation, lest do a turn on spot
        if(_machine.navAgent.hasPath && Mathf.Abs(angle) >= _maxAngle  && !_machine.IsAnimationPlaying("TurnOnSpotBlendTree"))
            _machine.turnOnSpot = angle;            
        
        //if we are in good range lest start patrol the path
        else if(Mathf.Abs(angle) < _minAngle)
            return AIStateType.Patroling;
        

        return AIStateType.Idle;
    }
}
