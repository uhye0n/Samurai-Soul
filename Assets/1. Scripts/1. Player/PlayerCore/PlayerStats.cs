using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private Player player;

    public float runSpeed = 3.5f;
    public float walkSpeed = 2f;
    
    public int maxHealth = 5;
    public int currentHealth;

    public void Initialize(Player player)
    {
        this.player = player;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // 사망 처리
            Debug.Log("Player Dead");
        }
    }
}
