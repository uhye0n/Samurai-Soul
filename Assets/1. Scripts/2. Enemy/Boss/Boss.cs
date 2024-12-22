using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 30;
    private int currentHealth;
    public bool isInvulnerable { get; private set; }
    
    [Header("Phase Settings")]
    public float invulnerablePhaseTime = 10f;
    public float vulnerablePhaseTime = 5f;
    public Vector3 safePosition;  // 무적 상태일 때의 위치
    public Vector3 battlePosition; // 전투 위치
    public float moveSpeed = 10f;

    [Header("Attack Settings")]
    public GameObject tileAttackPrefab;
    
    private BossState currentState;
    private Animator animator;
    private Player player;
    private Vector3 patternCenter; // 패턴의 중심점 저장용

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        SetState(new BossInvulnerableState(this));
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void SetState(BossState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
        // 필요한 경우 시각적 효과 추가
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        animator?.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (currentState is BossVulnerableState)
        {
            // 취약 상태에서 공격을 받으면 체력 표시 등 추가
        }
    }

    private void Die()
    {
        animator?.SetTrigger("Die");
        // 죽음 처리 로직
        Destroy(gameObject, 2f);
    }

    public void SetPatternCenter(Vector3 center)
    {
        patternCenter = new Vector3(center.x, 0f, center.z);
        Debug.Log($"Pattern center set to: {patternCenter}");
    }

    public void SpawnTileAttack(Vector3 position)
    {
        if (tileAttackPrefab == null)
        {
            Debug.LogError("Tile Attack Prefab is not assigned!");
            return;
        }

        // 월드 좌표에 직접 스폰
        Vector3 spawnPosition = new Vector3(position.x, 0f, position.z);
        GameObject tile = Instantiate(tileAttackPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Spawned tile at: {spawnPosition}");
    }

    // 코루틴을 실행하기 위한 메서드 추가
    public void StartSpiralPattern(System.Action<Vector3> spawnAction)
    {
        StartCoroutine(SpiralPatternCoroutine(spawnAction));
    }

    private IEnumerator SpiralPatternCoroutine(System.Action<Vector3> spawnAction)
    {
        float angle = 0f;
        float radius = 2f;
        Vector3 center = Vector3.zero;
        
        for (int i = 0; i < 20; i++)
        {
            angle += 30f;
            radius += 0.5f;
            Vector3 pos = center + new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );
            spawnAction(pos);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
