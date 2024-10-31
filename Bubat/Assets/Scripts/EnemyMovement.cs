	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	public string playerTag = "Player";
	public float speed = 1.0f;
	public float rotSpeed = 80.0f;
	public float detectionRange = 5.0f;
	public float attackRange = 1.5f;
	
	public float attack = 1.0f;
	public float gravity = 8.0f;
	private Vector3 moveDir = Vector3.zero;
	private CharacterController controller;
	private Animator anim;
	private Transform player;

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

	void Update()
	{
		// Check if the player is within detection range
		if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
		{
			// Check if the player is within attack range
			if (Vector3.Distance(transform.position, player.position) <= attackRange)
			{
				AttackPlayer();
			}
			else
			{
				MoveTowardsPlayer();
			}
		}
		else
		{
			// Reset movement if player is out of range
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

		// Rotate towards the player
		float targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
		float rotation = Mathf.LerpAngle(transform.eulerAngles.y, targetRotation, rotSpeed * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, rotation, 0);
	}

	private void AttackPlayer()
	{
		moveDir = Vector3.zero;
		
		anim.SetFloat("speed", 0); 
		anim.SetFloat("attack", 1);
		
	}

	// Gizmos for visualizing the detection range
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, detectionRange);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, attackRange); // Visualize attack range
	}
}