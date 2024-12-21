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
        Debug.Log($"PlayerStats initialized. Health: {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(int damage)
    {
        if (!player.isDead || !player.playerCombat.isAttacking)
        {
            currentHealth -= damage;
            Debug.Log($"Player taking damage: {damage}. Current health: {currentHealth}/{maxHealth}");
            player.an.SetTrigger("Hit");
        
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                player.isDead = true;  // 플레이어 사망 상태 설정
                Debug.Log("Player Dead");
                player.an.SetBool("Die",true);
            }
        }
    }
}
