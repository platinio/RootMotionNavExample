using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIStateType
{
    Idle,
    Patroling
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class FSM : MonoBehaviour 
{
    [SerializeField] private Dictionary<AIStateType, AIState>   _states             = new Dictionary<AIStateType, AIState>();
    [SerializeField] private AIStateType                        _currentStateType   = AIStateType.Idle;
    [SerializeField] private AIWaypointNetwork                  _waypointNetwork    = null;
    
    private AIState         _currentState       = null;
    private Animator        _animator           = null;
    private NavMeshAgent    _navAgent           = null;
    private float           _speed              = 0.0f;
    private float           _turnOnSpot         = 0.0f;
    private bool            _useRootRotation    = false;
    private bool            _useRootPosition    = false;

    public float                speed               { get { return _speed;              }       set { _speed = value;           } }
    public float                turnOnSpot          { get { return _turnOnSpot;         }       set { _turnOnSpot = value;      } }
    public AIWaypointNetwork    waypointNetwork     { get { return _waypointNetwork;    } }
    public NavMeshAgent         navAgent            { get { return _navAgent;           } }

    void Awake()
    {
        //get references
        _animator = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();        
    }

    void Start()
    {
        //get states
        AIState[] states = GetComponents<AIState>();

        //store staes in dictonary
        for (int n = 0; n < states.Length; n++)
        {
            //if the dictionary is no null and no exist already in the list add it to the dictionary
            if (states[n] != null && !_states.ContainsKey(states[n].GetStateType()))
            {
                _states[states[n].GetStateType()] = states[n];
                states[n].SetStateMachine(this);
            }
        }

        //set initial state
        if (_states.ContainsKey(_currentStateType))
        {
            _currentState = _states[_currentStateType];
            _currentState.OnEnterState();
        }
        //if we forgot to add the initial state les end the game
        else
        {
            _currentState = null;
            Debug.LogError("AI_ERROR: initial state is missing");
            Debug.Break();
        }
    }

    private void Update()
    {
        if (_currentState == null) return;

        SetAnimParam();

        AIStateType newStateType = _currentState.OnUpdate();

        if (newStateType != _currentStateType)
        {
            AIState newState = null;

            if (_states.TryGetValue(newStateType, out newState))
            {
                _currentState.OnExitState();
                newState.OnEnterState();
                _currentState = newState;
            }
            else if (_states.TryGetValue(AIStateType.Idle , out newState))
            {
                _currentState.OnExitState();
                newState.OnEnterState();
                _currentState = newState;
            }

            else
            {
                Debug.LogError("AI_ERROR: AIState is missing");
                Debug.Break();
            }

            _currentStateType = newStateType;
        }
    }

    /// <summary>
    /// Configure animation root motion
    /// </summary>
    /// <param name="updatePosition">Must update position?</param>
    /// <param name="updateRotaion">Must update rotation?</param>
    public void SetAnimatorRootMotionControl(bool updatePosition , bool updateRotaion)
    {
        _useRootPosition = updatePosition;
        _useRootRotation = updateRotaion;
    }

    /// <summary>
    /// Configure Navigation Agent controll
    /// </summary>
    /// <param name="updatePosition">Must update position?</param>
    /// <param name="updateRotation">Must update rotation?</param>
    public void SetNavAgentControl(bool updatePosition , bool updateRotation)
    {
        _navAgent.updatePosition = updatePosition;
        _navAgent.updateRotation = updateRotation;
    }

    /// <summary>
    /// Called by Unity before apply rootMotion in the object
    /// </summary>
    public void OnAnimatorMove()
    {
        if (_useRootPosition)
            _navAgent.velocity = _animator.deltaPosition / Time.deltaTime;
        if (_useRootRotation)
            transform.rotation = _animator.rootRotation;
    }

    /// <summary>
    /// Set animation parameters
    /// </summary>
    private void SetAnimParam()
    {
        _animator.SetFloat("TurnOnSpot" ,   _turnOnSpot);
        _animator.SetFloat("Speed"      ,   _speed);
    }

    /// <summary>
    /// Is the animation already playing?
    /// </summary>
    /// <param name="animationName">the animation state name</param>
    /// <param name="layer">(optional) layer</param>
    /// <returns></returns>
    public bool IsAnimationPlaying(string animationName , int layer = 0)
    {
        return _animator.GetCurrentAnimatorStateInfo(layer).IsName(animationName);
    }

    /// <summary>
    /// returns the signed angle 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float FindSignedAngle(Vector3 from, Vector3 to)
    {

        //if the two vectors are equal just return 0
        if (from == to)
            return 0.0f;

        //get angle and cross product
        float angle = Vector3.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        return angle * Mathf.Sign(cross.y);

    }
	
}
