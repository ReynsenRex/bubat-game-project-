using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossChangeScene : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    private float health;
    // Start is called before the first frame update
    void Start()
    {
        health = enemyHealth.health;   
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            SceneManager.LoadScene("CSC_ALTER_Ending");
        }
    }
}
