using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat
{
    private Player player;

    public float detectionRadius = 5f;
    public LayerMask enemyLayer = LayerMask.GetMask("Enemy");
    public Transform currentTarget;
    public List<Transform> detectedEnemies = new List<Transform>();
    public int currentTargetIndex = 0;
    public bool alreadyAttacked = false;
    public int attackDamage = 1;
    public int skillDamage = 2;
    public float attackRange = 3f;
    public bool isAttacking = false;
    public bool isAttackMoving = false;

    public delegate void ComboChangedDelegate(int comboCount);
    public event ComboChangedDelegate onComboChanged;

    private int _comboStack = 0;
    public int comboStack
    {
        get { return _comboStack; }
        set
        {
            _comboStack = value;
            onComboChanged?.Invoke(_comboStack);
        }
    }

    private float lastHitTime = 0f;
    private float comboResetTime = 2.5f; // 2.5초 동안 타격이 없으면 콤보 초기화

    private Vector3 attackStartPosition;  // 공격 시작 위치
    private Vector3 attackDirection;      // 공격 방향
    private float attackMoveDistance = 2f; // 공격 이동 거리

    public void Initialize(Player player)
    {
        this.player = player;
    }

    public void DetectEnemies()
    {
        detectedEnemies.Clear();
        Collider[] enemiesInRange = Physics.OverlapSphere(player.transform.position, detectionRadius, enemyLayer);

        foreach (Collider enemy in enemiesInRange)
        {
            detectedEnemies.Add(enemy.transform);
        }

        // 적 탐지 상태를 애니메이터에 반영
        player.an.SetBool("EnemyDetected", detectedEnemies.Count > 0);
    }

    public Transform GetClosestEnemy()
    {
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform enemy in detectedEnemies)
        {
            float distance = Vector3.Distance(player.transform.position, enemy.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
                currentTargetIndex = detectedEnemies.IndexOf(enemy);
            }
        }

        return closestEnemy;
    }

    public void LookAtTarget(Transform target)
    {
        if (target != null)
        {
            Vector3 direction = (target.position - player.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            
            player.transform.rotation = lookRotation;
        }
    }

    public void UpdateMovementAnimation()
    {
        if (currentTarget == null) return;

        float moveX = player.playerMove.horizontalAxis;
        float moveY = player.playerMove.verticalAxis;

        Vector3 inputDirection = new Vector3(moveX, 0, moveY);
        Vector3 localMovement = player.transform.InverseTransformDirection(inputDirection);

        player.an.SetFloat("MoveX", localMovement.x);
        player.an.SetFloat("MoveY", localMovement.z);
    }

    public bool ReadyCheck()
    {
        if (currentTarget == null) return false;

        Vector3 targetDirection = (currentTarget.position - player.transform.position).normalized;

        Vector3 playerMovementDirection = new Vector3(player.playerMove.horizontalAxis, 0, player.playerMove.verticalAxis).normalized;

        float angle = Vector3.Angle(targetDirection, playerMovementDirection);

        if (angle > 120f && playerMovementDirection.magnitude > 0.1f)
        {
            return true;
        }

        return false;
    }

    public bool AttackCheck()
    {
        if (currentTarget == null) return false;

        Vector3 targetDirection = (currentTarget.position - player.transform.position).normalized;

        Vector3 playerMovementDirection = new Vector3(player.playerMove.horizontalAxis, 0, player.playerMove.verticalAxis).normalized;

        float angle = Vector3.Angle(targetDirection, playerMovementDirection);

        if (angle < 30f && playerMovementDirection.magnitude > 0.1f)
        {
            return true;
        }

        return false;
    }

    public void AttackMove(float speed)
    {
        // 현재까지 이동한 거리 계산
        float distanceMoved = Vector3.Distance(attackStartPosition, player.transform.position);
        
        // 목표 거리에 도달하지 않았다면 계속 이동
        if (distanceMoved < attackMoveDistance)
        {
            player.transform.position += attackDirection * speed * Time.deltaTime;
        }
        else
        {
            // 목표 거리에 도달했다면 정확히 그 위치로 설정
            player.transform.position = attackStartPosition + (attackDirection * attackMoveDistance);
            isAttackMoving = false;
        }
    }

    public void Attack(float power)
    {
        if (currentTarget == null) return;
        
        // 공격 시작 시 현재 위치와 방향 저장
        attackStartPosition = player.transform.position;
        attackDirection = (currentTarget.position - attackStartPosition).normalized;
        
        float distanceToTarget = Vector3.Distance(player.transform.position, currentTarget.position);
        
        if (distanceToTarget <= attackRange)
        {
            GameObject targetObject = currentTarget.gameObject;
            if (targetObject.CompareTag("Enemy"))
            {
                var damageable = targetObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }
        alreadyAttacked = true;
    }

    public void Update()
    {
        // 적 탐지 상태 업데이트
        bool hasEnemies = detectedEnemies.Count > 0;
        player.an.SetBool("EnemyDetected", hasEnemies);
        bool comboOver3 = _comboStack >= 3;
        player.an.SetBool("ComboOver3", comboOver3);

        if (!hasEnemies && _comboStack > 0)
        {
            ResetCombo();
        }
        
        // 마지막 타격 이후 일정 시간이 지나면 콤보 초기화
        if (_comboStack > 0 && Time.time - lastHitTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    public void RegisterHit()
    {
        comboStack++;
        lastHitTime = Time.time;
    }

    public void ResetCombo()
    {
        comboStack = 0;
        Debug.Log("Combo Reset");
    }

    public void RemoveEnemy(Transform enemy)
    {
        // 죽은 적이 현재 타겟이었다면 먼저 처리
        if (currentTarget == enemy)
        {
            currentTarget = null;
        }

        // 리스트에서 안전하게 제거
        if (detectedEnemies.Contains(enemy))
        {
            detectedEnemies.Remove(enemy);
        }
        
        // UI 상태 갱신
        player.an.SetBool("EnemyDetected", detectedEnemies.Count > 0);
        
        // 새로운 타겟 찾기
        if (currentTarget == null && detectedEnemies.Count > 0)
        {
            currentTarget = GetClosestEnemy();
        }

        Debug.Log($"Enemy removed. Remaining enemies: {detectedEnemies.Count}");
    }
}
