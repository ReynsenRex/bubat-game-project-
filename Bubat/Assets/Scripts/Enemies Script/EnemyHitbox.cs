using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
	public float damageAmount = 10.0f; // Amount of damage to apply
	private GameObject player;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void OnTriggerEnter(Collider other)
	{
		// Check if the collider belongs to the player
		if (other.CompareTag("Player"))
		{
			// Call a method on the player to apply damage
			other.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
		}
		
		if (other.CompareTag("Objective"))
		{
			// Call a method on the player to apply damage
			other.GetComponent<NPCHealth>().TakeDamage(damageAmount);
		}
		
		if (other.CompareTag("Block"))
		{
			// Deactivate the enemy's weapon hitbox
			DeactivateHitbox();

			// Notify the player of the block
			player.GetComponent<Movement>().OnBlockSuccess(transform.position);
		}
	}

	public void DeactivateHitbox()
	{
		gameObject.SetActive(false);
	}

	public void ReactivateHitbox()
	{
		gameObject.SetActive(true);
	}
}