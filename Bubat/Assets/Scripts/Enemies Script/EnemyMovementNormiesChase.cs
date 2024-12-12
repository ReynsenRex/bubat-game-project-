using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementNormiesChase : MonoBehaviour
{
	public string playerTag = "Player";
    public string allyTag = "Ally"; // Tag for allied NPCs
    public GameObject payloadTarget; // Payload to follow
    public float speed = 1.0f;
    public float rotSpeed = 80.0f;
    public float detectionRange = 5.0f;
    public float attackRange = 1.5f;
    public float gravity = 8.0f;
    public GameObject weaponHitbox;

    private Vector3 moveDir = Vector3.zero;
    private CharacterController controller;
    private Animator anim;
    private Transform currentTarget;
    private Transform player;
    private bool isAttacking = false; // Track if currently attacking

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;

        if (payloadTarget == null)
        {
            Debug.LogError("Payload target not assigned!");
        }

        currentTarget = payloadTarget?.transform;
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            // Check for player or nearest ally within detection range
            Transform newTarget = GetNearestTargetWithinRange();
            if (newTarget != null)
            {
                currentTarget = newTarget; // Switch target to player or nearest ally
            }
            else
            {
                currentTarget = payloadTarget?.transform; // Default back to payload
            }

            // If the target is within attack range, attack
            if (Vector3.Distance(transform.position, currentTarget.position) <= attackRange)
            {
                if (!isAttacking)
                {
                    AttackTarget();
                }
            }
            else
            {
                MoveTowardsTarget(); // Move towards the current target if not attacking
            }
        }
        else
        {
            // Stop moving if there's no target
            moveDir = Vector3.zero;
            anim.SetFloat("speed", 0);
        }

        // Apply gravity
        moveDir.y -= gravity * Time.deltaTime;

        // Move the character controller
        controller.Move(moveDir * Time.deltaTime);
    }

    private void MoveTowardsTarget()
    {
        // Calculate direction towards the target
        Vector3 direction = (currentTarget.position - transform.position).normalized;

        // Move the enemy
        moveDir = direction * speed;

        // Set animation speed
        anim.SetFloat("speed", moveDir.magnitude); // Use the magnitude for smoother animation

        // Rotate towards the target
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
    }

    private void AttackTarget()
    {
        // Stop movement during attack
        moveDir = Vector3.zero;
        anim.SetFloat("speed", 0);

        // Randomly select an attack type
        int attackType = Random.Range(0, 2);
        anim.SetFloat("attack", attackType + 1);

        // Enable hitbox
        if (weaponHitbox != null)
        {
            weaponHitbox.SetActive(true);
        }

        isAttacking = true;

        StartCoroutine(WaitForAttackAnimation());
    }

    private IEnumerator WaitForAttackAnimation()
    {
        // Wait for the duration of the attack animation
        yield return new WaitForSeconds(1.0f); // Adjust this to match your animation duration

        // Disable hitbox after attack
        if (weaponHitbox != null)
        {
            weaponHitbox.SetActive(false);
        }

        // Add a delay between attacks
        yield return new WaitForSeconds(3.0f); // 2-second cooldown

        isAttacking = false;
        anim.SetFloat("attack", 0); // Reset attack animation
    }

    private Transform GetNearestTargetWithinRange()
    {
        Transform nearestTarget = null;
        float closestDistance = detectionRange;

        // Check distance to player
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            nearestTarget = player;
            closestDistance = Vector3.Distance(transform.position, player.position);
        }

        // Check distance to allied NPCs
        GameObject[] allies = GameObject.FindGameObjectsWithTag(allyTag);
        foreach (GameObject ally in allies)
        {
            float distanceToAlly = Vector3.Distance(transform.position, ally.transform.position);
            if (distanceToAlly < closestDistance)
            {
                nearestTarget = ally.transform;
                closestDistance = distanceToAlly;
            }
        }

        return nearestTarget;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Visualize attack range
    }
}
