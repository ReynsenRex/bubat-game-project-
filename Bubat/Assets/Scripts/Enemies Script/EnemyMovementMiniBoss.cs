using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementMiniBoss : MonoBehaviour
{
    public string playerTag = "Player";
    public float speed = 1.0f;
    public float rotSpeed = 80.0f;
    public float detectionRange = 5.0f;
    public float attackRange = 1.5f;
    public float spawnDistance = 2.0f; // Distance to spawn enemy from this enemy (left or right)
    public float spawnAreaRadius = 5.0f; // Radius around the spawn point to place the new enemies
    public float gravity = 8.0f;
    public GameObject weaponHitbox;
    public GameObject weaponHitbox2;
    public GameObject enemyPrefab; // Prefab of the enemy to spawn
    private EnemyHealth enemyHealth; // Reference to the enemy's health script
    public GameObject spawnPoint; // GameObject to attach the spawn point to the enemy (this can be an empty GameObject)

    private Vector3 moveDir = Vector3.zero;
    private CharacterController controller;
    private Animator anim;
    private Transform player;
    private bool isAttacking = false; // Track if currently attacking
    private bool isSpawning = false;  // Track if the enemy is in the spawn animation
    private int spawnCount = 0; // Count how many times the enemy has spawned (maximum 2)

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        enemyHealth = GetComponent<EnemyHealth>(); // Get the enemy health script

        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }

        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth script not found!");
        }
    }

    private void Update()
    {
        // Check if the player is within detection range
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            // Check if the player is within attack range
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                // If not currently attacking, initiate attack
                if (!isAttacking && !isSpawning)
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

        // Check health and trigger spawn when health reaches 60 or 30 (only if spawnCount < 2)
        if (spawnCount < 2)
        {
            if (enemyHealth.health <= 60f && enemyHealth.health > 30f)
            {
                SpawnEnemy(); // Spawn enemy when health reaches 60
            }

            if (enemyHealth.health <= 30f)
            {
                SpawnEnemy(); // Spawn enemy when health reaches 30
            }
        }

        // Check if spawning is triggered
        if (isSpawning)
        {
            // Stop movement while spawning
            moveDir = Vector3.zero;
            anim.SetFloat("speed", 0);
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
        weaponHitbox2.SetActive(true); // Assuming you have a reference to the hitbox

        isAttacking = true;

        // Start the coroutine to wait for attack animation to complete
        StartCoroutine(WaitForAttackAnimation());
    }

    private IEnumerator WaitForAttackAnimation()
    {
        // Wait for the duration of the attack animation (adjust based on attack duration)
        yield return new WaitForSeconds(1.0f);

        // Disable hitboxes after the attack animation
        weaponHitbox.SetActive(false);
        weaponHitbox2.SetActive(false);

        // Optionally, add a short delay before reactivating hitboxes (if needed)
        yield return new WaitForSeconds(0.5f);
        weaponHitbox.GetComponent<EnemyHitbox>().ReactivateHitbox();
        weaponHitbox2.GetComponent<EnemyHitbox>().ReactivateHitbox();

        // After the attack, reset attack state
        isAttacking = false;
        anim.SetFloat("attack", 0); // Reset attack animation

        // If the player is still within attack range, attempt another attack
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            AttackPlayer(); // Initiate another attack if player is still in range
        }
    }

    public void TakeDamage(float damage)
    {
        enemyHealth.TakeDamage(damage); // Use the health system to take damage
    }

    public void SpawnEnemy()
    {
        // Check if spawning is triggered only if spawnCount < 2
        if (spawnCount < 2)
        {
            // Increment spawn count
            spawnCount++;

            // Set isSpawning to true to prevent other actions while spawning
            isSpawning = true;

            // Trigger the spawn animation
            anim.SetBool("spawn", true);

            // Start the coroutine to handle the spawn process
            StartCoroutine(HandleEnemySpawn());
        }
    }

    private IEnumerator HandleEnemySpawn()
    {
        // Wait for the spawn animation to finish (adjust the time based on the animation length)
        yield return new WaitForSeconds(1.0f);

        // Randomize spawn position within the spawn area (circle or square around the spawn point)
        Vector3 randomSpawnOffset = new Vector3(Random.Range(-spawnAreaRadius, spawnAreaRadius), 0f, Random.Range(-spawnAreaRadius, spawnAreaRadius));

        // Calculate the spawn position by adding the random offset to the spawn point
        Vector3 spawnPosition = spawnPoint.transform.position + randomSpawnOffset;

        // Instantiate the enemy at the calculated spawn position
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity); // Spawn with no rotation

        // After spawning, reset the spawn flag and animation
        isSpawning = false;
        anim.SetBool("spawn", false); // Reset spawn animation
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Visualize attack range

        // Visualize the spawn area (optional)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint.transform.position, spawnAreaRadius);
    }
}
