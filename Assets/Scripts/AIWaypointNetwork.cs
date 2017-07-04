using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIWaypointNetwork : MonoBehaviour 
{    
    [SerializeField] private List<Transform> _waypoints  = null;

    private int _currentWaypoint    = 0;

    public Vector3 GetWayPoint(bool random = false)
    {
        int waypoint = 0;

        if (random)
        {
            waypoint = Random.Range(0 , _waypoints.Count);

            while(waypoint == _currentWaypoint)            
                waypoint = Random.Range(0 , _waypoints.Count);

            _currentWaypoint = waypoint;

            return _waypoints[waypoint].position;
        }

        _currentWaypoint = _currentWaypoint == _waypoints.Count - 1 ? 0 : _currentWaypoint + 1;

        return _waypoints[_currentWaypoint].position;
    }
	
}
