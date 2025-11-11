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
    public Material mat;
    public float freezetimer = 3f;

    [SerializeField]
    private float _speed;

    private int _targetWaypointIndex;

    private Transform _previousWaypoint;
    private Transform _targetWaypoint;


    private float _timeToWaypoint;
    private float _elapsedTime;

    public bool isFrozen = false;
    private Coroutine freezeWarningCoroutine;



    void Start()
    {
        TargetNextWaypoint();
        platformRenderer = GetComponent<MeshRenderer>();    
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



        if (other.tag == "Player")
        {
            other.transform.SetParent(transform);
        }

        if (other.tag == "freezeBullet" && !isFrozen)
        {
            StartCoroutine(FreezePlatform());
            Destroy(other.gameObject);
        }




    }

    private IEnumerator FreezePlatform()
    {
        // Set frozen state and material
        isFrozen = true;
        platformRenderer.material = frozenMat;

        // Start warning effect after 1.5 seconds
        StartCoroutine(StartFreezeWarning());

        // Wait for freeze duration
        yield return new WaitForSeconds(freezetimer);

        // Stop warning effect and restore normal state
        isFrozen = false;
        platformRenderer.material = mat;

        if (freezeWarningCoroutine != null)
        {
            StopCoroutine(freezeWarningCoroutine);
        }
    }

    private IEnumerator StartFreezeWarning()
    {
        // Wait until there's only 1.5 seconds left in the freeze
        yield return new WaitForSeconds(freezetimer - 1.5f);

        // Start the flashing warning effect
        freezeWarningCoroutine = StartCoroutine(FreezeWarning());
    }

    private IEnumerator FreezeWarning()
    {
        // Flash between materials until freeze ends
        while (isFrozen)
        {
            platformRenderer.material = mat;
            yield return new WaitForSeconds(0.25f);
            platformRenderer.material = frozenMat;
            yield return new WaitForSeconds(0.25f);
        }

        // Ensure we end with the normal material
        platformRenderer.material = mat;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.SetParent(null);
        }
            
    }


}
