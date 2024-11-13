using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public List<GameObject> enemyHitboxes;
    public float speed = 5.0f;
    public float rollSpeed = 5f;
    public float rollDuration = 0.5f;
    private Rigidbody rb;
    private CharacterController controller;
    private Animator anim;

    private bool dodge = false;
    private float rollTime = 0f;
    private Vector3 rollDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found on this GameObject.");
        }

        // Ensure enemyHitboxes is initialized
        if (enemyHitboxes == null)
        {
            Debug.LogError("enemyHitboxes list is not initialized.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!dodge)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.transform.Translate(movement * speed * Time.deltaTime);

            if (movement.magnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
            }

            anim.SetFloat("x", moveHorizontal);
            anim.SetFloat("y", moveVertical);

            float currentSpeed = movement.magnitude;
            anim.SetFloat("speed", currentSpeed);

            if (Input.GetKeyDown(KeyCode.Space) && currentSpeed > 0)
            {
                StartDodge();
            }
        }
        else
        {
            Dodge();
        }
    }

    void StartDodge()
    {
        dodge = true;
        rollTime = rollDuration;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rollDirection = movement.normalized;
        anim.SetBool("dodge", true);
    }

    void Dodge()
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

        // Check if playerHealth is null before using it
        if (playerHealth != null)
        {
            playerHealth.SetInvulnerable(rollDuration);
        }
        else
        {
            Debug.LogError("playerHealth is null when trying to set invulnerability.");
        }

        // Check if enemyHitboxes is null before iterating
        if (enemyHitboxes != null)
        {
            foreach (GameObject hitbox in enemyHitboxes)
            {
                if (hitbox != null)
                {
                    hitbox.SetActive(false); // Deactivate the individual hitbox
                    Debug.Log("Enemy hitbox deactivated during dodge: " + hitbox.name);
                }
                else
                {
                    Debug.LogWarning("A hitbox in enemyHitboxes is null!");
                }
            }

            // Reactivate hitboxes after a delay (e.g., 1 second)
            StartCoroutine(ReactivateHitbox(1f));
        }
        else
        {
            Debug.LogError("enemyHitboxes is null when trying to deactivate hitboxes.");
        }
    }

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