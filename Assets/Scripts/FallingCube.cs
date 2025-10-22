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
    public float warningStartTime = 1.5f; // When to start flashing before unfreeze

    private Rigidbody rb;
    private Renderer cubeRenderer;
    private Material normalMat;
    private Material frozenMat;
    private float spawnTime;
    private Coroutine freezeWarningCoroutine;
    private Coroutine unfreezeCoroutine;

    public void Initialize(float speed, float despawnY, float life, Material normalMaterial, Material frozenMaterial)
    {
        fallSpeed = speed;
        despawnHeight = despawnY;
        lifetime = life;
        normalMat = normalMaterial;
        frozenMat = frozenMaterial;
        spawnTime = Time.time;

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
            unfreezeCoroutine = StartCoroutine(UnfreezeAfterTime());
        }
    }

    IEnumerator UnfreezeAfterTime()
    {
        // Calculate when to start the warning flash (freezeDuration - warningStartTime seconds before unfreezing)
        float warningStart = freezeDuration - warningStartTime;

        // Wait until it's time to start the warning
        yield return new WaitForSeconds(warningStart);

        // Start the flashing warning effect
        freezeWarningCoroutine = StartCoroutine(FreezeWarning());

        // Wait for the remaining warning duration
        yield return new WaitForSeconds(warningStartTime);

        UnfreezeCube();
    }

    IEnumerator FreezeWarning()
    {
        // Flash between materials until unfrozen
        while (isFrozen)
        {
            if (cubeRenderer != null && normalMat != null)
                cubeRenderer.material = normalMat;
            yield return new WaitForSeconds(0.25f);

            if (cubeRenderer != null && frozenMat != null)
                cubeRenderer.material = frozenMat;
            yield return new WaitForSeconds(0.25f);
        }
    }

    void UnfreezeCube()
    {
        isFrozen = false;

        // Stop any running warning coroutine
        if (freezeWarningCoroutine != null)
        {
            StopCoroutine(freezeWarningCoroutine);
            freezeWarningCoroutine = null;
        }

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

