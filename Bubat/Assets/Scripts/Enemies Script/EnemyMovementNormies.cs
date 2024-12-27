using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementNormies : MonoBehaviour
{
    public string playerTag = "Player";
    public float speed = 1.0f;
    public float rotSpeed = 80.0f;
    public float detectionRange = 5.0f;
    public float attackRange = 1.5f;
    public float attack = 0.0f;
    public float gravity = 8.0f;
    public GameObject weaponHitbox;
    private Vector3 moveDir = Vector3.zero;
    private CharacterController controller;
    private Animator anim;
    private Transform player;
    private float attackDuration;
    private bool isAttacking = false; // Track if currently attacking
    private bool isHit = false; // Track if the enemy is hit

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;

        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }
    }

    private void Update()
    {
        // Check if the enemy is hit
        if (isHit)
        {
            // If hit, do not move or attack
            anim.SetBool("isHit", true);
            return; // Exit the update to prevent further actions
        }
        else
        {
            anim.SetBool("isHit", false); // Reset hit state
        }

        // Check if the player is within detection range
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            // Check if the player is within attack range
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                // If not currently attacking, initiate attack
                if (!isAttacking)
                {
                    AttackPlayer();
                }
            }
            else
            {
                MoveTowardsPlayer(); // Move towards the player if not attacking
            }
        }
        else
        {
            // Reset movement if player is out of range
            moveDir = Vector3.zero; // Stop movement
            anim.SetFloat("speed", 0);
            isAttacking = false; // Reset attack state
        }

        // Apply gravity
        moveDir.y -= gravity * Time.deltaTime;

        // Move the character controller
        controller.Move(moveDir * Time.deltaTime);
    }

    private void MoveTowardsPlayer()
    {
        // Calculate direction towards the player
        Vector3 direction = (player.position - transform.position).normalized;

        // Move the enemy
        moveDir = direction * speed;

        // Set animation speed
        anim.SetFloat("speed", moveDir.magnitude); // Use the magnitude for smoother animation

        // Rotate towards player
        Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 20, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
    }

    private void AttackPlayer()
    {
        // Stop movement during attack
        moveDir = Vector3.zero;
        anim.SetFloat("speed", 0);

        // Randomly select an attack type
        int attackType = Random.Range(0, 2);
        anim.SetFloat("attack", attackType + 1);

        // Enable hitbox
        weaponHitbox.SetActive(true); // Assuming you have a reference to the hitbox

        isAttacking = true;

        StartCoroutine(WaitForAttackAnimation());
    }

    private IEnumerator WaitForAttackAnimation()
    {
        // Wait for the duration of the attack animation
        yield return new WaitForSeconds(1.0f); // Adjust this to match your animation duration

        // Disable hitbox after attack
        weaponHitbox.SetActive(false);

        // Reactivate hitbox after a delay
        yield return new WaitForSeconds(0.5f); // Delay for reactivation if needed
        weaponHitbox.GetComponent<EnemyHitbox>().ReactivateHitbox();

        // Add a delay between attacks
        yield return new WaitForSeconds(2.0f); // 2-second cooldown

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            isAttacking = false;
            AttackPlayer();
        }
        else
        {
            isAttacking = false;
            anim.SetFloat("attack", 0); // Reset attack animation
        }
    }

    public void TakeDamage()
    {
        isHit = true; // Set hit state to true
        anim.SetBool("isHit", true); // Trigger hit animation
        StartCoroutine(HandleHit());
    }

    private IEnumerator HandleHit()
    {
        // Wait for a short duration to allow the hit animation to play
        yield return new WaitForSeconds(1.0f); // Adjust duration as needed
        isHit = false; // Reset hit state after the duration
        anim.SetBool("isHit", false); // Reset hit animation
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Visualize attack range
    }
}