using UnityEngine;

public class EnemyRabbit : MonoBehaviour, IDamageable
{
    public Animator animator;
    public Rigidbody rb;
    public Player playerTarget; // Player 컴포넌트 직접 참조

    [Header("Stats")]
    private int maxHealth = 3;
    private int currentHealth;
    public float moveSpeed = 5f;
    public float detectionRange = 8f;
    public float attackRange = 2f;
    public float attackCooldown = 5f;

    private bool isStunned;
    private bool isDead;
    private bool isAttacking;
    private float stunDuration = 3f;
    private float stunEndTime;
    private float lastAttackTime;

    public void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        playerTarget = FindObjectOfType<Player>(); // 씬에서 Player 컴포넌트 찾기
    }

    public void Update()
    {
        // playerTarget.playerStats.currentHealth <= 0 대신 isDead 사용
        if (isDead || playerTarget == null || 
            playerTarget.isDead || 
            playerTarget.playerCombat.isAttacking) return;

        UpdateStunState();
        
        if (!isStunned && !isAttacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.transform.position);
            
            if (distanceToPlayer <= detectionRange)
            {
                if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
                {
                    StartAttack();
                    Debug.Log("Attack!");
                }
                else if (distanceToPlayer > attackRange)
                {
                    ChasePlayer();
                }
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
    }

    public void ChasePlayer()
    {
        Vector3 direction = (playerTarget.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, 
                                            Quaternion.LookRotation(direction), 
                                            5f * Time.deltaTime);
        animator.SetBool("isWalking", true);
    }

    private void StartAttack()
    {
        // playerTarget.playerStats.currentHealth <= 0 대신 isDead 사용
        if (isDead || isAttacking || 
            playerTarget.isDead || 
            playerTarget.playerCombat.isAttacking) return;
        
        isAttacking = true;
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        
        // 즉시 데미지 처리
        if (playerTarget != null && playerTarget.playerStats != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.transform.position);
            if (distanceToPlayer <= attackRange)
            {
                Debug.Log("Attempting to damage player");
                playerTarget.playerStats.TakeDamage(1);
            }
        }
        
        // 공격 상태 종료를 위한 타이머 설정
        Invoke("OnAttackEnd", attackCooldown);
    }

    private void UpdateStunState()
    {
        if (isStunned && Time.time >= stunEndTime)
        {
            isStunned = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        isStunned = true;
        stunEndTime = Time.time + stunDuration;
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rb.isKinematic = true;
        OnDeath?.Invoke();  // 죽음 이벤트 발생
        Destroy(gameObject, 2f);
    }

    // Animation Event Methods
    public void OnAttackHit()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.transform.position);
        if (distanceToPlayer <= attackRange)
        {
            if (playerTarget != null && playerTarget.playerStats != null)
            {
                Debug.Log("Attempting to damage player");
                playerTarget.playerStats.TakeDamage(1);
            }
            else
            {
                Debug.Log("Player or PlayerStats is null!");
            }
        }
    }

    public void OnAttackEnd()
    {
        CancelInvoke("OnAttackEnd");  // 기존 Invoke 취소
        isAttacking = false;
        Debug.Log("Attack End");
    }
}
