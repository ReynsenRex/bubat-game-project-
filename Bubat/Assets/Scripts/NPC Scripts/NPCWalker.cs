using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCWalker : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Movement speed
    public float rotationSpeed = 5.0f; // Rotation speed
    public List<GameObject> waypoints; // List of waypoints
    public float detectionRadius = 5.0f; // Radius to detect enemies
    public LayerMask enemyLayer; // Layer mask to identify enemies

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
        isStopped = IsEnemyNearby();

        if (isStopped)
        {
            // Stop the NPC
            anim.SetFloat("speed", 0);
            return;
        }

        // NPC movement logic here
        MoveToNextWaypoint();
    }

    bool IsEnemyNearby()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        return hitColliders.Length > 0;
    }

    void MoveToNextWaypoint()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        GameObject targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.transform.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint.transform.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }

        anim.SetFloat("speed", moveSpeed);
    }
}