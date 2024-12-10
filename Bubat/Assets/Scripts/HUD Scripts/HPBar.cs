using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Image playerHPBar;
    public Image enemyHPBar;
    private float playerHP = 150;
    private float playerMaxHP = 150;
    private float enemyHP = 150;
    private float enemyMaxHP = 150;
    private float speed = 1;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Simulate player taking damage
        {
            TakeDamage("Player", 30);
        }
        else if (Input.GetKeyDown(KeyCode.H)) // Simulate player healing
        {
            Heal("Player", 30);
        }
        else if (Input.GetKeyDown(KeyCode.E)) // Simulate enemy taking damage
        {
            TakeDamage("Enemy", 30);
        }
        else if (Input.GetKeyDown(KeyCode.R)) // Simulate enemy healing
        {
            Heal("Enemy", 30);
        }

        UpdateHP(playerHPBar, playerHP, playerMaxHP);
        UpdateHP(enemyHPBar, enemyHP, enemyMaxHP);

        // Keep health bar facing the camera
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    private void TakeDamage(string target, float damage)
    {
        if (target == "Player")
        {
            playerHP -= damage;
            if (playerHP < 0) playerHP = 0;
        }
        else if (target == "Enemy")
        {
            enemyHP -= damage;
            if (enemyHP < 0) enemyHP = 0;
        }
    }

    private void Heal(string target, float heal)
    {
        if (target == "Player")
        {
            playerHP += heal;
            if (playerHP > playerMaxHP) playerHP = playerMaxHP;
        }
        else if (target == "Enemy")
        {
            enemyHP += heal;
            if (enemyHP > enemyMaxHP) enemyHP = enemyMaxHP;
        }
    }

    void UpdateHP(Image hpBar, float currentHP, float maxHP)
    {
        float target = currentHP / maxHP;
        hpBar.fillAmount = Mathf.MoveTowards(hpBar.fillAmount, target, speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamage("Player", 10); // Example: player takes 10 damage
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage("Enemy", 10); // Example: enemy takes 10 damage
        }
    }
}
