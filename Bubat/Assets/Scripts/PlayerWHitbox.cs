using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWHitbox : MonoBehaviour
{
	public float damageAmount = 10.0f; // Amount of damage to apply

	private void OnTriggerEnter(Collider other)
	{
		// Check if the collider belongs to an enemy
		Debug.Log("HA");
		if (other.CompareTag("Enemy"))
		{
			// Call a method on the enemy to apply damage
			other.GetComponent<EnemyHealth>().TakeDamage(damageAmount);
			Debug.Log("Enemy hit by player hitbox, damage applied: " + damageAmount);
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
