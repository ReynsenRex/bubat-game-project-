using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public List<GameObject> enemyHitboxes;
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

        if (enemyHitboxes == null)
        {
            Debug.LogError("enemyHitboxes list is not initialized.");
        }
    }

    void Update()
    {
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

   private void HandleMovement()
{
    float moveHorizontal = Input.GetAxis("Horizontal");
    float moveVertical = Input.GetAxis("Vertical");

    // Create a movement vector based on input
    Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

    // Check if the Shift key is pressed for sprinting
    float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

    // If there is movement input, move the character
    if (movement.magnitude > 0)
    {
        // Calculate the desired movement direction based on the character's forward direction
        Vector3 desiredMovement = transform.forward * movement.z + transform.right * movement.x;

        // Check for wall collision using a raycast
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, desiredMovement, out hit, 1f))
        {
            // Move the character using Rigidbody
            rb.MovePosition(rb.position + desiredMovement.normalized * currentSpeed * Time.deltaTime);

            // Rotate the character to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else if (hit.collider.CompareTag("Wall"))
        {
            // If the object hit is tagged as "Wall", do not move
            Debug.Log("Hit a wall, stopping movement.");
        }
    }

    // Update animator parameters
    anim.SetFloat("x", moveHorizontal);
    anim.SetFloat("y", moveVertical);
    anim.SetFloat("speed", movement.magnitude);
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
        anim.SetFloat("speed ", 0);

        int attackCombo = 0; // Current combo index
        int maxCombo = 3; // Maximum combo attacks
        float attackDuration = 1.0f; // Time for each attack animation
        float comboResetTime = 1.5f; // Time before combo resets
        float lastInputTime = Time.time;

        while (isAttacking)
        {
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
                    yield break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.1f); // Small delay before next combo

            if (attackCombo == 0) break; // Exit loop if combo is reset
        }

        anim.SetFloat("attack", 0); // Reset animation to idle
        isAttacking = false;
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
}