using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public Transform GetWaypoint(int index)
    {
        if (index < 0 || index >= transform.childCount)
        {
            return null;
        }
        return transform.GetChild(index);
    }

    public int GetNextWaypointIndex(int currentIndex)
    {
        if (currentIndex + 1 == transform.childCount)
        {
            return 0; // Loop back to the first waypoint
        }
        return currentIndex + 1;
    }
}
