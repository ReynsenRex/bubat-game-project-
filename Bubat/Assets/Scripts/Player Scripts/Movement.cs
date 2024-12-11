using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public List<GameObject> enemyHitboxes = new List<GameObject>(); // Initialize the list to prevent null errors
    public GameObject weaponHitbox;
    public float speed = 1.0f; // Normal movement speed
    public float sprintSpeed = 5.0f; // Sprint speed
    public float rollSpeed = 5f;
    public float rollDuration = 0.5f;
    private Rigidbody rb;
    private Animator anim;

    private bool dodge = false;
    private float rollTime = 0f;
    private Vector3 rollDirection;
    private bool isAttacking = false;
    private Vector3 rootMotionDelta = Vector3.zero; // To store root motion adjustments

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found on this GameObject.");
        }

        AssignHitboxes();

        if (weaponHitbox != null)
        {
            weaponHitbox.SetActive(false); // Disable weapon hitbox by default
        }
        else
        {
            Debug.LogWarning("Weapon hitbox is not assigned. Please set it in the inspector.");
        }
    }

    void Update()
    {
        // Clean up enemyHitboxes list by removing inactive or null hitboxes
        RemoveInactiveHitboxes();

        if (!isAttacking) // Only allow movement if not attacking
        {
            if (!dodge)
            {
                HandleMovement();
                if (Input.GetMouseButtonDown(0)) // Attack on left mouse button
                {
                    StartCoroutine(Attack());
                }
                if (Input.GetKeyDown(KeyCode.Space)) // Dodge on spacebar
                {
                    StartDodge();
                }
            }
            else
            {
                Dodge();
            }
        }
    }

    private void RemoveInactiveHitboxes()
    {
        for (int i = enemyHitboxes.Count - 1; i >= 0; i--)
        {
            if (enemyHitboxes[i] == null || !enemyHitboxes[i].activeSelf)
            {
                Debug.Log("Removed inactive or null hitbox: " + (enemyHitboxes[i] != null ? enemyHitboxes[i].name : "null"));
                enemyHitboxes.RemoveAt(i); // Remove inactive or null hitbox from the list
            }
        }
    }

    private void AssignHitboxes()
    {
        // Find all objects with the "Hitbox" tag in the entire scene
        GameObject[] allHitboxes = GameObject.FindGameObjectsWithTag("Hitbox");

        foreach (GameObject hitbox in allHitboxes)
        {
            if (!enemyHitboxes.Contains(hitbox))  // Check to avoid adding duplicates
            {
                enemyHitboxes.Add(hitbox);
                Debug.Log("Assigned hitbox: " + hitbox.name);
            }
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Get camera-forward and camera-right vectors
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Flatten camera vectors to ignore vertical axis
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Create movement vector relative to the camera
        Vector3 movement = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;

        // Determine current speed (normal or sprint)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

        // If there's movement, check for obstacles and move
        if (movement.magnitude > 0)
        {
            // Calculate desired movement direction
            Vector3 desiredMovement = movement * currentSpeed * Time.deltaTime;

            // Perform a raycast to check for collisions
            if (!Physics.Raycast(transform.position, movement, out RaycastHit hit, 0.5f))
            {
                // Apply movement using Rigidbody
                rb.transform.Translate(desiredMovement, Space.World);

                // Smoothly rotate the character towards movement direction
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        // Update animator parameters
        anim.SetFloat("x", moveHorizontal);
        anim.SetFloat("y", moveVertical);
        anim.SetFloat("speed", movement.magnitude * currentSpeed / sprintSpeed); // Normalize speed for sprinting
    }

    private void StartDodge()
    {
        dodge = true;
        rollTime = rollDuration;

        // Deactivate enemy hitboxes when dodging
        foreach (GameObject hitbox in enemyHitboxes)
        {
            if (hitbox != null)
            {
                hitbox.GetComponent<EnemyHitbox>().DeactivateHitbox();
            }
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rollDirection = movement.normalized;
        anim.SetBool("dodge", true);

        StartCoroutine(ReactivateHitbox(rollDuration));
    }

    private void Dodge()
    {
        if (rollTime > 0)
        {
            transform.Translate(rollDirection * rollSpeed * Time.deltaTime);
            rollTime -= Time.deltaTime;
        }
        else
        {
            dodge = false;
            anim.SetBool("dodge", false);
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        // Stop movement and reset speed in Animator
        anim.SetFloat("speed", 0);

        int attackCombo = 0; // Current combo index
        int maxCombo = 3; // Maximum combo attacks
        float attackDuration = 1.0f; // Time for each attack animation
        float comboResetTime = 1.5f; // Time before combo resets
        float lastInputTime = Time.time;

        while (isAttacking)
        {
            // Enable the weapon's hitbox at the start of the attack
            if (weaponHitbox != null)
            {
                weaponHitbox.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Weapon hitbox is not assigned!");
            }

            anim.SetFloat("attack", attackCombo + 1); // Trigger attack animation (1-based index)
            float animationTime = 0f;

            while (animationTime < attackDuration)
            {
                animationTime += Time.deltaTime;

                // Handle combo continuation
                if (Input.GetMouseButtonDown(0) && animationTime >= 0.5f)
                {
                    lastInputTime = Time.time;
                    if (attackCombo < maxCombo - 1)
                    {
                        attackCombo++;
                    }
                    else
                    {
                        attackCombo = 0; // Reset combo
                    }
                    break;
                }

                // Reset combo if no input within the reset time
                if (Time.time - lastInputTime > comboResetTime)
                {
                    attackCombo = 0;
                    anim.SetFloat("attack", 0); // Reset attack animation
                    isAttacking = false;

                    // Disable the weapon's hitbox after the attack finishes
                    if (weaponHitbox != null)
                    {
                        weaponHitbox.SetActive(false);
                    }
                    yield break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.1f); // Small delay before next combo

            if (attackCombo == 0) break; // Exit loop if combo is reset
        }

        anim.SetFloat("attack", 0); // Reset animation to idle
        isAttacking = false;

        // Disable the weapon's hitbox after the attack ends
        if (weaponHitbox != null)
        {
            weaponHitbox.SetActive(false);
        }
    }

    // Reactivate hitboxes after a delay
    private IEnumerator ReactivateHitbox(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (enemyHitboxes != null)
        {
            foreach (GameObject hitbox in enemyHitboxes)
            {
                if (hitbox != null)
                {
                    hitbox.SetActive(true);
                    Debug.Log("Enemy hitbox reactivated: " + hitbox.name);
                }
                else
                {
                    Debug.LogWarning("A hitbox in enemyHitboxes is null during reactivation!");
                }
            }
        }
        else
        {
            Debug.LogError("enemyHitboxes is null when trying to reactivate hitboxes.");
        }
    }

    void OnAnimatorMove()
    {
        // Apply root motion only when the character is attacking
        if (anim && isAttacking)
        {
            Vector3 deltaPosition = anim.deltaPosition + rootMotionDelta;
            Quaternion deltaRotation = anim.deltaRotation;

            // Move and rotate the character
            rb.MovePosition(rb.position + deltaPosition);
            rb.MoveRotation(rb.rotation * deltaRotation);

            // Reset accumulated manual root motion after applying
            rootMotionDelta = Vector3.zero;
        }
    }
}
