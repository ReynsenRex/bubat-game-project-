using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100.0f; // Total health of the player
    private bool isDead = false; // Track if the player is dead
    private bool isInvincible = false; // Track if the player is currently invincible
    public float invincibilityDuration = 0.3f; // Duration of invincibility after taking damage

    // Method to apply damage to the player
    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible) return; // If the player is dead or invincible, ignore damage

        health -= damage; // Subtract damage from health
        Debug.Log("Player took damage: " + damage + ". Remaining health: " + health);

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

    // Method to handle player death
    private void Die()
    {
        isDead = true; // Set the player as dead
        Debug.Log("Player has died.");
        
        // Optionally, you can trigger a respawn or game over logic here
    }

    // Optional: Method to reset player health (for respawning or other purposes)
    public void ResetHealth()
    {
        health = 100.0f; // Reset to initial health value
        isDead = false; // Mark the player as alive
        Debug.Log("Player health reset. Current health: " + health);
    }
}