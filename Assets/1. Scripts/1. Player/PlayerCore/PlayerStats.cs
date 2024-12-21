using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    // 싱글톤 구현
    private static PlayerStats instance;
    public static PlayerStats Instance { get; private set; }

    private Player player;
    public delegate void OnHealthChangedDelegate();
    public OnHealthChangedDelegate onHealthChangedCallback;

    public float runSpeed = 3.5f;
    public float walkSpeed = 2f;
    
    public int maxHealth = 5;
    public int currentHealth;
    public int maxTotalHealth = 10;

    public void Initialize(Player player)
    {
        this.player = player;
        currentHealth = maxHealth;
        Instance = this;  // 싱글톤 인스턴스 설정
        Debug.Log($"PlayerStats initialized. Health: {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(int damage)
    {
        if (!player.isDead && !player.playerCombat.isAttacking)
        {
            currentHealth -= damage;
            Debug.Log($"Player taking damage: {damage}. Current health: {currentHealth}/{maxHealth}");
            player.an.SetTrigger("Hit");
        
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                player.isDead = true;
                Debug.Log("Player Dead");
                player.an.SetBool("Die", true);
            }

            if (onHealthChangedCallback != null)
                onHealthChangedCallback.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (!player.isDead)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            if (onHealthChangedCallback != null)
                onHealthChangedCallback.Invoke();
        }
    }

    public void AddMaxHealth()
    {
        if (maxHealth < maxTotalHealth)
        {
            maxHealth += 1;
            currentHealth = maxHealth;
            if (onHealthChangedCallback != null)
                onHealthChangedCallback.Invoke();
        }
    }

    // UI 시스템을 위한 프로퍼티들
    public float Health { get { return currentHealth; } }
    public float MaxHealth { get { return maxHealth; } }
    public float MaxTotalHealth { get { return maxTotalHealth; } }
}
