using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    public float health = 50.0f; // Total health of the enemy
    private bool isDead = false; // Track if the enemy is dead
    private bool isInvincible = false; // Track if the enemy is currently invincible
    public float invincibilityDuration = 0.3f; // Duration of invincibility after taking damage

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Method to apply damage to the enemy
    public void TakeDamage(float damage)
    {
        audioManager.PlaySFX(audioManager.enemyHurt);

        if (isDead || isInvincible) return; // If the enemy is dead or invincible, ignore damage

        health -= damage; // Subtract damage from health
        Debug.Log("Enemy took damage: " + damage + ". Remaining health: " + health);

        // Start invincibility
        StartCoroutine(InvincibilityCoroutine());

        // Check if health is less than or equal to zero
        if (health <= 0)
        {
            Die(); // Call the Die method if health is depleted
        }
    }

    // Coroutine to handle invincibility
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // Set invincible state
        yield return new WaitForSeconds(invincibilityDuration); // Wait for the duration
        isInvincible = false; // Reset invincible state
    }

    // Method to handle enemy death
    private void Die()
    {
        isDead = true; // Set the enemy as dead
        Debug.Log("Enemy has died.");
        
        // Destroy the enemy GameObject
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        // Optionally, you can play a death animation or particle effect here
    }

    // Optional: Method to reset enemy health (for respawning or other purposes)
    public void ResetHealth()
    {
        health = 50.0f; // Reset to initial health value
        isDead = false; // Mark the enemy as alive
        gameObject.SetActive(true); // Reactivate the enemy GameObject
        Debug.Log("Enemy health reset. Current health: " + health);
    }

    public void ChangeScene()
    {
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}