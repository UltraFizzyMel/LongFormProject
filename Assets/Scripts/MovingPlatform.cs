using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private WayPointPath _waypointPath;
    public freezeBullet freezeB;
    private Renderer platformRenderer;
    public Material frozenMat;

    [SerializeField]
    private float _speed;

    private int _targetWaypointIndex;

    private Transform _previousWaypoint;
    private Transform _targetWaypoint;


    private float _timeToWaypoint;
    private float _elapsedTime;

    public bool isFrozen = false;

    

     void Start()
    {
        TargetNextWaypoint();
        platformRenderer = GetComponent<Renderer>();    
    }

    

     void FixedUpdate()
    {
        if (!isFrozen)
        {
            _elapsedTime += Time.deltaTime;

            float elapsedPercentage = _elapsedTime / _timeToWaypoint;
            elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);

            transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);

            if (elapsedPercentage >= 1)
            {
                TargetNextWaypoint();
            }
        }
       


    }


    private void TargetNextWaypoint()
    {
        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        _elapsedTime = 0;

        float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timeToWaypoint = distanceToWaypoint / _speed;
    }

    public void OnTriggerEnter(Collider other)
    {
       


        if(other.tag == "Player")
        {
            other.transform.SetParent(transform);
        }

        if(other.tag == "freezeBullet")
        {

            while (isFrozen)
            {
                platformRenderer.material = frozenMat;

            }
            StartCoroutine(FreezePlatform());
            Destroy(other.gameObject);

        }

       
        
    }

    private IEnumerator FreezePlatform()
    {
        isFrozen = true;
        yield return new WaitForSeconds(3f);
        isFrozen = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.SetParent(null);
        }
            
    }


}
