using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public float damageAmount = 10.0f; // Amount of damage to apply

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Call a method on the player to apply damage
            other.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
        }
    }
}