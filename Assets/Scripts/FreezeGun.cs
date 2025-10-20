using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeGun : MonoBehaviour
{
    public GameObject freezeBullet;
    public freezeBullet bullet;
    
    public Camera cam;
    public Transform attackPt;


    public float shootForce;
    public float maxShotDistance = 100f;


    public void shootBullet()
    {

        Debug.Log("SHOOT");
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, maxShotDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxShotDistance);
        }

        Vector3 directionOfBullet = targetPoint - attackPt.position;

        Debug.DrawRay(attackPt.position, directionOfBullet.normalized * 5f, Color.cyan, 2.0f);

        GameObject fBullet = Instantiate(freezeBullet, attackPt.position, Quaternion.identity);
        fBullet.GetComponent<Rigidbody>().AddForce(directionOfBullet.normalized * shootForce, ForceMode.Impulse);

    }

    
    
}
