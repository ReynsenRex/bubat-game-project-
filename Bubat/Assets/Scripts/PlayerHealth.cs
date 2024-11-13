using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100.0f;
    private bool isInvulnerable = false;

    public void TakeDamage(float damageAmount)
    {
        if (!isInvulnerable)
        {
            health -= damageAmount;
            Debug.Log("Player took damage: " + damageAmount + ". Remaining health: " + health);

            // Optional: Add logic for player death
            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // Implement player death logic (e.g., disable player, play animation, etc.)
    }

    public void SetInvulnerable(float duration)
    {
        StartCoroutine(InvulnerabilityCoroutine(duration));
    }

    private IEnumerator InvulnerabilityCoroutine(float duration)
    {
        isInvulnerable = true;
        // Optionally, add visual feedback (like flashing or changing color)
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }
}