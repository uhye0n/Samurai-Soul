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
        if (isInvincible || player.isDead || player.playerCombat.isAttacking)
            return;

        lastDamageTime = Time.time;
        currentHealth -= damage;
        
        if (blinkCoroutine != null)
            player.StopCoroutine(blinkCoroutine);
        blinkCoroutine = player.StartCoroutine(BlinkEffect());

        player.playerCombat.comboStack = 0;
    
        if (currentHealth <= 0)
        {
            HandleDeath();
        }

        onHealthChangedCallback?.Invoke();
    }

    private void HandleDeath()
    {
        currentHealth = 0;
        player.isDead = true;
        onHealthChangedCallback?.Invoke();

        // 죽음 애니메이션 재생
        player.an.SetBool("Die", true);

        // 2초 후에 DeathUI 표시
        player.StartCoroutine(ShowDeathUIAfterDelay());
    }

    private IEnumerator ShowDeathUIAfterDelay()
    {
        yield return new WaitForSeconds(2f); // 죽음 애니메이션을 위한 대기 시간

        DeathUI deathUI = Object.FindObjectOfType<DeathUI>();
        if (deathUI != null)
        {
            deathUI.ShowDeathScreen();
        }
    }

    private IEnumerator BlinkEffect()
    {
        WaitForSeconds blinkWait = new WaitForSeconds(blinkDuration);
        
        for (int i = 0; i < blinkCount && !player.isDead; i++)
        {
            foreach (var renderer in player.AllRenderers)
            {
                if (renderer != null)
                    renderer.enabled = false;
            }

            yield return blinkWait;

            foreach (var renderer in player.AllRenderers)
            {
                if (renderer != null)
                    renderer.enabled = true;
            }

            yield return blinkWait;
        }
    }

    private IEnumerator ResetTimeScale()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 1f;
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
