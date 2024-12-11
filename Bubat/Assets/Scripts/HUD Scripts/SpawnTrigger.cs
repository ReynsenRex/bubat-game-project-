using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required for using TextMeshPro

public class SpawnTrigger : MonoBehaviour
{
    public List<GameObject> enemies;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Objective"))
        {
            foreach (GameObject enemy in enemies)
            {
                enemy.SetActive(true); // Show each enemy
            }
        }
    }


}
