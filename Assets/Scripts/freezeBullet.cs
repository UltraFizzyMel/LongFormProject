using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freezeBullet : MonoBehaviour
{
    private MovingPlatform platform;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        GameObject Platform =  GameObject.FindGameObjectWithTag("platform");
        if (collision.gameObject == Platform)
        {
            rb.velocity = Vector3.zero; 
            this.transform.SetParent(Platform.transform);
            StartCoroutine(Freeze(Platform));
           
        }

        

    }

    public IEnumerator Freeze(GameObject Obstacle)
    {
        platform = Obstacle.GetComponent<MovingPlatform>();
        platform.isFrozen = true;
        
        yield return new WaitForSeconds(3f);
        platform.isFrozen = false;
        Destroy(this.gameObject);
    }
}
