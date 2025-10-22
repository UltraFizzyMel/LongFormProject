using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 

public class FallingCube : MonoBehaviour
{
    [Header("Cube Behavior")]
    public float fallSpeed = 5f;
    public float despawnHeight = -10f;
    public float lifetime = 8f;

    [Header("Freeze Settings")]
    public bool isFrozen = false;
    public float freezeDuration = 3f;

    private Rigidbody rb;
    private Renderer cubeRenderer;
    private Material normalMat;
    private Material frozenMat;
    private float spawnTime;

    //Frozen particle effect
    public GameObject effect;

    public void Initialize(float speed, float despawnY, float life, Material normalMaterial, Material frozenMaterial)
    {
        fallSpeed = speed;
        despawnHeight = despawnY;
        lifetime = life;
        normalMat = normalMaterial;
        frozenMat = frozenMaterial;
        spawnTime = Time.time;

        effect.SetActive(false);

        SetupCube();
    }

    void SetupCube()
    {
        // Get or add components
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer == null)
            cubeRenderer = GetComponentInChildren<Renderer>();

        // Configure rigidbody
        rb.useGravity = false; // We'll handle gravity manually
        rb.isKinematic = false;

        // Set initial material
        if (cubeRenderer != null && normalMat != null)
            cubeRenderer.material = normalMat;
    }

    void Update()
    {
        if (!isFrozen)
        {
            // Manual falling
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            effect.SetActive(true); 
        }

        // Check for despawn conditions
        if (transform.position.y <= despawnHeight || Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Player can jump on cube
        if (collision.gameObject.CompareTag("Player"))
        {
            // Optional: Make player parent to cube for moving platform effect
            if (isFrozen)
            {
                collision.transform.SetParent(transform);
            }
        }

        // Freeze bullet hit
        if (collision.gameObject.CompareTag("freezeBullet"))
        {
            FreezeCube();
            Destroy(collision.gameObject); // Destroy the bullet
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Player leaves cube
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    public void FreezeCube()
    {
        if (!isFrozen)
        {
            isFrozen = true;

            // Change material to frozen appearance
            if (cubeRenderer != null && frozenMat != null)
                cubeRenderer.material = frozenMat;

            // Stop physics
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // Start unfreeze timer
            StartCoroutine(UnfreezeAfterTime());
        }
    }

    IEnumerator UnfreezeAfterTime()
    {
        yield return new WaitForSeconds(freezeDuration);

        UnfreezeCube();
    }

    void UnfreezeCube()
    {
        isFrozen = false;
        effect.SetActive(false);

        // Restore normal material
        if (cubeRenderer != null && normalMat != null)
            cubeRenderer.material = normalMat;

        // Resume physics
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}

