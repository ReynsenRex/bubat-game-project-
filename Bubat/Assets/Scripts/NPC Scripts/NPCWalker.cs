using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCWalker : MonoBehaviour
{
	public float moveSpeed = 2.0f; // Movement speed
	public float rotationSpeed = 5.0f; // Rotation speed
	public List<GameObject> waypoints; // List of waypoints
	//public NPCHealth health;

	private int currentWaypointIndex = 0; // Current waypoint index
	private Animator anim; // Animator for controlling the speed animation
	private bool isStopped = false; // Flag to indicate if movement is stopped

	void Start()
	{
		if (waypoints == null || waypoints.Count == 0)
		{
			Debug.LogError("Waypoints are not assigned to the NPC.");
		}

		anim = GetComponent<Animator>();
		if (anim == null)
		{
			Debug.LogError("Animator component not found on the NPC.");
		}
	}

	void Update()
	{
		if (isStopped || waypoints == null || waypoints.Count == 0)
		{
			if (anim != null)
			{
				anim.SetFloat("speed", 0); // Stop animation when stopped
			}
			return; // Stop movement if flagged or no waypoints are available
		}

		// Get the current waypoint
		GameObject currentWaypoint = waypoints[currentWaypointIndex];
		float distanceToWaypoint = Vector3.Distance(transform.position, currentWaypoint.transform.position);

		// Move towards the waypoint
		if (distanceToWaypoint > 0.5f) // Stop moving if close to the waypoint
		{
			Vector3 direction = (currentWaypoint.transform.position - transform.position).normalized;
			direction.y = 0; // Keep movement on the horizontal plane

			// Rotate smoothly towards the waypoint
			Quaternion targetRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

			// Move the NPC forward
			transform.position += transform.forward * moveSpeed * Time.deltaTime;

			// Set animator speed
			if (anim != null)
			{
				anim.SetFloat("speed", 1); // Play walk animation
			}
		}
		if (distanceToWaypoint == 0)
		{
			// Move to the next waypoint
			currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
			// Cek apakah waypoint terakhir tercapai
			if (currentWaypointIndex == waypoints.Count - 1)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Panggil fungsi untuk memuat scene berikutnya
			}
			else
			{
				// Move to the next waypoint
				currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
			}
		}
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			// Stop the NPC's movement
			isStopped = true;
			moveSpeed = 0; // Set speed to 0

			// Stop the animation
			if (anim != null)
			{
				anim.SetFloat("speed", 0); // Stop walking animation
			}
		}
		else 
		{
			isStopped = false;
			moveSpeed = 1.0f; // Reset speed to original value
			if (anim != null)
			{
				anim.SetFloat("speed", 1); // Stop walking animation
			}
		}
	}
	
	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			// Stop the NPC's movement
			isStopped = true;
			moveSpeed = 0; // Set speed to 0

			// Stop the animation
			if (anim != null)
			{
				anim.SetFloat("speed", 0); // Stop walking animation
			}
		}
		else 
		{
			isStopped = false;
			moveSpeed = 1.0f; // Reset speed to original value
			if (anim != null)
			{
				anim.SetFloat("speed", 1); // Stop walking animation
			}
		}
	}

}
