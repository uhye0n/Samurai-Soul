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

    // 무적 시간 관련 변수
    public float invincibilityDuration = 1f; // 무적 지속 시간
    private float lastDamageTime;
    private bool isSkillInvincible = false;
    public bool isInvincible => Time.time < lastDamageTime + invincibilityDuration || isSkillInvincible;

    private const float blinkDuration = 0.1f;  // 깜빡임 간격
    private const int blinkCount = 3;          // 깜빡임 횟수
    private Coroutine blinkCoroutine;

    public void Initialize(Player player)
    {
        this.player = player;
        currentHealth = maxHealth;
        Instance = this;  // 싱글톤 인스턴스 설정
        lastDamageTime = -invincibilityDuration; // 시작할 때는 무적이 아닌 상태로
        Debug.Log($"PlayerStats initialized. Health: {currentHealth}/{maxHealth}");
    }

    public void SetInvincible(bool invincible)
    {
        isSkillInvincible = invincible;
    }

    public void TakeDamage(int damage)
    {
        // 무적 상태이거나 죽은 상태, 또는 공격 중이면 데미지를 받지 않음
        if (isInvincible || player.isDead || player.playerCombat.isAttacking)
            return;

        lastDamageTime = Time.time; // 마지막 피격 시간 갱신
        currentHealth -= damage;
        Debug.Log($"Player taking damage: {damage}. Current health: {currentHealth}/{maxHealth}");
        
        // 깜빡임 효과 시작
        if (blinkCoroutine != null)
            player.StopCoroutine(blinkCoroutine);
        blinkCoroutine = player.StartCoroutine(BlinkEffect());

        player.playerCombat.comboStack = 0;
    
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

    private IEnumerator BlinkEffect()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            // 모든 렌더러 비활성화
            foreach (var renderer in player.AllRenderers)
            {
                if (renderer != null)
                    renderer.enabled = false;
            }

            yield return new WaitForSeconds(blinkDuration);

            // 모든 렌더러 활성화
            foreach (var renderer in player.AllRenderers)
            {
                if (renderer != null)
                    renderer.enabled = true;
            }

            yield return new WaitForSeconds(blinkDuration);
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
