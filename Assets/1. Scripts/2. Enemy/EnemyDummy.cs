using UnityEngine;

public class EnemyDummy : MonoBehaviour, IDamageable
{
    public Animator animator;
    public Rigidbody rb;
    
    private int maxHealth = 999;
    private int currentHealth;
    private bool isStunned;
    private float stunDuration = 0.5f;
    private float stunEndTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // 체력은 감소하지 않고 경직만 적용
        isStunned = true;
        stunEndTime = Time.time + stunDuration;
        animator.SetTrigger("Hit");
        
        Debug.Log($"Dummy Hit! Damage: {damage}, Health: {currentHealth}/{maxHealth}");
    }

    public void Update()
    {
        // 경직 상태 업데이트
        if (isStunned && Time.time >= stunEndTime)
        {
            isStunned = false;
        }
    }
}
